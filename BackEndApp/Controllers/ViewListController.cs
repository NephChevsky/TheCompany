using DbApp.Models;
using BackEndApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ModelsApp;

namespace BackEndApp.Controllers
{
	public class ViewListController : Controller
	{
		[HttpPost]
		public ActionResult Get([FromBody] ViewListQuery query)
		{
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
		}

		private List<Dictionary<string, string>> GetIndividuals()
		{
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
			return values;
		}

		private List<Dictionary<string, string>> GetInvoices()
		{
			List<Dictionary<string, string>> values = new List<Dictionary<string, string>>();
			using (var db = new TheCompanyDbContext())
			{
				Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				List<Invoice> invoices = db.Invoices.Where(obj => obj.Owner == owner)
																	.OrderBy(obj => obj.InvoiceNumber)
																	.ToList();
				Dictionary<string, string> header = new Dictionary<string, string>();
				header.Add("0", "InvoiceNumber");
				header.Add("1", "CustomerId");
				values.Add(header);
				invoices.ForEach(invoice =>
				{
					Dictionary<string, string> value = new Dictionary<string, string>();
					value.Add("0", invoice.InvoiceNumber);
					value.Add("1", invoice.CustomerId.ToString());
					values.Add(value);
				});
			}
			return values;
		}
	}
}
