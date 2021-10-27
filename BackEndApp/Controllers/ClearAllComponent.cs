using Azure.Storage.Blobs;
using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelsApp;
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
				using (var db = new TheCompanyDbContext())
				{
					List<Individual> dbIndividuals = db.Customers_Individual.Where(x => x.Owner == owner).ToList();
					dbIndividuals.ForEach(individual =>
					{
						db.Remove(individual);
					});
					db.SaveChanges();
				}
			}
			if (json.GetProperty("invoices").GetBoolean())
			{
				using (var db = new TheCompanyDbContext())
				{
					List<Invoice> dbInvoices = db.Invoices.Where(x => x.Owner == owner).ToList();
					dbInvoices.ForEach(invoice =>
					{
						// Uncomment this code when true delete is impleted
						/*BlobContainerClient containerClient = new BlobContainerClient(Configuration.GetConnectionString("AzureStorageAccount"), owner.ToString());
						containerClient.DeleteBlob(invoice.FileId.ToString());
						if (invoice.ExtractId != Guid.Empty)
							containerClient.DeleteBlob(invoice.ExtractId.ToString());*/
						db.Remove(invoice);
					});
					db.SaveChanges();
				}
			}
			if (json.GetProperty("extractionSettings").GetBoolean())
			{
				using (var db = new TheCompanyDbContext())
				{
					List<ExtractionSettings> dbExtractionSettings = db.ExtractionSettings.Where(x => x.Owner == owner).ToList();
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
