using DbApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelsApp.DbModels;
using Newtonsoft.Json;
using OcrApp;
using OcrApp.Models;
using StorageApp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerServiceApp
{
	public class ExtractDocument : BackgroundService
	{
		private readonly ILogger<ExtractDocument> _logger;
        private IConfiguration Configuration { get; }

        private Guid AppId { get; set; }

        public ExtractDocument(ILogger<ExtractDocument> logger, IConfiguration configuration)
		{
			_logger = logger;
            Configuration = configuration;
            AppId = Guid.NewGuid();
        }

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation(string.Concat("Function \"ExtractDocument\" started (Id: ", AppId, ")"));
			
			while (!stoppingToken.IsCancellationRequested)
			{
                using (var db = new TheCompanyDbContext(Guid.Empty))
                {
                    bool stop = false;
                    while (!stop && !stoppingToken.IsCancellationRequested)
                    {
                        Invoice invoice = db.Invoices.Where(x => x.ShouldBeExtracted == true && x.IsExtracted == false && string.IsNullOrEmpty(x.LockedBy)).FirstOrDefault();
                        if (invoice != null)
                        {
                            _logger.LogInformation(string.Concat("Start processing invoice " + invoice.Id.ToString()));
                            db.SetOwner(invoice.Owner);
                            invoice.LockedBy = string.Concat("ExtractDocument-", AppId);
                            db.SaveChanges();

                            // TODO: run in a task (not sure if it's worth it in azure function since it will be parallelized by kubernets
                            Storage storage = new Storage(Configuration.GetSection("Storage"), invoice.Owner);
                            MemoryStream inputStream;
                            if (!storage.GetFile(invoice.FileId, out inputStream))
                            {
                                throw new Exception("Couldn't retrieve file " + invoice.FileId.ToString());
                            }
                            string fileExtension = invoice.FileName.Split(".").Last();
                            string tempFileName = Path.GetTempFileName().Replace(".tmp", string.Concat(".", fileExtension));
                            using (var fs = new FileStream(tempFileName, FileMode.OpenOrCreate))
                            {
                                inputStream.Seek(0, SeekOrigin.Begin);
                                inputStream.CopyTo(fs);
                            }

                            Ocr ocr = new Ocr(Configuration.GetSection("Ocr"));
                            if (!ocr.ExtractPDF(tempFileName))
                            {
                                throw new Exception("Couldn't extract PDF file");
                            }

                            Type type = typeof(Invoice);
                            List<ExtractionSettings> extractSettings = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == false).ToList();
                            extractSettings.ForEach(item =>
                            {
                                Rectangle rect = new Rectangle(item.X, item.Y, item.Width, item.Height);
                                string txt;
                                if (ocr.ExtractTextInArea(rect, out txt))
                                {
                                    PropertyInfo property = type.GetProperty(item.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                    if (property == null)
                                    {
                                        throw new Exception("Unknown property");
                                    }
                                    if (property.PropertyType == typeof(DateTime))
                                    {
                                        property.SetValue(invoice, DateTime.Parse(txt));
                                    }
                                    else if (property.PropertyType == typeof(string))
                                    {
                                        property.SetValue(invoice, txt);
                                    }
                                    else if (property.PropertyType == typeof(double))
                                    {
                                        property.SetValue(invoice, Double.Parse(txt));
                                    }
                                    else if (property.PropertyType == typeof(Guid))
                                    {
                                        property.SetValue(invoice, Guid.Parse(txt));
                                    }
                                    else
                                    {
                                        throw new Exception("Unknow property type");
                                    }
                                }
                            });

                            if (!string.IsNullOrEmpty(invoice.CustomerNumber))
                            {
                                Individual dbIndividual = db.Individuals.Where(x => x.CustomerNumber == invoice.CustomerNumber).SingleOrDefault();
                                if (dbIndividual == null)
                                {
                                    dbIndividual = new Individual();
                                    Guid customerId = Guid.NewGuid();
                                    dbIndividual.Id = customerId;
                                    dbIndividual.CustomerNumber = invoice.CustomerNumber;
                                    dbIndividual.FirstName = invoice.CustomerFirstName;
                                    dbIndividual.LastName = invoice.CustomerLastName;
                                    if (!string.IsNullOrEmpty(invoice.CustomerAddress))
                                    {
                                        string address = String.Join(System.Environment.NewLine, invoice.CustomerAddress.Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                                        address = String.Join(" ", address.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                                        dbIndividual.Address = address;
                                    }
                                    invoice.CustomerId = customerId;
                                    db.Individuals.Add(dbIndividual);
                                }
                                invoice.CustomerId = dbIndividual.Id;
                            }

                            db.LineItems.RemoveRange(db.LineItems.Where(x => x.InvoiceId == invoice.Id));

                            ExtractionSettings dbBox = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "LineItem").SingleOrDefault();
                            if (dbBox != null)
                            {
                                ExtractionSettings reference = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Reference").SingleOrDefault();
                                if (reference != null)
                                {
                                    Rectangle rect = new Rectangle(reference.X, dbBox.Y, reference.Width, dbBox.Height);
                                    rect.X = reference.X;
                                    rect.Width = reference.Width;
                                    List<ExtractBlock> references;
                                    if (ocr.ExtractLinesInArea(rect, out references))
                                    {
                                        if (references != null)
                                        {
                                            List<ExtractionSettings> lineItemFields = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field != "LineItem" && x.Field != "Reference").ToList();
                                            for (int i = 0; i < references.Count; i++)
                                            {
                                                ExtractBlock referenceLine = references.ElementAt(i);
                                                LineItem lineItem = new LineItem(invoice.Id);
                                                lineItem.Reference = referenceLine.Text;
                                                if (lineItemFields.Count != 0)
                                                {
                                                    ExtractBlock nextreference = null;
                                                    if (i != references.Count - 1)
                                                        nextreference = references.ElementAt(i + 1);

                                                    lineItemFields.ForEach(field =>
                                                    {
                                                        int endY = dbBox.Y + dbBox.Height;
                                                        if (nextreference != null)
                                                            endY = nextreference.Y - 1;
                                                        Rectangle areaToExtract = new Rectangle(field.X, referenceLine.Y, field.Width, endY - referenceLine.Y);
                                                        string txt;
                                                        if (ocr.ExtractTextInArea(areaToExtract, out txt))
                                                        {
                                                            double tmp;
                                                            switch (field.Field)
                                                            {
                                                                case "Description":
                                                                    lineItem.Description = txt;
                                                                    break;
                                                                case "Quantity":
                                                                    if (Double.TryParse(txt, out tmp))
                                                                        lineItem.Quantity = tmp;
                                                                    break;
                                                                case "UnitaryPrice":
                                                                    if (Double.TryParse(txt, out tmp))
                                                                        lineItem.UnitaryPrice = tmp;
                                                                    break;
                                                                case "Price":
                                                                    if (Double.TryParse(txt, out tmp))
                                                                        lineItem.Price = tmp;
                                                                    break;
                                                            }
                                                        }
                                                    });
                                                }
                                                if (!(lineItem.Quantity == 0 && lineItem.UnitaryPrice == 0 && lineItem.Price == 0))
                                                {
                                                    lineItem.CreationDateTime = DateTime.Now;
                                                    db.LineItems.Add(lineItem);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            List<ExtractBlock> result = ocr.GetExtractedBlocks(true);
                            byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                            MemoryStream outputStream = new MemoryStream(byteArray);
                            if (invoice.ExtractId != Guid.Empty)
                            {
                                if (!storage.DeleteFile(invoice.ExtractId))
                                {
                                    throw new Exception("Couldn't delete previous extraction file");
                                }
                            }
                            Guid id;
                            if (!storage.CreateFile(outputStream, out id))
                            {
                                throw new Exception("Couldn't create extraction file " + id.ToString());
                            }
                            invoice.ExtractId = id;
                            invoice.ExtractDateTime = DateTime.Now;
                            invoice.LockedBy = null;
                            invoice.IsExtracted = true;
                            db.SaveChanges();
                            db.SetOwner(Guid.Empty);
                            _logger.LogInformation(string.Concat("Invoice " + invoice.Id.ToString() + " has been processed"));
                        }
                        else
                        {
                            stop = true;
                        }
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
            _logger.LogInformation(string.Concat("Function \"ExtractDocument\" ended (Id: ", AppId, ")"));
        }
	}
}
