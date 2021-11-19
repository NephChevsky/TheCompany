using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using DbApp.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using ModelsApp.DbModels;
using Newtonsoft.Json;
using StorageApp;
using OcrApp;
using System.Reflection;
using OcrApp.Models;

namespace AzureFunctionsApp
{
    public class ExtractDocument
    {
        private readonly ILogger<ExtractDocument> _logger;
        private IConfiguration Configuration { get; }
        public ExtractDocument(ILogger<ExtractDocument> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }


        [FunctionName("ExtractDocument")]
        public void Run([TimerTrigger("0 */5 * * * *"
#if DEBUG
            , RunOnStartup=true
#endif
            )]TimerInfo myTimer)
        {
            Guid appId = Guid.NewGuid();
            _logger.LogInformation(string.Concat("Function \"ExtractDocument\" triggered by time (Id: ", appId, ")"));
            using (var db = new TheCompanyDbContext())
            {
                bool stop = false;
                while (!stop)
                {
                    Invoice invoice = db.Invoices.Where(x => x.ShouldBeExtracted == true && x.IsExtracted == false && string.IsNullOrEmpty(x.LockedBy)).FirstOrDefault();
                    if (invoice != null)
                    {
                        invoice.LockedBy = string.Concat("ExtractDocument-", appId);
                        db.SaveChanges();

                        // TODO: run in a task (not sure if it's worth it in azure function since it will be parallelized by kubernets
                        Storage storage = new Storage(Configuration.GetSection("Storage"), invoice.Owner);
                        MemoryStream inputStream;
                        if (!storage.GetFile(invoice.FileId, out inputStream))
						{
                            throw new Exception("Couldn't retrieve file " + invoice.FileId.ToString());
						}
                        string contentType;
                        new FileExtensionContentTypeProvider().TryGetContentType(invoice.FileName, out contentType);
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
                        List<ExtractionSettings> extractSettings = db.ExtractionSettings.Where(x => x.Owner == invoice.Owner && x.DataSource == "Invoice" && x.IsLineItem == false).ToList();
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
                            Individual dbIndividual = db.Individuals.Where(x => x.CustomerNumber == invoice.CustomerNumber && x.Owner == invoice.Owner).SingleOrDefault();
                            if (dbIndividual == null)
							{
                                dbIndividual = new Individual();
                                Guid customerId = Guid.NewGuid();
                                string address = String.Join(System.Environment.NewLine, invoice.CustomerAddress.Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                                address = String.Join(" ", address.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                                dbIndividual.Id = customerId;
                                dbIndividual.CustomerNumber = invoice.CustomerNumber;
                                dbIndividual.Owner = invoice.Owner;
                                dbIndividual.Address = address;
                                dbIndividual.FirstName = invoice.CustomerFirstName;
                                dbIndividual.LastName = invoice.CustomerLastName;
                                invoice.CustomerId = customerId;
                                db.Individuals.Add(dbIndividual);
                            }
                            invoice.CustomerId = dbIndividual.Id;
                        }

                        db.InvoiceLineItems.RemoveRange(db.InvoiceLineItems.Where(x => x.Owner == invoice.Owner && x.InvoiceId == invoice.Id));

                        ExtractionSettings dbBox = db.ExtractionSettings.Where(x => x.Owner == invoice.Owner && x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "LineItem").SingleOrDefault();
                        if (dbBox != null)
                        {
                            ExtractionSettings reference = db.ExtractionSettings.Where(x => x.Owner == invoice.Owner && x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Reference").SingleOrDefault();
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
                                        List<ExtractionSettings> lineItemFields = db.ExtractionSettings.Where(x => x.Owner == invoice.Owner && x.DataSource == "Invoice" && x.IsLineItem == true && x.Field != "LineItem" && x.Field != "Reference").ToList();
                                        for (int i = 0; i < references.Count; i++)
                                        {
                                            ExtractBlock referenceLine = references.ElementAt(i);
                                            InvoiceLineItem lineItem = new InvoiceLineItem(invoice.Id, invoice.Owner);
                                            lineItem.Reference = referenceLine.Text;
                                            if (lineItemFields.Count != 0)
                                            {
                                                ExtractBlock nextreference = null;
                                                if (i != references.Count - 1)
                                                    nextreference = references.ElementAt(i + 1);

                                                lineItemFields.ForEach(field => {
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
                                                db.InvoiceLineItems.Add(lineItem);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        List<ExtractBlock> result;
                        if (!ocr.GetExtractedBlocks(out result))
						{
                            throw new Exception("Couldn't get list of extracted blocks");
						}
                        byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                        MemoryStream outputStream = new MemoryStream(byteArray);
                        if (invoice.ExtractId != Guid.Empty)
                        {
                            if(!storage.DeleteFile(invoice.ExtractId))
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
                    }
                    else
                    {
                        stop = true;
                    }
                }
            }
            _logger.LogInformation(string.Concat("Function \"ExtractDocument\" ended (Id: ", appId, ")"));
        }
    }
}
