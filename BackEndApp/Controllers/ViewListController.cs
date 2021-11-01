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
			{
				_logger.LogInformation("End of Get method");
				return BadRequest();
			}

			switch (query.DataSource)
			{
				case "Individual":
					_logger.LogInformation("End of Get method");
					return Ok(GetIndividuals(query));
				case "Invoice":
					_logger.LogInformation("End of Get method");
					return Ok(GetInvoices(query));
				case "AdditionalField":
					_logger.LogInformation("End of Get method");
					return Ok(GetAdditionalFields(query));
				case "InvoiceLineItem":
					_logger.LogInformation("End of Get method");
					return Ok(GetInvoiceLineItems(query));
				default:
					_logger.LogInformation("End of Get method");
					return BadRequest();
			}
			
		}

		private List<Dictionary<string, string>> GetIndividuals(ViewListQuery query)
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

		private List<Dictionary<string, string>> GetInvoices(ViewListQuery query)
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

		private List<Dictionary<string, string>> GetAdditionalFields(ViewListQuery query)
		{
			_logger.LogInformation("Start of GetAdditionalFields method");
			List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
			using (var db = new TheCompanyDbContext())
			{
				Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				List<AdditionalField> additionalFields = db.AdditionalFields.Where(obj => obj.Owner == owner)
																	.OrderBy(obj => obj.Name)
																	.FilterDynamic(query.Filters)
																	.ToList();
				Dictionary<string, string> header = new Dictionary<string, string>();
				header.Add("0", "Id");
				header.Add("1", "Name");
				values.Add(header);
				additionalFields.ForEach(additionalField =>
				{
					Dictionary<string, string> value = new Dictionary<string, string>();
					value.Add("0", additionalField.Id.ToString());
					value.Add("1", additionalField.Name);
					values.Add(value);
				});
			}
			_logger.LogInformation("End of GetAdditionalFields method");
			return values;
		}

		private List<Dictionary<string, string>> GetInvoiceLineItems(ViewListQuery query)
		{
			_logger.LogInformation("Start of GetInvoiceLineItems method");
			List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
			using (var db = new TheCompanyDbContext())
			{
				Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				List<InvoiceLineItem> invoiceLineItems = db.InvoiceLineItems.Where(obj => obj.Owner == owner)
																	.OrderBy(obj => obj.CreationDateTime)
																	.FilterDynamic(query.Filters)
																	.ToList();
				Dictionary<string, string> header = new Dictionary<string, string>();
				header.Add("0", "Reference");
				header.Add("1", "Description");
				header.Add("2", "Quantity");
				header.Add("3", "UnitaryPrice");
				header.Add("4", "Price");
				values.Add(header);
				invoiceLineItems.ForEach(invoiceLineItem =>
				{
					Dictionary<string, string> value = new Dictionary<string, string>();
					value.Add("0", invoiceLineItem.Reference);
					value.Add("1", invoiceLineItem.Description);
					value.Add("2", invoiceLineItem.Quantity.ToString());
					value.Add("3", invoiceLineItem.UnitaryPrice.ToString());
					value.Add("4", invoiceLineItem.Price.ToString());
					values.Add(value);
				});
			}
			_logger.LogInformation("End of GetInvoiceLineItems method");
			return values;
		}
	}
}
