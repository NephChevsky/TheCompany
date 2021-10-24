using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Azure.Storage.Blobs;
using AzureFunctionsApp.Models;
using DbApp.Models;
using IronOcr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                        IronTesseract Ocr = new IronTesseract();
                        using (OcrInput Input = new OcrInput(tempFileName))
                        {
                            // Input.Deskew();  // use if image not straight
                            // Input.DeNoise(); // use if image contains digital noise
                            OcrResult Result = Ocr.Read(Input);
                            Array.ForEach(Result.Words, delegate (OcrResult.Word word){
                                ExtractBlock extractBlock = new ExtractBlock(word.X, word.Y, word.Height, word.Width, word.Text);
                                result.Add(extractBlock);
                            });
                        }
                        byte[] byteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
                        MemoryStream stream = new MemoryStream(byteArray);
                        Guid id = Guid.NewGuid();
                        containerClient.UploadBlob(id.ToString(), stream);
                        File.Delete(tempFileName);
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
            log.LogInformation($"End of function \"ExtractDocument\" at: {DateTime.Now}");
        }
    }
}
