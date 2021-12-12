using DbApp.Models;
using BackEndApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using ModelsApp.DbModels;
using System.Reflection;
using ModelsApp.Attributes;
using ModelsApp.Helpers;
using BackEndApp.DTO;
using ModelsApp.Models;

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
        public ActionResult Get([FromBody] ViewListGetQuery query)
        {
            _logger.LogInformation("Start of Get method");
            if (query == null)
            {
                _logger.LogInformation("End of Get method");
                return BadRequest();
            }

            Type type = Type.GetType("ModelsApp.DbModels." + query.DataSource + ",ModelsApp");
            if (type == null)
                return BadRequest();

            if (!AttributeHelper.CheckAttribute<Viewable>(type))
                return BadRequest();

            foreach (string fieldName in query.Fields)
            {
                if (!AttributeHelper.CheckAttribute<Viewable>(type, fieldName))
                    return BadRequest();
            }

            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            ViewListGetResponse values;
            using (var db = new TheCompanyDbContext())
            {
                switch (query.DataSource)
                {
                    case "Individual":
                        List<Individual> individuals = db.Individuals.Where(obj => obj.Owner == owner)
                                                                    .OrderBy(obj => obj.CustomerNumber)
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
                    case "LineItem":
                        List<LineItem> lineItems = db.LineItems.Where(obj => obj.Owner == owner)
                                                                            .OrderBy(obj => obj.CreationDateTime)
                                                                            .FilterDynamic(query.Filters)
                                                                            .ToList();
                        values = FormatResultValues(lineItems, query.Fields);
                        break;
                    case "LineItemDefinition":
                        List<LineItemDefinition> lineItemDefinitions = db.LineItemsDefinitions.Where(obj => obj.Owner == owner)
                                                                            .OrderBy(obj => obj.CreationDateTime)
                                                                            .FilterDynamic(query.Filters)
                                                                            .ToList();
                        values = FormatResultValues(lineItemDefinitions, query.Fields);
                        break;
                    default:
                        return BadRequest();
                }
            }

            return Ok(values);
        }

        private ViewListGetResponse FormatResultValues<T>(List<T> values, List<string> fields)
        {
            ViewListGetResponse result = new ViewListGetResponse();
            result.Items = new List<ViewListGetResponse.Item>();
            Type type = typeof(T);
            result.FieldsData = new List<Field>();
            fields.ForEach(field =>
            {
                PropertyInfo property = type.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                Field newField = new Field();
                newField.Name = field;
                newField.Type = AttributeHelper.GetFieldType(property);
                result.FieldsData.Add(newField);
            });

            values.ForEach(value =>
            {
                ViewListGetResponse.Item item = new ViewListGetResponse.Item();
                item.Fields = new List<Field>();
                PropertyInfo property = type.GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                item.LinkValue = property.GetValue(value).ToString();
                fields.ForEach(field =>
                {
                    PropertyInfo property = type.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    Field newField = new Field();
                    newField.Name = property.Name;
                    newField.Type = AttributeHelper.GetFieldType(property);
                    newField.Value = AttributeHelper.GetFieldValue(value, property);
                    item.Fields.Add(newField);
                });
                result.Items.Add(item);
            });
            return result;
        }
    }
}
