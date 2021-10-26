using DbApp.Models;
using BackEndApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ModelsApp;
using Microsoft.Extensions.Logging;

namespace BackEndApp.Controllers
{
	public class ViewListController : Controller
	{
		private readonly ILogger<ViewListController> _logger;

		public ViewListController(ILogger<ViewListController> logger)
		{
			_logger = logger;
		}

		[HttpPost]
		public ActionResult Get([FromBody] ViewListQuery query)
		{
			_logger.LogInformation("Start of Get method");
			if (query == null)
				return BadRequest();

			switch (query.DataSource)
			{
				case "Individual":
					return Ok(GetIndividuals());
				case "Invoices":
					return Ok(GetInvoices());
				default:
					return BadRequest();
			}
			_logger.LogInformation("End of Get method");
		}

		private List<Dictionary<string, string>> GetIndividuals()
		{
			_logger.LogInformation("Start of GetIndividuals method");
			List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
			using (var db = new TheCompanyDbContext())
			{
				Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				List<Individual> individuals = db.Customers_Individual.Where(obj => obj.Owner == owner)
																	.OrderBy(obj => obj.CustomerId)
																	.ToList();
				Dictionary<string, string> header = new Dictionary<string, string>();
				header.Add("0", "CustomerId");
				header.Add("1", "LastName");
				header.Add("2", "FirstName");
				header.Add("3", "Address");
				values.Add(header);
				individuals.ForEach(individual =>
				{
					Dictionary<string, string> value = new Dictionary<string, string>();
					value.Add("0", individual.CustomerId);
					value.Add("1", individual.LastName);
					value.Add("2", individual.FirstName);
					value.Add("3", individual.Address);
					values.Add(value);
				});
			}
			_logger.LogInformation("End of GetIndividuals method");
			return values;
		}

		private List<Dictionary<string, string>> GetInvoices()
		{
			_logger.LogInformation("Start of GetInvoices method");
			List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
			using (var db = new TheCompanyDbContext())
			{
				Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				List<Invoice> invoices = db.Invoices.Where(obj => obj.Owner == owner)
																	.OrderBy(obj => obj.InvoiceNumber)
																	.ToList();
				Dictionary<string, string> header = new Dictionary<string, string>();
				header.Add("0", "Id");
				header.Add("1", "InvoiceNumber");
				header.Add("2", "CustomerId");
				values.Add(header);
				invoices.ForEach(invoice =>
				{
					Dictionary<string, string> value = new Dictionary<string, string>();
					value.Add("0", invoice.Id.ToString());
					value.Add("1", invoice.InvoiceNumber);
					value.Add("2", invoice.CustomerId.ToString());
					values.Add(value);
				});
			}
			_logger.LogInformation("End of GetInvoices method");
			return values;
		}
	}
}
