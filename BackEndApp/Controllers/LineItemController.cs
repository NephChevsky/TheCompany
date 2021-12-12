using BackEndApp.DTO;
using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using ModelsApp.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;

namespace BackEndApp.Controllers
{
    [Route("[controller]")]
    public class LineItemController : ControllerBase
    {
        private readonly ILogger<LineItemController> _logger;
        private IConfiguration Configuration { get; }

        public LineItemController(IConfiguration configuration, ILogger<LineItemController> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        [HttpGet("GetFields")]
        public ActionResult GetFields()
        {
            _logger.LogInformation("Start of GetFields method");
            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            List<PropertyInfo> properties = AttributeHelper.GetAuthorizedProperties<Editable>(typeof(LineItemDefinition));
            LineItemGetFieldsResponse result = new LineItemGetFieldsResponse();

            properties.ForEach(property =>
            {
                Field field = new Field();
                field.Name = property.Name;
                field.Type = AttributeHelper.GetFieldType(property);
                field.Value = "";
                result.Fields.Add(field);
            });

            _logger.LogInformation("End of GetFields method");
            return Ok(result);
        }

        [HttpPost("Create")]
        public ActionResult Create([FromBody] LineItemCreateQuery query)
        {
            _logger.LogInformation("Start of Create method");

            if (query == null || query.Fields == null || query.Fields.Count == 0)
            {
                return BadRequest();
            }

            using (var db = new TheCompanyDbContext())
            {
                Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                LineItemDefinition lineItem = new LineItemDefinition();
                lineItem.Owner = owner;

                Type type = typeof(LineItemDefinition);
                bool notEditableField = false;
                query.Fields.ForEach(field =>
                {
                    PropertyInfo property = type.GetProperty(field.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (AttributeHelper.CheckAttribute<Editable>(type, property))
                    {
                        if (property.PropertyType == typeof(DateTime))
                        {
                            property.SetValue(lineItem, DateTime.Parse(field.Value));
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(lineItem, field.Value);
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            property.SetValue(lineItem, Double.Parse(field.Value));
                        }
                        else if (property.PropertyType == typeof(Guid))
                        {
                            property.SetValue(lineItem, Guid.Parse(field.Value));
                        }
                        else
                        {
                            throw new Exception("Unknow property type");
                        }
                    }
                    else
                    {
                        notEditableField = true;
                    }
                });

                if (notEditableField)
                {
                    return BadRequest();
                }

                db.Add(lineItem);
                db.SaveChanges();

                _logger.LogInformation("End of Create method");
                return Ok();
            }
        }
    }
}
