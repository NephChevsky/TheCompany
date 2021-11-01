using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Azure.Storage.Blobs;
using AzureFunctionsApp.Models;
using DbApp.Models;
using IronOcr;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ModelsApp;
using Newtonsoft.Json;
using static IronOcr.OcrResult;

namespace AzureFunctionsApp
{
    public class ExtractDocument
    {
        private readonly ILogger<ExtractDocument> _logger;
        public ExtractDocument(ILogger<ExtractDocument> logger)
        {
            _logger = logger;
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
                        BlobContainerClient containerClient = new BlobContainerClient("UseDevelopmentStorage=true", invoice.Owner.ToString()); //TODO use Azure dll
                        string contentType;
                        new FileExtensionContentTypeProvider().TryGetContentType(invoice.FileName, out contentType);
                        string fileExtension = invoice.FileName.Split(".").Last();
                        string tempFileName = Path.GetTempFileName().Replace(".tmp", string.Concat(".", fileExtension));
                        containerClient.GetBlobClient(invoice.FileId.ToString()).DownloadTo(tempFileName);
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
                            Array.ForEach(Result.Words, delegate (OcrResult.Word word) {
                                ExtractBlock extractBlock = new ExtractBlock(word.X, word.Y, word.Height, word.Width, word.Text);
                                result.Add(extractBlock);
                            });
                            byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                            
                            MemoryStream stream = new MemoryStream(byteArray);
                            if (invoice.ExtractId != Guid.Empty)
                                containerClient.DeleteBlob(invoice.ExtractId.ToString());
                            containerClient.UploadBlob(id.ToString(), stream);
                            invoice.ExtractId = id;
                        }

                        string newCustomerNumber = "";
                        string newCustomerLastName = "";
                        string newCustomerFirstName = "";
                        List<ExtractionSettings> extractSettings = db.ExtractionSettings.Where(x => x.Owner == invoice.Owner && x.DataSource == "Invoice").ToList();
                        extractSettings.ForEach(item => {
                            Rectangle rect = new Rectangle(item.X, item.Y, item.Width, item.Height);
                            using (OcrInput Input = new OcrInput(rect, tempFileName))
                            {
                                OcrResult tmp = Ocr.Read(Input);
                                if (item.Field == "InvoiceNumber") // TODO: reflection
                                {
                                    invoice.InvoiceNumber = tmp.Text;
                                }
                                else if (item.Field == "CustomerId")
                                {
                                    Individual dbInvidual = db.Customers_Individual.Where(x => x.CustomerId == tmp.Text && x.Owner == invoice.Owner).SingleOrDefault();
                                    if (dbInvidual != null)
                                        invoice.CustomerId = dbInvidual.Id;
                                    else
                                        newCustomerNumber = tmp.Text;
                                }
                                else if (item.Field == "Address")
                                {
                                    string address = String.Join(System.Environment.NewLine, tmp.Text.Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                                    address = String.Join(" ", address.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                                    invoice.CustomerAddress = address;
                                }
                                else if (item.Field == "LastName")
								{
                                    newCustomerLastName = tmp.Text;
								}
                                else if (item.Field == "FirstName")
                                {
                                    newCustomerFirstName = tmp.Text;
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(newCustomerNumber))
						{
                            Individual newCustomer = new Individual();
                            newCustomer.CustomerId = newCustomerNumber;
                            newCustomer.LastName = newCustomerLastName;
                            newCustomer.FirstName = newCustomerFirstName;
                            newCustomer.Owner = invoice.Owner;
                            newCustomer.Address = invoice.CustomerAddress;
                            newCustomer = db.Customers_Individual.Add(newCustomer).Entity;
                            invoice.CustomerId = newCustomer.Id;
						}

                        File.Delete(tempFileName);

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
