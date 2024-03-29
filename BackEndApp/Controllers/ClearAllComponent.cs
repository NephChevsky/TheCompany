﻿using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelsApp.DbModels;
using StorageApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace BackEndApp.Controllers
{
	public class ClearAllController : Controller
	{
		private readonly ILogger<ClearAllController> _logger;
		private IConfiguration Configuration { get; }

		public ClearAllController(ILogger<ClearAllController> logger, IConfiguration configuration)
		{
			_logger = logger;
			Configuration = configuration;
		}

		[HttpPost]
		public ActionResult Clear([FromBody] JsonElement json)
		{
			_logger.LogInformation("Start of ClearAll method");
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			if (json.GetProperty("customers").GetBoolean())
			{
				using (var db = new TheCompanyDbContext(owner))
				{
					List<Individual> dbIndividuals = db.Individuals.ToList();
					dbIndividuals.ForEach(individual =>
					{
						db.Remove(individual);
					});
					db.SaveChanges();
				}
			}
			if (json.GetProperty("invoices").GetBoolean())
			{
				using (var db = new TheCompanyDbContext(owner))
				{
					List<Invoice> dbInvoices = db.Invoices.ToList();
					dbInvoices.ForEach(invoice =>
					{
						// Uncomment this code when true delete is impleted
						/*Storage storage = new Storage(Configuration.GetSection("Storage"), owner);
						if (!storage.DeleteFile(invoice.FileId.ToString()))
						{
							_logger.LogError("Couldn't delete file " + invoice.FileId.ToString() + " during clear all");
						}
						if (invoice.ExtractId != Guid.Empty)
							storage.DeleteFile(invoice.ExtractId.ToString());*/
						db.Remove(invoice);
					});
					db.SaveChanges();
				}
			}
			if (json.GetProperty("extractionSettings").GetBoolean())
			{
				using (var db = new TheCompanyDbContext(owner))
				{
					List<ExtractionSettings> dbExtractionSettings = db.ExtractionSettings.ToList();
					dbExtractionSettings.ForEach(extractionSettings =>
					{
						db.Remove(extractionSettings);
					});
					db.SaveChanges();
				}
			}
			_logger.LogInformation("End of ClearAll method");
			return Ok();
		}
	}
}
