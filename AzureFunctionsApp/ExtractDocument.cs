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
    public static class ExtractDocument
    {
        [FunctionName("ExtractDocument")]
        public static void Run([TimerTrigger("0 */5 * * * *"
#if DEBUG
            , RunOnStartup=true
#endif
            )]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function \"ExtractDocument\" executed at: {DateTime.Now}");
            using (var db = new TheCompanyDbContext())
            {
                bool stop = false;
                while (!stop)
                {
                    Invoice invoice = db.Invoices.Where(x => x.ShouldBeExtracted == true && x.IsExtracted == false && string.IsNullOrEmpty(x.LockedBy)).FirstOrDefault();
                    if (invoice != null)
                    {
                        invoice.LockedBy = "ExtractDocument-0000";
                        db.SaveChanges();

                        // TODO: run in a task
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
                            Array.ForEach(Result.Words, delegate (OcrResult.Word word) {
                                ExtractBlock extractBlock = new ExtractBlock(word.X, word.Y, word.Height, word.Width, word.Text);
                                result.Add(extractBlock);
                            });
                            byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                            
                            MemoryStream stream = new MemoryStream(byteArray);
                            containerClient.UploadBlob(id.ToString(), stream);
                            invoice.ExtractId = id;
                        }

                        List<ExtractionSettings> extractSettings = db.ExtractionSettings.Where(x => x.Owner == invoice.Owner && x.DataSource == "Invoices").ToList();
                        extractSettings.ForEach(item => {
                            Rectangle rect = new Rectangle(item.X, item.Y, item.Width, item.Height);
                            using (OcrInput Input = new OcrInput(rect, tempFileName))
                            {
                                OcrResult tmp = Ocr.Read(Input);
                                if (item.Field == "InvoiceNumber") // TODO: reflection
                                {
                                    invoice.InvoiceNumber = tmp.Text;
                                }
                                else if (item.Field == "Address")
                                {
                                    string address = String.Join(System.Environment.NewLine, tmp.Text.Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                                    address = String.Join(" ", tmp.Text.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                                    invoice.CustomerAddress = new Address(address);
                                }
                            }
                        });

                        File.Delete(tempFileName);
                        // TODO: delete previous OCR file
                        
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
            log.LogInformation($"End of function \"ExtractDocument\" at: {DateTime.Now}");
        }
    }
}
