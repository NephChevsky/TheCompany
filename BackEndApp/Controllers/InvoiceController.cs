﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DbApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace BackEndApp.Controllers
{
	public class InvoiceController : Controller
	{
		private IConfiguration Configuration { get; }

		public InvoiceController(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		[HttpPost]
		public ActionResult Import([FromForm] IFormFile File)
		{
			if (File.FileName.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0
				|| File.FileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
			{
				return UnprocessableEntity("InvalidCharacters");
			}

			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

			using (var db = new TheCompanyDbContext())
			{
				BlobContainerClient containerClient = new BlobContainerClient(Configuration.GetConnectionString("AzureStorageAccount"), owner.ToString());
				containerClient.CreateIfNotExists(); // TODO implement as singleton

				Guid id = Guid.NewGuid();
				using (System.IO.Stream input = File.OpenReadStream())
				{
					containerClient.UploadBlob(id.ToString(), input);
				}

				Invoice newInvoice = new Invoice(owner, id, File.FileName, File.Length);
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
		}

		[HttpPost]
		public ActionResult SaveExtractionSettings([FromBody] dynamic json)
		{
			JsonElement invoiceNumber = json.GetProperty("invoiceNumber");
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			ExtractionSettings extSet = new ExtractionSettings("Invoices", "InvoiceNumber", Convert.ToInt32(invoiceNumber.GetProperty("x").GetString()),
																Convert.ToInt32(invoiceNumber.GetProperty("y").GetString()),
																Convert.ToInt32(invoiceNumber.GetProperty("width").GetString()),
																Convert.ToInt32(invoiceNumber.GetProperty("height").GetString()),
																owner);
			using (var db = new TheCompanyDbContext())
			{
				ExtractionSettings dbExtSet = db.ExtractionSettings.Where(x => x.DataSource == "Invoices" && x.Field == "InvoiceNumber" && x.Owner == owner).SingleOrDefault();
				if (dbExtSet == null)
				{
					db.Add(extSet);
					db.SaveChanges();
				}
				else
				{
					dbExtSet.Deleted = true;
					db.Add(extSet);
					db.SaveChanges();
				}
			}
			return Ok();
		}

		[HttpGet]
		public ActionResult GetExtractionSettings()
		{
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			List<ExtractionSettings> results = new List<ExtractionSettings>();
			using (var db = new TheCompanyDbContext())
			{
				results = db.ExtractionSettings.Where(x => x.DataSource == "Invoices" && x.Owner == owner).ToList();
			}
			return Ok(results);
		}
	}
}
