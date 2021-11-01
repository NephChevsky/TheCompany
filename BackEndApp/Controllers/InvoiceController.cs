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
			JsonElement fields = json.GetProperty("fields");
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
						ExtractionSettings extSet = new ExtractionSettings("Invoice", name, x, y, width, height, owner);
						ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.Field == fieldName && x.Owner == owner).SingleOrDefault();
						if (dbExtSet != null)
						{
							dbExtSet.Deleted = true;
						}
						db.Add(extSet);
						db.SaveChanges();
					}
					else if (x == -1 && y == -1 && width == -1 && height == -1)
					{
						ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.Field == fieldName && x.Owner == owner).SingleOrDefault();
						if (dbExtSet != null)
						{
							dbExtSet.Deleted = true;
							db.SaveChanges();
						}
					}
				}
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
					MemoryStream stream = new MemoryStream();
					BlobContainerClient containerClient = new BlobContainerClient(Configuration.GetConnectionString("AzureStorageAccount"), owner.ToString());
					containerClient.GetBlobClient(dbInvoice.ExtractId.ToString()).DownloadTo(stream);
					_logger.LogInformation("End of Preview method");
					return Ok(Convert.ToBase64String(stream.ToArray()));
				}
			}
		}
	}
}
