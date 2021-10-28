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
					ExtractionSettings extSet = new ExtractionSettings("Invoice",
																	fieldName,
																	field.GetProperty("x").GetInt32(),
																	field.GetProperty("y").GetInt32(),
																	field.GetProperty("width").GetInt32(),
																	field.GetProperty("height").GetInt32(),
																	owner);

					ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoice" && x.Field == fieldName && x.Owner == owner).SingleOrDefault();
					if (dbExtSet != null)
					{
						dbExtSet.Deleted = true;
					}
					db.Add(extSet);
					db.SaveChanges();
				}
			}
			_logger.LogInformation("End of SaveExtractionSettings method");
			return Ok();
		}

		[HttpGet("GetExtractionSettings")]
		public ActionResult GetExtractionSettings()
		{
			_logger.LogInformation("Start of GetExtractionSettings method");
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			List<ExtractionSettings> results = new List<ExtractionSettings>();
			using (var db = new TheCompanyDbContext())
			{
				results = db.ExtractionSettings.Where(x => x.DataSource == "Invoices" && x.Owner == owner).ToList();
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
	}
}
