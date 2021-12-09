using DbApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelsApp.DbModels;
using Newtonsoft.Json;
using OcrApp;
using OcrApp.Models;
using PdfApp;
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
    public class GenerateDocument : BackgroundService
    {
        private readonly ILogger<GenerateDocument> _logger;
        private IConfiguration Configuration { get; }

        private Guid AppId { get; set; }

        public GenerateDocument(ILogger<GenerateDocument> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
            AppId = Guid.NewGuid();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(string.Concat("Function \"GenerateDocument\" started (Id: ", AppId, ")"));

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var db = new TheCompanyDbContext())
                {
                    bool stop = false;
                    while (!stop && !stoppingToken.IsCancellationRequested)
                    {
                        Invoice invoice = db.Invoices.Where(x => x.ShouldBeGenerated == true && x.IsGenerated == false && string.IsNullOrEmpty(x.LockedBy)).FirstOrDefault();
                        if (invoice != null)
                        {
                            _logger.LogInformation(string.Concat("Start processing invoice " + invoice.Id.ToString()));
                            invoice.LockedBy = string.Concat("GenerateDocument-", AppId);
                            db.SaveChanges();

                            PdfEngine pdfEngine = new PdfEngine();
                            if (!pdfEngine.CreateDocument())
                            {
                                throw new Exception("Couldn't generate pdf file");
                            }
                            if (!pdfEngine.InsertPage(0))
                            {
                                throw new Exception("Couldn't insert page in pdf file");
                            }
                            if (!pdfEngine.AddText("test", 20, 20))
                            {
                                throw new Exception("Couldn't add text in pdf file");
                            }

                            MemoryStream output = pdfEngine.ToStream();

                            Storage storage = new Storage(Configuration.GetSection("Storage"), invoice.Owner);
                            Guid fileId;
                            if (!storage.CreateFile(output, out fileId))
                            {
                                throw new Exception("Couldn't create file " + fileId.ToString());
                            }

                            invoice.FileId = fileId;
                            invoice.FileName = "Invoice" + invoice.InvoiceNumber + ".pdf";
                            invoice.FileSize = output.Length;
                            invoice.LockedBy = null;
                            invoice.IsGenerated = true;
                            invoice.GenerationDateTime = DateTime.Now;
                            db.SaveChanges();
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
            _logger.LogInformation(string.Concat("Function \"GenerateDocument\" ended (Id: ", AppId, ")"));
        }
    }
}