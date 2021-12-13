using BackEndApp.DTO;
using DbApp.Models;
using MagickApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelsApp.DbModels;
using ModelsApp.Attributes;
using StorageApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using ModelsApp.Helpers;
using OcrApp;
using OcrApp.Models;
using System.Text.Json;

namespace BackEndApp.Controllers
{
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly ILogger<InvoiceController> _logger;
        private IConfiguration Configuration { get; }

        public InvoiceController(IConfiguration configuration, ILogger<InvoiceController> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        [HttpPost("Import")]
        public ActionResult Import([FromForm] IFormFile File)
        {
            _logger.LogInformation("Start of Import method");
            if (File.FileName.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0
                || File.FileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
            {
                return UnprocessableEntity("InvalidCharacters");
            }

            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            Storage storage = new Storage(Configuration.GetSection("Storage"), owner);

            using (var db = new TheCompanyDbContext(owner))
            {
                if (File.ContentType == "application/x-zip-compressed")
                {
                    using (var zip = new ZipArchive(File.OpenReadStream(), ZipArchiveMode.Read))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            using (Stream stream = entry.Open())
                            {
                                if (!CreateInvoice(db, storage, stream, entry.Name, entry.Length))
                                {
                                    return UnprocessableEntity();
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!CreateInvoice(db, storage, File.OpenReadStream(), File.FileName, File.Length))
                    {
                        return UnprocessableEntity();
                    }
                }
                db.SaveChanges();
            }
            _logger.LogInformation("End of Import method");
            return Ok();
        }

        private bool CreateInvoice(TheCompanyDbContext db, Storage storage, Stream stream, string fileName, long fileSize)
        {
            MemoryStream tmp = new MemoryStream();
            stream.CopyTo(tmp);
            Guid id;
            if (UploadFile(storage, tmp, out id))
            {
                Invoice newInvoice = new Invoice(id, fileName, fileSize);
                newInvoice.ShouldBeExtracted = true;
                db.Invoices.Add(newInvoice);
            }
            else
            {
                return false;
            }
            return true;
        }

        private bool UploadFile(Storage storage, MemoryStream stream, out Guid fileId)
        {
            fileId = Guid.Empty;
            if (!storage.CreateFile(stream, out fileId))
            {
                return false;
            }
            return true;
        }

        [HttpPost("SaveExtractionSettings")]
        public ActionResult SaveExtractionSettings([FromBody] InvoiceSaveExtractionSettingsQuery query)
        {
            _logger.LogInformation("Start of SaveExtractionSettings method");

            if (query == null)
            {
                return BadRequest();
            }

            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            using (var db = new TheCompanyDbContext(owner))
            {
                query.InvoiceSettings.Fields.ForEach(field =>
                {
                    if (field.X != -1 && field.Y != -1 && field.Width != -1 && field.Height != -1)
                    {
                        Guid id;
                        if (Guid.TryParse(field.Id, out id) && id != Guid.Empty)
                            field.Name = field.Id.ToString();
                        ExtractionSettings extSet = new ExtractionSettings("Invoice", false, field.Name, field.X, field.Y, field.Width, field.Height);
                        ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == false && x.Field == field.Name).SingleOrDefault();
                        if (dbExtSet != null)
                        {
                            db.ExtractionSettings.Remove(dbExtSet);
                        }
                        db.Add(extSet);
                    }
                    else if (field.X == -1 && field.Y == -1 && field.Width == -1 && field.Height == -1)
                    {
                        ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == false && x.Field == field.Name).SingleOrDefault();
                        if (dbExtSet != null)
                        {
                            db.ExtractionSettings.Remove(dbExtSet);
                        }
                    }
                });

                if (query.LineItemSettings.BoxYMin !=-1 && query.LineItemSettings.BoxYMax != -1)
                {
                    ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "LineItem").SingleOrDefault();
                    if (dbExtSet != null)
                    {
                        db.ExtractionSettings.Remove(dbExtSet);
                    }
                    ExtractionSettings extSet = new ExtractionSettings("Invoice", true, "LineItem", 0, query.LineItemSettings.BoxYMin, 0, query.LineItemSettings.BoxYMax - query.LineItemSettings.BoxYMin);
                    db.ExtractionSettings.Add(extSet);
                    if (query.LineItemSettings.ReferenceXMin != -1 && query.LineItemSettings.ReferenceXMax != -1)
                    {
                        ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Reference").SingleOrDefault();
                        if (dbFieldExtSet != null)
                        {
                            db.ExtractionSettings.Remove(dbFieldExtSet);
                        }
                        ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Reference", query.LineItemSettings.ReferenceXMin, 0, query.LineItemSettings.ReferenceXMax - query.LineItemSettings.ReferenceXMin, 0);
                        db.ExtractionSettings.Add(fieldExtSet);
                    }
                    if (query.LineItemSettings.DescriptionXMin != -1 && query.LineItemSettings.DescriptionXMax != -1)
                    {
                        ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Description").SingleOrDefault();
                        if (dbFieldExtSet != null)
                        {
                            db.ExtractionSettings.Remove(dbFieldExtSet);
                        }
                        ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Description", query.LineItemSettings.DescriptionXMin, 0, query.LineItemSettings.DescriptionXMax - query.LineItemSettings.DescriptionXMin, 0);
                        db.ExtractionSettings.Add(fieldExtSet);
                    }
                    if (query.LineItemSettings.QuantityXMin != -1 && query.LineItemSettings.QuantityXMax != -1)
                    {
                        ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Quantity").SingleOrDefault();
                        if (dbFieldExtSet != null)
                        {
                            db.ExtractionSettings.Remove(dbFieldExtSet);
                        }
                        ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Quantity", query.LineItemSettings.QuantityXMin, 0, query.LineItemSettings.QuantityXMax - query.LineItemSettings.QuantityXMin, 0);
                        db.ExtractionSettings.Add(fieldExtSet);
                    }
                    if (query.LineItemSettings.UnitaryPriceXMin != -1 && query.LineItemSettings.UnitaryPriceXMax != -1)
                    {
                        ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "UnitaryPrice").SingleOrDefault();
                        if (dbFieldExtSet != null)
                        {
                            db.ExtractionSettings.Remove(dbFieldExtSet);
                        }
                        ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "UnitaryPrice", query.LineItemSettings.UnitaryPriceXMin, 0, query.LineItemSettings.UnitaryPriceXMax - query.LineItemSettings.UnitaryPriceXMin, 0);
                        db.ExtractionSettings.Add(fieldExtSet);
                    }
                    if (query.LineItemSettings.PriceXMin != -1 && query.LineItemSettings.PriceXMax != -1)
                    {
                        ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Price").SingleOrDefault();
                        if (dbFieldExtSet != null)
                        {
                            db.ExtractionSettings.Remove(dbFieldExtSet);
                        }
                        ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Price", query.LineItemSettings.PriceXMin, 0, query.LineItemSettings.PriceXMax - query.LineItemSettings.PriceXMin, 0);
                        db.ExtractionSettings.Add(fieldExtSet);
                    }
                }

                db.SaveChanges();
            }
            _logger.LogInformation("End of SaveExtractionSettings method");
            return Ok();
        }

        [HttpPost("GetExtractionSettings")]
        public ActionResult GetExtractionSettings([FromBody] List<string> query)
        {
            _logger.LogInformation("Start of GetExtractionSettings method");
            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (query.Count == 0)
            {
                query.AddRange(AttributeHelper.GetAuthorizedPropertiesAsString<Extractable>(typeof(Invoice)));
                query.Add("LineItem");
                query.AddRange(AttributeHelper.GetAuthorizedPropertiesAsString<Extractable>(typeof(LineItem)));
            }

            List<ExtractionSettings> results = new List<ExtractionSettings>();
            using (var db = new TheCompanyDbContext(owner))
            {
                results = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && query.Contains(x.Field)).ToList();
            }
            List<InvoiceGetExtractionSettingsResponse> returnValues = new List<InvoiceGetExtractionSettingsResponse>();
            bool lineItem = false;
            query.ForEach(x =>
            {
                if (x == "LineItem")
                    lineItem = true;
                ExtractionSettings dbField = results.Find(y => y.Field == x);
                if (dbField != null)
                {
                    returnValues.Add((InvoiceGetExtractionSettingsResponse)dbField);
                }
                else
                {
                    InvoiceGetExtractionSettingsResponse item = new InvoiceGetExtractionSettingsResponse();
                    item.Field = x;
                    item.IsLineItem = lineItem;
                    returnValues.Add(item);
                }
            });
            _logger.LogInformation("End of GetExtractionSettings method");
            return Ok(returnValues);
        }

        [HttpGet("Show/{id}")]
        public ActionResult Show(string id)
        {
            _logger.LogInformation("Start of Show method");
            if (id == null || id == "null")
            {
                Invoice invoice = new Invoice();
                return Ok((InvoiceShowResponse<Editable>)invoice);
            }
            else
            {
                Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                using (var db = new TheCompanyDbContext(owner))
                {
                    Invoice dbInvoice = db.Invoices.Where(x => x.Id.ToString() == id).SingleOrDefault();
                    if (dbInvoice == null)
                    {
                        return UnprocessableEntity("NotFound");
                    }
                    else
                    {
                        _logger.LogInformation("End of Show method");
                        return Ok((InvoiceShowResponse<Viewable>)dbInvoice);
                    }
                }
            }
        }

        [HttpGet("GetPreview/{id}/{page}")]
        public ActionResult GetPreview(string id, int page)
        {
            _logger.LogInformation("Start of Preview method");
            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            using (var db = new TheCompanyDbContext(owner))
            {
                
                Invoice dbInvoice = db.Invoices.Where(x => x.Id.ToString() == id).SingleOrDefault();
                if (dbInvoice == null)
                {
                    return UnprocessableEntity("NotFound");
                }
                else
                {
                    if (dbInvoice.FileId == Guid.Empty)
                    {
                        return Ok();
                    }
                    else
                    {
                        Storage storage = new Storage(Configuration.GetSection("Storage"), owner);
                        MemoryStream output;

                        FilePreview filePreview = db.FilePreviews.Where(x => x.FileId == dbInvoice.FileId && x.Page == page).SingleOrDefault();
                        if (filePreview == null)
                        {
                            MemoryStream stream;
                            if (!storage.GetFile(dbInvoice.FileId, out stream))
                            {
                                return UnprocessableEntity("NotFound");
                            }
                            MagickEngine magickEngine = new MagickEngine();
                            output = magickEngine.ConvertToPng(stream, page);
                            Guid previewId;
                            if (storage.CreateFile(output, out previewId))
                            {
                                filePreview = new FilePreview();
                                filePreview.Id = previewId;
                                filePreview.FileId = dbInvoice.FileId;
                                filePreview.Page = page;
                                db.FilePreviews.Add(filePreview);
                                db.SaveChanges();
                            }
                            else
                            {
                                throw new Exception("Couldn't create preview");
                            }
                        }
                        else
                        {
                            if (!storage.GetFile(filePreview.Id, out output))
                            {
                                return NotFound();
                            }
                        }

                        _logger.LogInformation("End of Preview method");
                        return Ok(Convert.ToBase64String(output.ToArray()));
                    }
                }
            }
        }

        [HttpPost("GetPreviewOnTheFly")]
        public ActionResult GetPreviewOnTheFly([FromForm] IFormFile File)
        {
            _logger.LogInformation("Start of GetPreviewOnTheFly method");

            if (File == null)
            {
                return BadRequest();
            }

            InvoiceGetSampleExtractionResponse result = new InvoiceGetSampleExtractionResponse();

            MemoryStream stream = new MemoryStream();
            File.OpenReadStream().CopyTo(stream);

            MagickEngine magickEngine = new MagickEngine();
            MemoryStream preview = magickEngine.ConvertToPng(stream, 1);

            result.Preview = Convert.ToBase64String(preview.ToArray());

            string fileExtension = File.FileName.Split(".").Last();
            string tempFileName = Path.GetTempFileName().Replace(".tmp", string.Concat(".", fileExtension));
            using (var fs = new FileStream(tempFileName, FileMode.Create))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fs);
            }
            Ocr ocr = new Ocr(Configuration.GetSection("Ocr"));
            if (!ocr.ExtractPDF(tempFileName))
            {
                throw new Exception("Couldn't extract PDF file");
            }
            ;

            result.Extraction = Convert.ToBase64String(Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ocr.GetExtractedBlocks(true))));

            _logger.LogInformation("End of GetPreviewOnTheFly method");
            return Ok(result);
        }

        [HttpGet("Extraction/{id}")]
        public ActionResult Extraction(string id)
        {
            _logger.LogInformation("Start of Extraction method");
            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            using (var db = new TheCompanyDbContext(owner))
            {
                Invoice dbInvoice = db.Invoices.Where(x => x.Id.ToString() == id).SingleOrDefault();
                if (dbInvoice == null)
                {
                    return UnprocessableEntity("NotFound");
                }
                else
                {
                    if (dbInvoice.ExtractId != Guid.Empty)
                    {
                        MemoryStream stream;
                        Storage storage = new Storage(Configuration.GetSection("Storage"), owner);
                        if (!storage.GetFile(dbInvoice.ExtractId, out stream))
                        {
                            _logger.LogInformation("End of Extraction method");
                            return NotFound();
                        }

                        _logger.LogInformation("End of Extraction method");
                        return Ok(Convert.ToBase64String(stream.ToArray()));
                    }
                    else
                    {
                        string result = "[]";
                        _logger.LogInformation("End of Extraction method");
                        return Ok(Convert.ToBase64String(Encoding.ASCII.GetBytes(result)));
                    }
                }
            }
        }

        [HttpPost("SaveInvoice")]
        public ActionResult SaveInvoice([FromBody] InvoiceSaveQuery query)
        {
            _logger.LogInformation("Start of SaveInvoice method");
            if (query == null || query.Fields == null || query.LineItems == null)
            {
                return BadRequest();
            }

            Guid retValue;
            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            using (var db = new TheCompanyDbContext(owner))
            {
                Invoice invoice;
                if (query.Id == null || query.Id == "null")
                {
                    invoice = new Invoice();
                    invoice.Id = Guid.NewGuid();
                    invoice.ShouldBeGenerated = true;
                }
                else
                {
                    Guid invoiceId = Guid.Parse(query.Id);
                    invoice = db.Invoices.Where(x => x.Id == invoiceId).SingleOrDefault();
                }

                Type type = typeof(Invoice);
                bool notEditableField = false;
                query.Fields.ForEach(x =>
                {
                    PropertyInfo property = type.GetProperty(x.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (AttributeHelper.CheckAttribute<Editable>(type, property))
                    {
                        if (property.PropertyType == typeof(DateTime))
                        {
                            property.SetValue(invoice, DateTime.Parse(x.Value));
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(invoice, x.Value);
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            property.SetValue(invoice, Double.Parse(x.Value));
                        }
                        else if (property.PropertyType == typeof(Guid))
                        {
                            property.SetValue(invoice, Guid.Parse(x.Value));
                        }
                        else
                        {
                            throw new Exception("Unknow property type");
                        }
                    }
                    else
                    {
                        notEditableField = true;
                    }
                });
                if (notEditableField)
                    return BadRequest();

                query.LineItems.ForEach(lineItem =>
                {
                    Guid lineItemId;
                    LineItem currentItem;
                    if (Guid.TryParse(lineItem.Id, out lineItemId) && lineItemId != Guid.Empty)
                    {
                        currentItem = db.LineItems.Where(x => x.InvoiceId == invoice.Id && x.Id == lineItemId).SingleOrDefault();
                    }
                    else
                    {
                        currentItem = new LineItem();
                        currentItem.CreationDateTime = DateTime.Now;
                        currentItem.InvoiceId = invoice.Id;
                    }
                    Type type = typeof(LineItem);
                    lineItem.Fields.ForEach(field =>
                    {
                        PropertyInfo property = type.GetProperty(field.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (AttributeHelper.CheckAttribute<Editable>(type, property))
                        {
                            if (property.PropertyType == typeof(DateTime))
                            {
                                property.SetValue(currentItem, DateTime.Parse(field.Value));
                            }
                            else if (property.PropertyType == typeof(string))
                            {
                                property.SetValue(currentItem, field.Value);
                            }
                            else if (property.PropertyType == typeof(double))
                            {
                                property.SetValue(currentItem, Double.Parse(field.Value));
                            }
                            else if (property.PropertyType == typeof(Guid))
                            {
                                property.SetValue(currentItem, Guid.Parse(field.Value));
                            }
                            else
                            {
                                throw new Exception("Unknow property type");
                            }
                        }
                        else
                        {
                            notEditableField = true;
                        }
                    });

                    if (lineItemId == Guid.Empty)
                    {
                        db.LineItems.Add(currentItem);
                    }
                });
                if (notEditableField)
                    return BadRequest();

                if (query.Id == null || query.Id == "null")
                {
                    db.Invoices.Add(invoice);
                }

                retValue = invoice.Id;

                db.SaveChanges();
            }

            _logger.LogInformation("End of SaveInvoice method");
            return Ok(retValue);
        }
    }
}
