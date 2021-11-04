using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DbApp.Models;
using MagickApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelsApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
			BlobContainerClient containerClient = new BlobContainerClient(Configuration.GetConnectionString("AzureStorageAccount"), owner.ToString());
			containerClient.CreateIfNotExists(); // TODO implement as singleton

			using (var db = new TheCompanyDbContext())
			{
				if (File.ContentType == "application/x-zip-compressed")
				{
					using (var zip = new ZipArchive(File.OpenReadStream(), ZipArchiveMode.Read))
					{
						foreach (var entry in zip.Entries)
						{
							using (Stream stream = entry.Open())
							{
								UploadFile(db, containerClient, stream, owner, entry.Name, entry.Length);
							}
						}
					}
				}
				else
				{
					UploadFile(db, containerClient, File.OpenReadStream(), owner, File.FileName, File.Length);
				}

				
			}
			_logger.LogInformation("End of Import method");
			return Ok();
		}

		private ActionResult UploadFile(TheCompanyDbContext db, BlobContainerClient containerClient, Stream stream, Guid owner, string fileName, long fileSize)
		{
			Guid id = Guid.NewGuid();
			containerClient.UploadBlob(id.ToString(), stream);

			Invoice newInvoice = new Invoice(owner, id, fileName, fileSize);
			db.Invoices.Add(newInvoice);
			try
			{
				db.SaveChanges();
			}
			catch (Exception e)
			{
				containerClient.DeleteBlob(id.ToString());
				if (e.GetType().IsAssignableFrom(typeof(DbUpdateException)) && ((e.InnerException as SqlException)?.Number == 2601 || (e.InnerException as SqlException)?.Number == 2627))
				{
					return Conflict("AlreadyExists");
				}
				else
				{
					throw e;
				}
			}

			return Ok();
		}

		[HttpPost("SaveExtractionSettings")]
		public ActionResult SaveExtractionSettings([FromBody] dynamic json)
		{
			_logger.LogInformation("Start of SaveExtractionSettings method");
			JsonElement invoiceSettings = json.GetProperty("invoiceSettings");
			JsonElement lineItemSettings = json.GetProperty("lineItemSettings");
			JsonElement fields = invoiceSettings.GetProperty("fields");
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			using (var db = new TheCompanyDbContext())
			{
				foreach (JsonElement field in fields.EnumerateArray())
				{
					string fieldName = field.GetProperty("name").GetString();
					int x = ExtractPositon(field.GetProperty("x"));
					int y = ExtractPositon(field.GetProperty("y"));
					int width = ExtractPositon(field.GetProperty("width"));
					int height = ExtractPositon(field.GetProperty("height"));

					if (x != -1 && y != -1 && width != -1 && height != -1)
					{
						string name = field.GetProperty("id").GetString();
						if (string.IsNullOrEmpty(name))
							name = fieldName;
						ExtractionSettings extSet = new ExtractionSettings("Invoice", false, name, x, y, width, height, owner);
						ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == false && x.Field == fieldName && x.Owner == owner).SingleOrDefault();
						if (dbExtSet != null)
						{
							dbExtSet.Deleted = true;
						}
						db.Add(extSet);
					}
					else if (x == -1 && y == -1 && width == -1 && height == -1)
					{
						ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == false && x.Field == fieldName && x.Owner == owner).SingleOrDefault();
						if (dbExtSet != null)
						{
							dbExtSet.Deleted = true;
						}
					}
				}

				int boxYmin = ExtractPositon(lineItemSettings.GetProperty("boxymin"));
				int boxYmax = ExtractPositon(lineItemSettings.GetProperty("boxymax"));
				int referenceXmin = ExtractPositon(lineItemSettings.GetProperty("referencexmin"));
				int referenceXmax = ExtractPositon(lineItemSettings.GetProperty("referencexmax"));
				int descriptionXmin = ExtractPositon(lineItemSettings.GetProperty("descriptionxmin"));
				int descriptionXmax = ExtractPositon(lineItemSettings.GetProperty("descriptionxmax"));
				int quantityXmin = ExtractPositon(lineItemSettings.GetProperty("quantityxmin"));
				int quantityXmax = ExtractPositon(lineItemSettings.GetProperty("quantityxmax"));
				int unitarypriceXmin = ExtractPositon(lineItemSettings.GetProperty("unitarypricexmin"));
				int unitarypriceXmax = ExtractPositon(lineItemSettings.GetProperty("unitarypricexmax"));
				int priceXmin = ExtractPositon(lineItemSettings.GetProperty("pricexmin"));
				int priceXmax = ExtractPositon(lineItemSettings.GetProperty("pricexmax"));

				if (boxYmin !=-1 && boxYmax != -1)
				{
					ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "LineItem" && x.Owner == owner).SingleOrDefault();
					if (dbExtSet != null)
					{
						dbExtSet.Deleted = true;
					}
					ExtractionSettings extSet = new ExtractionSettings("Invoice", true, "LineItem", 0, boxYmin, 0, boxYmax-boxYmin, owner);
					db.ExtractionSettings.Add(extSet);
					if (referenceXmin != -1 && referenceXmax != -1)
					{
						ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Reference" && x.Owner == owner).SingleOrDefault();
						if (dbFieldExtSet != null)
						{
							dbFieldExtSet.Deleted = true;
						}
						ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Reference", referenceXmin, 0, referenceXmax - referenceXmin, 0, owner);
						db.ExtractionSettings.Add(fieldExtSet);
					}
					if (descriptionXmin != -1 && descriptionXmax != -1)
					{
						ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Description" && x.Owner == owner).SingleOrDefault();
						if (dbFieldExtSet != null)
						{
							dbFieldExtSet.Deleted = true;
						}
						ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Description", descriptionXmin, 0, descriptionXmax - descriptionXmin, 0, owner);
						db.ExtractionSettings.Add(fieldExtSet);
					}
					if (quantityXmin != -1 && quantityXmax != -1)
					{
						ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Quantity" && x.Owner == owner).SingleOrDefault();
						if (dbFieldExtSet != null)
						{
							dbFieldExtSet.Deleted = true;
						}
						ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Quantity", quantityXmin, 0, quantityXmax - quantityXmin, 0, owner);
						db.ExtractionSettings.Add(fieldExtSet);
					}
					if (unitarypriceXmin != -1 && unitarypriceXmax != -1)
					{
						ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "UnitaryPrice" && x.Owner == owner).SingleOrDefault();
						if (dbFieldExtSet != null)
						{
							dbFieldExtSet.Deleted = true;
						}
						ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "UnitaryPrice", unitarypriceXmin, 0, unitarypriceXmax - unitarypriceXmin, 0, owner);
						db.ExtractionSettings.Add(fieldExtSet);
					}
					if (priceXmin != -1 && priceXmax != -1)
					{
						ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Price" && x.Owner == owner).SingleOrDefault();
						if (dbFieldExtSet != null)
						{
							dbFieldExtSet.Deleted = true;
						}
						ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Price", priceXmin, 0, priceXmax - priceXmin, 0, owner);
						db.ExtractionSettings.Add(fieldExtSet);
					}
				}

				db.SaveChanges();
			}
			_logger.LogInformation("End of SaveExtractionSettings method");
			return Ok();
		}

		private int ExtractPositon(JsonElement elem)
		{
			int result = -1;
			if (elem.ValueKind == JsonValueKind.Number)
				result = elem.GetInt32();
			else if (elem.ValueKind == JsonValueKind.String)
			{
				string tmp = elem.GetString();
				if (!string.IsNullOrEmpty(tmp))
				{
					result = Convert.ToInt32(tmp);
				}
			}
			return result;
		}

		[HttpPost("GetExtractionSettings")]
		public ActionResult GetExtractionSettings([FromBody] List<string> ids)
		{
			_logger.LogInformation("Start of GetExtractionSettings method");
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			List<ExtractionSettings> results = new List<ExtractionSettings>();
			using (var db = new TheCompanyDbContext())
			{
				results = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.Owner == owner && ids.Contains(x.Field)).ToList();
			}
			_logger.LogInformation("End of GetExtractionSettings method");
			return Ok(results);
		}

		[HttpGet("Show/{id}")]
		public ActionResult Show(string id)
		{
			_logger.LogInformation("Start of Show method");
			using (var db = new TheCompanyDbContext())
			{
				Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				Invoice dbInvoice = db.Invoices.Where(x => x.Id.ToString() == id && x.Owner == owner).SingleOrDefault();
				if (dbInvoice == null)
				{
					return UnprocessableEntity("NotFound");
				}
				else
				{
					_logger.LogInformation("End of Show method");
					return Ok(dbInvoice);
				}
			}
		}

		[HttpGet("Preview/{id}/{page}")]
		public ActionResult Preview(string id, int page)
		{
			_logger.LogInformation("Start of Preview method");
			using (var db = new TheCompanyDbContext())
			{
				Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				Invoice dbInvoice = db.Invoices.Where(x => x.Id.ToString() == id && x.Owner == owner).SingleOrDefault();
				if (dbInvoice == null)
				{
					return UnprocessableEntity("NotFound");
				}
				else
				{
					// TODO: store preview in Azure Storage for faster load the next time
					MemoryStream stream = new MemoryStream();
					BlobContainerClient containerClient = new BlobContainerClient(Configuration.GetConnectionString("AzureStorageAccount"), owner.ToString());
					containerClient.GetBlobClient(dbInvoice.FileId.ToString()).DownloadTo(stream);
					MagickEngine magickEngine = new MagickEngine();
					byte[] output = magickEngine.ConvertToPng(stream, page);
					_logger.LogInformation("End of Preview method");
					return Ok(Convert.ToBase64String(output));
				}
			}
		}

		[HttpGet("Extraction/{id}")]
		public ActionResult Extraction(string id)
		{
			_logger.LogInformation("Start of Extraction method");
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			using (var db = new TheCompanyDbContext())
			{
				Invoice dbInvoice = db.Invoices.Where(x => x.Id.ToString() == id && x.Owner == owner).SingleOrDefault();
				if (dbInvoice == null)
				{
					return UnprocessableEntity("NotFound");
				}
				else
				{
					if (dbInvoice.ExtractId != Guid.Empty)
					{
						MemoryStream stream = new MemoryStream();
						BlobContainerClient containerClient = new BlobContainerClient(Configuration.GetConnectionString("AzureStorageAccount"), owner.ToString());
						try
						{
							containerClient.GetBlobClient(dbInvoice.ExtractId.ToString()).DownloadTo(stream);
						}
						catch
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
		public ActionResult SaveInvoice([FromBody] JsonElement json) // TODO: implement DTO
		{
			_logger.LogInformation("Start of SaveInvoice method");
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			using (var db = new TheCompanyDbContext())
			{
				JsonElement data = json.GetProperty("data");
				Guid invoiceId = Guid.Parse(data.GetProperty("id").GetString());
				Invoice invoice = db.Invoices.Where(x => x.Owner == owner && x.Id == invoiceId).SingleOrDefault();
				invoice.InvoiceNumber = data.GetProperty("invoiceNumber").GetString();
				invoice.CustomerNumber = data.GetProperty("customerNumber").GetString();
				invoice.CustomerAddress = data.GetProperty("customerAddress").GetString();
				JsonElement addedLineItems = json.GetProperty("addedLineItems").GetProperty("lines");
				foreach (JsonElement line in addedLineItems.EnumerateArray())
				{
					string reference = line.GetProperty("reference").GetString();
					string description = line.GetProperty("description").GetString();
					double quantity = Convert.ToDouble(line.GetProperty("quantity").GetString());
					double unitaryprice = Convert.ToDouble(line.GetProperty("unitaryprice").GetString());
					double price = Convert.ToDouble(line.GetProperty("price").GetString());
					InvoiceLineItem newItem = new InvoiceLineItem(invoice.Id, owner, reference, description, quantity, unitaryprice, price);
					db.InvoiceLineItems.Add(newItem);
				}
				db.SaveChanges();
			}
			
			_logger.LogInformation("End of SaveInvoice method");
			return Ok();
		}
	}
}
