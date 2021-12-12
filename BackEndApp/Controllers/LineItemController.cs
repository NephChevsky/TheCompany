using BackEndApp.DTO;
using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using System;
using System.Linq;
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

        [HttpGet("Show/{id}")]
        public ActionResult Show(string id)
        {
            _logger.LogInformation("Start of Show method");

            if (id == null || id == "null")
            {
                LineItemDefinition lineItem = new LineItemDefinition();
                return Ok((LineItemShowResponse<Editable>)lineItem);
            }
            else
            {
                using (var db = new TheCompanyDbContext())
                {
                    Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                    LineItemDefinition dbLineItem = db.LineItemsDefinitions.Where(x => x.Id.ToString() == id && x.Owner == owner).SingleOrDefault();
                    if (dbLineItem == null)
                    {
                        return UnprocessableEntity("NotFound");
                    }
                    else
                    {
                        _logger.LogInformation("End of Show method");
                        return Ok((LineItemShowResponse<Viewable>)dbLineItem);
                    }
                }
            }
        }

        [HttpPost("Save")]
        public ActionResult Save([FromBody] LineItemSaveQuery query)
        {
            _logger.LogInformation("Start of Save method");

            if (query == null || query.Fields == null || query.Fields.Count == 0)
            {
                return BadRequest();
            }

            Guid retValue;
            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            using (var db = new TheCompanyDbContext())
            {
                LineItemDefinition lineItemDefinition;
                if (query.Id == null || query.Id == "null")
                {
                    lineItemDefinition = new LineItemDefinition();
                    lineItemDefinition.Id = Guid.NewGuid();
                    lineItemDefinition.Owner = owner;
                }
                else
                {
                    Guid lineItemId = Guid.Parse(query.Id);
                    lineItemDefinition = db.LineItemsDefinitions.Where(x => x.Owner == owner && x.Id == lineItemId).SingleOrDefault();
                }

                Type type = typeof(LineItemDefinition);
                bool notEditableField = false;
                query.Fields.ForEach(x =>
                {
                    PropertyInfo property = type.GetProperty(x.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (AttributeHelper.CheckAttribute<Editable>(type, property))
                    {
                        if (property.PropertyType == typeof(DateTime))
                        {
                            property.SetValue(lineItemDefinition, DateTime.Parse(x.Value));
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(lineItemDefinition, x.Value);
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            property.SetValue(lineItemDefinition, Double.Parse(x.Value));
                        }
                        else if (property.PropertyType == typeof(Guid))
                        {
                            property.SetValue(lineItemDefinition, Guid.Parse(x.Value));
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
                    return BadRequest();

                if (query.Id == null || query.Id == "null")
                {
                    db.Add(lineItemDefinition);
                }

                retValue = lineItemDefinition.Id;

                db.SaveChanges();

                _logger.LogInformation("End of Save method");
                return Ok(retValue);
            }
        }
    }
}
