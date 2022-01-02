using BackEndApp.DTO;
using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelsApp;
using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using ModelsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace BackEndApp.Controllers
{
	[Route("[controller]")]
	public class CustomerController : ControllerBase
	{
		private readonly ILogger<CustomerController> _logger;

		public CustomerController(ILogger<CustomerController> logger)
		{
			_logger = logger;
		}

        [HttpPost("Save")]
        public ActionResult Save([FromBody] CustomerSaveQuery query)
        {
            _logger.LogInformation("Start of Save method");
            if (query == null || query.Fields == null)
            {
                return BadRequest();
            }

            Guid retValue;
            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            using (var db = new TheCompanyDbContext(owner))
            {
                Individual individual;
                if (query.Id == null || query.Id == "null")
                {
                    individual = new Individual();
                    individual.Id = Guid.NewGuid();
                }
                else
                {
                    Guid customerId = Guid.Parse(query.Id);
                    individual = db.Individuals.Where(x => x.Id == customerId).SingleOrDefault();
                }

                Type type = typeof(Individual);
                bool notEditableField = false;
                query.Fields.ForEach(x =>
                {
                    PropertyInfo property = type.GetProperty(x.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property != null)
                    {
                        if (AttributeHelper.CheckAttribute<Editable>(type, property))
                        {
                            if (property.PropertyType == typeof(DateTime))
                            {
                                property.SetValue(individual, DateTime.Parse(x.Value));
                            }
                            else if (property.PropertyType == typeof(string))
                            {
                                property.SetValue(individual, x.Value);
                            }
                            else if (property.PropertyType == typeof(double))
                            {
                                property.SetValue(individual, Double.Parse(x.Value));
                            }
                            else if (property.PropertyType == typeof(Guid))
                            {
                                property.SetValue(individual, Guid.Parse(x.Value));
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
                    }
                    else
                    {
                        // TODO: use additional field id instead of name
                        AdditionalFieldDefinition additionalField = db.AdditionalFieldDefinitions.Where(field => field.DataSource=="Customer" && field.Name == x.Name).SingleOrDefault();
                        if (additionalField != null)
                        {
                            AdditionalField additionalFieldValue = db.AdditionalFields.Where(field => field.SourceId == individual.Id && field.FieldId == additionalField.Id).SingleOrDefault();
                            if (additionalFieldValue != null)
                            {
                                additionalFieldValue.Value = x.Value;
                            }
                        }
                    }
                });
                if (notEditableField)
                    return BadRequest();

                if (query.Id == null || query.Id == "null")
                {
                    db.Individuals.Add(individual);
                }

                retValue = individual.Id;

                db.SaveChanges();
            }

            _logger.LogInformation("End of Save method");
            return Ok(retValue);
        }

        [HttpGet("Show/{id}")]
		public ActionResult Show(string id)
		{
			_logger.LogInformation("Start of Show method");
			if (id == null || id == "null")
			{
				Individual customer = new Individual();
                CustomerShowResponse<Editable> result = (CustomerShowResponse<Editable>)customer;
                Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                using (var db = new TheCompanyDbContext(owner))
                {
                    List<AdditionalFieldDefinition> additionalFields = db.AdditionalFieldDefinitions.Where(x => x.DataSource == "Customer").ToList();
                    additionalFields.ForEach(field =>
                    {
                        Field newField = new Field("Customer", field.Name, "TextField", "");
                        result.Fields.Add(newField);
                    });
                }

                return Ok((CustomerShowResponse<Editable>)result);
			}
			else
			{
				Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				using (var db = new TheCompanyDbContext(owner))
				{
					Individual dbIndividual = db.Individuals.Where(x => x.Id.ToString() == id).SingleOrDefault();
					if (dbIndividual == null)
					{
						return UnprocessableEntity("NotFound");
					}
					else
					{
						CustomerShowResponse<Viewable> result = (CustomerShowResponse<Viewable>)dbIndividual;
						// TODO: use additional field id instead of name
						List<AdditionalFieldDefinition> additionalFields = db.AdditionalFieldDefinitions.Where(x => x.DataSource == "Customer").ToList();
						additionalFields.ForEach(field =>
						{
							AdditionalField value = db.AdditionalFields.Where(x => x.SourceId == dbIndividual.Id && x.FieldId == field.Id).SingleOrDefault();
							Field newField = new Field("Customer", field.Name, "TextField", "");
							if (value != null)
							{
								newField.Value = value.Value;
							}
							result.Fields.Add(newField);
						});

						_logger.LogInformation("End of Show method");
						return Ok(result);
					}
				}
			}
		}
	}
}
