using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BackEndApp.DTO;
using DbApp.Models;
using MagickApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using ModelsApp;
using ModelsApp.DbModels;
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
		public ActionResult SaveExtractionSettings([FromBody] InvoiceSaveExtractionSettingsQuery query)
		{
			_logger.LogInformation("Start of SaveExtractionSettings method");

			if (query == null)
			{
				return BadRequest();
			}

			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			using (var db = new TheCompanyDbContext())
			{
				query.InvoiceSettings.Fields.ForEach(field => {
					if (field.X != -1 && field.Y != -1 && field.Width != -1 && field.Height != -1)
					{
						Guid id;
						if (Guid.TryParse(field.Id, out id) && id != Guid.Empty)
							field.Name = field.Id.ToString();
						ExtractionSettings extSet = new ExtractionSettings("Invoice", false, field.Name, field.X, field.Y, field.Width, field.Height, owner);
						ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == false && x.Field == field.Name && x.Owner == owner).SingleOrDefault();
						if (dbExtSet != null)
						{
							db.ExtractionSettings.Remove(dbExtSet);
						}
						db.Add(extSet);
					}
					else if (field.X == -1 && field.Y == -1 && field.Width == -1 && field.Height == -1)
					{
						ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == false && x.Field == field.Name && x.Owner == owner).SingleOrDefault();
						if (dbExtSet != null)
						{
							db.ExtractionSettings.Remove(dbExtSet);
						}
					}
				});

				if (query.LineItemSettings.BoxYMin !=-1 && query.LineItemSettings.BoxYMax != -1)
				{
					ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "LineItem" && x.Owner == owner).SingleOrDefault();
					if (dbExtSet != null)
					{
						db.ExtractionSettings.Remove(dbExtSet);
					}
					ExtractionSettings extSet = new ExtractionSettings("Invoice", true, "LineItem", 0, query.LineItemSettings.BoxYMin, 0, query.LineItemSettings.BoxYMax - query.LineItemSettings.BoxYMin, owner);
					db.ExtractionSettings.Add(extSet);
					if (query.LineItemSettings.ReferenceXMin != -1 && query.LineItemSettings.ReferenceXMax != -1)
					{
						ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Reference" && x.Owner == owner).SingleOrDefault();
						if (dbFieldExtSet != null)
						{
							db.ExtractionSettings.Remove(dbFieldExtSet);
						}
						ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Reference", query.LineItemSettings.ReferenceXMin, 0, query.LineItemSettings.ReferenceXMax - query.LineItemSettings.ReferenceXMin, 0, owner);
						db.ExtractionSettings.Add(fieldExtSet);
					}
					if (query.LineItemSettings.DescriptionXMin != -1 && query.LineItemSettings.DescriptionXMax != -1)
					{
						ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Description" && x.Owner == owner).SingleOrDefault();
						if (dbFieldExtSet != null)
						{
							db.ExtractionSettings.Remove(dbFieldExtSet);
						}
						ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Description", query.LineItemSettings.DescriptionXMin, 0, query.LineItemSettings.DescriptionXMax - query.LineItemSettings.DescriptionXMin, 0, owner);
						db.ExtractionSettings.Add(fieldExtSet);
					}
					if (query.LineItemSettings.QuantityXMin != -1 && query.LineItemSettings.QuantityXMax != -1)
					{
						ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Quantity" && x.Owner == owner).SingleOrDefault();
						if (dbFieldExtSet != null)
						{
							db.ExtractionSettings.Remove(dbFieldExtSet);
						}
						ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Quantity", query.LineItemSettings.QuantityXMin, 0, query.LineItemSettings.QuantityXMax - query.LineItemSettings.QuantityXMin, 0, owner);
						db.ExtractionSettings.Add(fieldExtSet);
					}
					if (query.LineItemSettings.UnitaryPriceXMin != -1 && query.LineItemSettings.UnitaryPriceXMax != -1)
					{
						ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "UnitaryPrice" && x.Owner == owner).SingleOrDefault();
						if (dbFieldExtSet != null)
						{
							db.ExtractionSettings.Remove(dbFieldExtSet);
						}
						ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "UnitaryPrice", query.LineItemSettings.UnitaryPriceXMin, 0, query.LineItemSettings.UnitaryPriceXMax - query.LineItemSettings.UnitaryPriceXMin, 0, owner);
						db.ExtractionSettings.Add(fieldExtSet);
					}
					if (query.LineItemSettings.PriceXMin != -1 && query.LineItemSettings.PriceXMax != -1)
					{
						ExtractionSettings dbFieldExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.IsLineItem == true && x.Field == "Price" && x.Owner == owner).SingleOrDefault();
						if (dbFieldExtSet != null)
						{
							db.ExtractionSettings.Remove(dbFieldExtSet);
						}
						ExtractionSettings fieldExtSet = new ExtractionSettings("Invoice", true, "Price", query.LineItemSettings.PriceXMin, 0, query.LineItemSettings.PriceXMax - query.LineItemSettings.PriceXMin, 0, owner);
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
			List<ExtractionSettings> results = new List<ExtractionSettings>();
			using (var db = new TheCompanyDbContext())
			{
				results = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.Owner == owner && query.Contains(x.Field)).ToList();
			}
			List<InvoiceGetExtractionSettingsResponse> returnValues = new List<InvoiceGetExtractionSettingsResponse>();
			results.ForEach(x =>
			{
				returnValues.Add((InvoiceGetExtractionSettingsResponse) x);
			});
			_logger.LogInformation("End of GetExtractionSettings method");
			return Ok(returnValues);
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
					return Ok((InvoiceShowResponse) dbInvoice);
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
						catch (StorageException e) when (e.RequestInformation.ErrorCode == "BlobNotFound")
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
