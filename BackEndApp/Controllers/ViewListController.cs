using DbApp.Models;
using BackEndApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ModelsApp;
using Microsoft.Extensions.Logging;
using ModelsApp.DbModels;
using BackEndApp.DTO;
using System.Reflection;

namespace BackEndApp.Controllers
{
	[Route("[controller]")]
	public class ViewListController : ControllerBase
	{
		private readonly ILogger<ViewListController> _logger;

		public ViewListController(ILogger<ViewListController> logger)
		{
			_logger = logger;
		}

		[HttpPost("Get")]
		public ActionResult Get([FromBody] ViewListQuery query)
		{
			_logger.LogInformation("Start of Get method");
			if (query == null)
			{
				_logger.LogInformation("End of Get method");
				return BadRequest();
			}

			query.Fields.Insert(0, "Id");

			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			List<Dictionary<string, string>> values;
			using (var db = new TheCompanyDbContext())
			{
				switch (query.DataSource)
				{
					case "Individual":
						List<Individual> individuals = db.Individuals.Where(obj => obj.Owner == owner)
																	.OrderBy(obj => obj.CustomerId)
																	.FilterDynamic(query.Filters)
																	.ToList();
						values = FormatResultValues(individuals, query.Fields);
						break;
					case "Invoice":
						List<Invoice> invoices = db.Invoices.Where(obj => obj.Owner == owner)
															.OrderBy(obj => obj.InvoiceNumber)
															.FilterDynamic(query.Filters)
															.ToList();
						values = FormatResultValues(invoices, query.Fields);
						break;
					case "AdditionalField":
						List<AdditionalField> additionalFields = db.AdditionalFields.Where(obj => obj.Owner == owner)
																					.OrderBy(obj => obj.Name)
																					.FilterDynamic(query.Filters)
																					.ToList();
						values = FormatResultValues(additionalFields, query.Fields);
						break;
					case "InvoiceLineItem":
						List<InvoiceLineItem> lineItems = db.InvoiceLineItems.Where(obj => obj.Owner == owner)
																			.OrderBy(obj => obj.CreationDateTime)
																			.FilterDynamic(query.Filters)
																			.ToList();
						values = FormatResultValues(lineItems, query.Fields);
						break;
					default:
						return BadRequest();
				}
			}

			return Ok(values);
		}

		private List<Dictionary<string, string>> FormatResultValues<T>(List<T> values, List<string> fields)
		{
			List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
			Type type = typeof(T);
			values.ForEach(item =>
			{
				Dictionary<string, string> line = new Dictionary<string, string>();
				fields.ForEach(field =>
				{
					PropertyInfo property = type.GetProperty(field);
					string value;
					if (property.PropertyType == typeof(string))
					{
						value = property.GetValue(item) as string;
					}
					else if (property.PropertyType == typeof(Guid))
					{
						value = property.GetValue(item).ToString();
					}
					else if (property.PropertyType == typeof(double))
					{
						value = property.GetValue(item).ToString();
					}
					else
					{
						throw new Exception("Unknown property type");
					}
					line.Add(field, value);
				});
				result.Add(line);
			});
			return result;
		}
	}
}
