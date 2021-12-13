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

            bool debugMode = false;
#if DEBUG
            debugMode = true;
#endif

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var db = new TheCompanyDbContext(Guid.Empty))
                {
                    bool stop = false;
                    while (!stop && !stoppingToken.IsCancellationRequested)
                    {
                        Invoice dbInvoice = db.Invoices.Where(x => x.ShouldBeGenerated == true && x.IsGenerated == false && string.IsNullOrEmpty(x.LockedBy)).FirstOrDefault();
                        if (dbInvoice != null)
                        {
                            _logger.LogInformation(string.Concat("Start processing invoice " + dbInvoice.Id.ToString()));
                            db.SetOwner(dbInvoice.Owner);
                            dbInvoice.LockedBy = string.Concat("GenerateDocument-", AppId);
                            db.SaveChanges();

                            Storage storage = new Storage(Configuration.GetSection("Storage"), dbInvoice.Owner);

                            PdfEngine pdfEngine = new PdfEngine();
                            pdfEngine.CreateDocument();
                            pdfEngine.InsertPage(0);

                            int y = 20;

                            Company dbCompany = db.Companies.SingleOrDefault();
                            if (dbCompany != null)
                            {
                                MemoryStream logo;
                                if (!storage.GetFile(dbCompany.Logo, out logo))
                                {
                                    throw new Exception("Couldn't retrieve logo");
                                }
                                pdfEngine.InsertImage(logo, 20, y, 162, 127);
                            }

                            pdfEngine.AddText("Invoice N°: " + dbInvoice.InvoiceNumber, 325, y+20);
                            pdfEngine.AddText("Creation Date: " + dbInvoice.CreationDateTime.ToString("yyyy-MM-dd"), 325, y+40);

                            y = 200;
                            pdfEngine.AddText(dbCompany.Name, 20, y);
                            y+=20;
                            List<string> addressLines = dbCompany.Address.Split("\n").ToList();
                            addressLines.ForEach(line =>
                            {
                                pdfEngine.AddText(line, 20, y);
                                y+=20;
                            });
                            
                            pdfEngine.AddText("Phone: " + dbCompany.PhoneNumber, 20, y);
                            pdfEngine.AddText("Mobile: " + dbCompany.MobilePhoneNumber, 20, y+20);
                            pdfEngine.AddText("SIRET: " + dbCompany.Siret, 20, y+40);

                            if (dbInvoice.CustomerId != Guid.Empty)
                            {
                                // TODO: write customer info
                            }

                            y+=100;
                            pdfEngine.AddText("Reference", 20, y);
                            pdfEngine.AddText("Description", 100, y);
                            pdfEngine.AddText("Quantity", 350, y);
                            pdfEngine.AddText("UnitaryPrice", 420, y);
                            pdfEngine.AddText("Price", 510, y);

                            y+=30;
                            double total = 0;
                            List<LineItem> lineItems = db.LineItems.Where(x => x.InvoiceId == dbInvoice.Id).ToList();
                            lineItems.ForEach(item =>
                            {
                                pdfEngine.AddText(item.Reference, 20, y );
                                pdfEngine.AddText(item.Description, 100, y);
                                pdfEngine.AddText(string.Format("{0:N2}", item.Quantity), 350, y);
                                pdfEngine.AddText(string.Format("{0:N2}", item.UnitaryPrice), 420, y);
                                pdfEngine.AddText(string.Format("{0:N2}", item.Price), 510, y);
                                y+=20;
                                total += item.Price;
                            });

                            y+=100;
                            pdfEngine.AddText("Total: " + string.Format("{0:N2}", total) + " €", 420, y);

                            MemoryStream output = pdfEngine.ToStream();
                            Guid fileId;
                            if (!storage.CreateFile(output, out fileId))
                            {
                                throw new Exception("Couldn't create file " + fileId.ToString());
                            }

                            if (debugMode)
                            {
                                string executingAssembly = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                                FileStream file = new FileStream(Path.Combine(executingAssembly, "generated.pdf"), FileMode.OpenOrCreate);
                                output.WriteTo(file);
                                file.Close();
                            }

                            dbInvoice.FileId = fileId;
                            dbInvoice.FileName = "Invoice" + dbInvoice.InvoiceNumber + ".pdf";
                            dbInvoice.FileSize = output.Length;
                            dbInvoice.LockedBy = null;
                            dbInvoice.IsGenerated = true;
                            dbInvoice.GenerationDateTime = DateTime.Now;
                            db.SaveChanges();
                            db.SetOwner(Guid.Empty);
                            _logger.LogInformation(string.Concat("Invoice " + dbInvoice.Id.ToString() + " has been processed"));
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