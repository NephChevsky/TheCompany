using BackEndApp.DTO;
using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelsApp;
using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using ModelsApp.Models;
using StorageApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;

namespace BackEndApp.Controllers
{
    [Route("[controller]")]
    public class FieldController : ControllerBase
    {
        private readonly ILogger<FieldController> _logger;
        private IConfiguration Configuration { get; }

        public FieldController(IConfiguration configuration, ILogger<FieldController> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        [HttpGet("GetFile/{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            _logger.LogInformation("Start of GetFile method");
            if (fileId == null)
            {
                return BadRequest();
            }

            Guid id;
            if (!Guid.TryParse(fileId, out id))
            {
                return BadRequest();
            }

            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            Storage storage = new Storage(Configuration.GetSection("Storage"), owner);
            MemoryStream stream;
            if (!storage.GetFile(id, out stream))
            {
                return UnprocessableEntity();
            }
            _logger.LogInformation("End of GetFile method");
            return Ok(Convert.ToBase64String(stream.ToArray()));
        }

        [HttpPost("GetPossibleValues")]
        public ActionResult GetPossibleValues([FromBody] FieldGetPossibleValuesQuery query)
        {
            _logger.LogInformation("Start of GetPossibleValues method");
            if (query == null || query.DataSource == null || query.Name == null)
            {
                return BadRequest();
            }

            Type type = Type.GetType("ModelsApp.DbModels." + query.DataSource + ",ModelsApp");
            if (type == null)
            {
                return BadRequest();
            }

            PropertyInfo property = type.GetProperty(query.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return BadRequest();
            }

            if (!AttributeHelper.CheckAttribute<AutoCompletable>(type, property.Name))
            {
                return BadRequest();
            }

            AutoCompletable attr = property.GetCustomAttribute<AutoCompletable>(false);

            type = Type.GetType("ModelsApp.DbModels." + attr.DataSource + ",ModelsApp");
            if (type == null)
            {
                return BadRequest();
            }

            property = type.GetProperty(attr.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return BadRequest();
            }

            List<string> values;
            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            using (var db = new TheCompanyDbContext(owner))
            {
                switch (attr.DataSource)
                {
                    case "Individual":
                        List<Individual> individuals = db.Individuals.OrderBy(x => property.Name).ToList();
                        values = FormatResultValues(individuals, property);
                        break;
                    case "LineItemDefinition":
                        List<LineItemDefinition> items = db.LineItemDefinitions.OrderBy(x => property.Name).ToList();
                        values = FormatResultValues(items, property);
                        break;
                    default:
                        return UnprocessableEntity();
                }
            }
            _logger.LogInformation("End of GetPossibleValues method");
            return Ok(values);
        }

        private List<string> FormatResultValues<T>(List<T> values, PropertyInfo property)
        {
            List<string> result = new List<string>();
            values.ForEach(x =>
            {
                result.Add(AttributeHelper.GetFieldValue(x, property));
            });
            return result;
        }

        [HttpPost("GetBindingValues")]
        public ActionResult GetBindingValues([FromBody] FieldGetBindingValuesQuery query)
        {
            _logger.LogInformation("Start of GetBindingValues method");

            if (query == null || query.DataSource == null || query.Name == null || query.Value == null)
            {
                return BadRequest();
            }

            Type type = Type.GetType("ModelsApp.DbModels." + query.DataSource + ",ModelsApp");
            if (type == null)
            {
                return BadRequest();
            }

            PropertyInfo property = type.GetProperty(query.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return BadRequest();
            }
            
            List<Field> result = new List<Field>();
            
            Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            using (var db = new TheCompanyDbContext(owner))
            {
                ParameterExpression param;
                MemberExpression prop;
                Expression left, right, exp;
                switch (query.DataSource)
                {
                    case "Individual":
                        param = Expression.Parameter(type, "e");
                        prop = Expression.PropertyOrField(param, property.Name);
                        left = prop;
                        right = Expression.Constant(query.Value);
                        exp = Expression.Equal(left, right);
                        Expression<Func<Individual, bool>> individualPredicate = Expression.Lambda<Func<Individual, bool>>(exp, param);
                        Individual individual = db.Individuals.Where(individualPredicate).FirstOrDefault();
                        if (individual != null)
                        {
                            result = AttributeHelper.GetAuthorizedPropertiesAsField<Viewable>(individual);
                        }
                        break;
                    case "LineItemDefinition":
                        param = Expression.Parameter(type, "e");
                        prop = Expression.PropertyOrField(param, property.Name);
                        left = prop;
                        right = Expression.Constant(query.Value);
                        exp = Expression.Equal(left, right);
                        Expression<Func<LineItemDefinition, bool>> lineItemPredicate = Expression.Lambda<Func<LineItemDefinition, bool>>(exp, param);
                        LineItemDefinition lineItem = db.LineItemDefinitions.Where(lineItemPredicate).FirstOrDefault();
                        if (lineItem != null)
                        {
                            result = AttributeHelper.GetAuthorizedPropertiesAsField<Viewable>(lineItem);
                        }
                        break;
                    default:
                        return UnprocessableEntity();
                }
            }

            _logger.LogInformation("End of GetBindingValues method");
            return Ok(result);
        }
    }
}
