using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using AzureFunctionsApp.Models;
using DbApp.Models;
using IronOcr;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using ModelsApp.DbModels;
using Newtonsoft.Json;
using StorageApp;
using static IronOcr.OcrResult;

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
                        if (!storage.GetFile(invoice.FileId.ToString(), out inputStream))
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

                        List<ExtractBlock> result = new List<ExtractBlock>();
                        Guid id = Guid.NewGuid();
                        IronTesseract Ocr = new IronTesseract();
                        using (OcrInput Input = new OcrInput(tempFileName))
                        {
                            // Input.Deskew();  // use if image not straight
                            // Input.DeNoise(); // use if image contains digital noise
                            OcrResult Result = Ocr.Read(Input);
                            ExtractBlock size = new ExtractBlock(Result.Pages[0].ContentArea.X, Result.Pages[0].ContentArea.Y, Result.Pages[0].ContentArea.Height, Result.Pages[0].ContentArea.Width, "");
                            result.Add(size);
                            Array.ForEach(Result.Words, delegate (OcrResult.Word word)
                            {
                                ExtractBlock extractBlock = new ExtractBlock(word.X, word.Y, word.Height, word.Width, word.Text);
                                result.Add(extractBlock);
                            });

                            string newCustomerNumber = "";
                            string newCustomerLastName = "";
                            string newCustomerFirstName = "";
                            List<ExtractionSettings> extractSettings = db.ExtractionSettings.Where(x => x.Owner == invoice.Owner && x.DataSource == "Invoice" && x.IsLineItem == false).ToList();
                            extractSettings.ForEach(item =>
                            {
                                Rectangle rect = new Rectangle(item.X, item.Y, item.Width, item.Height);
                                string txt = ExtractTextInArea(Result, rect);

                                if (item.Field == "InvoiceNumber") // TODO: reflection
                                {
                                    invoice.InvoiceNumber = txt;
                                }
                                else if (item.Field == "CustomerId")
                                {
                                    invoice.CustomerNumber = txt;
                                    Individual dbInvidual = db.Individuals.Where(x => x.CustomerId == txt && x.Owner == invoice.Owner).SingleOrDefault();
                                    if (dbInvidual != null)
                                    {

                                        invoice.CustomerId = dbInvidual.Id;
                                    }
                                    else
                                        newCustomerNumber = txt;
                                }
                                else if (item.Field == "Address")
                                {
                                    string address = String.Join(System.Environment.NewLine, txt.Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                                    address = String.Join(" ", address.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                                    invoice.CustomerAddress = address;
                                }
                                else if (item.Field == "LastName")
                                {
                                    newCustomerLastName = txt;
                                }
                                else if (item.Field == "FirstName")
                                {
                                    newCustomerFirstName = txt;
                                }
                            });

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
                                    List<ExtractBlock> references = GetLinesInArea(Result, rect);
                                    references.OrderBy(x => x.Y);
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
                                                    string txt = ExtractTextInArea(Result, areaToExtract);
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
                                                });
                                            }
                                            if (!(lineItem.Quantity == 0 && lineItem.UnitaryPrice == 0 && lineItem.Price ==0))
											{
                                                lineItem.CreationDateTime = DateTime.Now;
                                                db.InvoiceLineItems.Add(lineItem);
                                            }
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(newCustomerNumber))
                            {
                                Individual newCustomer = new Individual();
                                newCustomer.CustomerId = newCustomerNumber;
                                newCustomer.LastName = newCustomerLastName;
                                newCustomer.FirstName = newCustomerFirstName;
                                newCustomer.Owner = invoice.Owner;
                                newCustomer.Address = invoice.CustomerAddress;
                                newCustomer = db.Individuals.Add(newCustomer).Entity;
                                invoice.CustomerId = newCustomer.Id;
                            }

                            byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                            MemoryStream outputStream = new MemoryStream(byteArray);
                            if (invoice.ExtractId != Guid.Empty)
                            {
                                try
								{
                                    if(!storage.DeleteFile(invoice.ExtractId.ToString()))
                                    {
                                        throw new Exception("Couldn't delete previous extraction file");
									}
                                }
                                catch (StorageException e) when (e.RequestInformation.ErrorCode == "BlobNotFound")
								{
                                    _logger.LogInformation("Blob " + invoice.ExtractId.ToString() + " doesn't exist in storage");
								}
                            }
                            if (!storage.CreateFile(id.ToString(), outputStream))
                            {
                                throw new Exception("Couldn't create extraction file " + id.ToString());
                            }
                            invoice.ExtractId = id;
                            invoice.ExtractDateTime = DateTime.Now;
                            invoice.LockedBy = null;
                            invoice.IsExtracted = true;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        stop = true;
                    }
                }
            }
            _logger.LogInformation(string.Concat("Function \"ExtractDocument\" ended (Id: ", appId, ")"));
        }

        private string ExtractTextInArea(OcrResult input, Rectangle rect)
		{
            string text = null;
            foreach (Word tmp in input.Words)
			{
                bool left = tmp.X + tmp.Width < rect.X;
                bool right = tmp.X > rect.X + rect.Width;
                bool above = tmp.Y > rect.Y + rect.Height;
                bool below = tmp.Y + tmp.Height < rect.Y;
                if (!(left || right || above || below))
                {
                    if (text != null)
                        text += " ";
                    text += tmp.Text;
                }
            }
            return text;
		}

        private List<ExtractBlock> GetLinesInArea(OcrResult input, Rectangle rect)
		{
            List<ExtractBlock> result = new List<ExtractBlock>();
            List<Word> list = new List<Word>();
            foreach (Word tmp in input.Words)
            {
                bool left = tmp.X + tmp.Width < rect.X;
                bool right = tmp.X > rect.X + rect.Width;
                bool above = tmp.Y > rect.Y + rect.Height;
                bool below = tmp.Y + tmp.Height < rect.Y;
                if (!(left || right || above || below))
                {
                    list.Add(tmp);
                }
            }
            list.ForEach(currentBlock => {
                ExtractBlock existingBlock = result.Find(aBlock => currentBlock.Y == aBlock.Y);
                if (existingBlock != null)
				{
                    existingBlock.Text += " " + currentBlock.Text;
                    existingBlock.Width = currentBlock.X + currentBlock.Width - existingBlock.X;
				}
                else
				{
                    ExtractBlock newBlock = new ExtractBlock(currentBlock.X, currentBlock.Y, currentBlock.Height, currentBlock.Width, currentBlock.Text);
                    result.Add(newBlock);
				}
            });
            return result;
        }
    }
}
