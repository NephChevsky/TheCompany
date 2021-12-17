using BackEndApp.DTO;
using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using StorageApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    case "Customer":
                        List<Individual> individuals = db.Individuals.OrderBy(x => property.Name).ToList();
                        values = FormatResultValues(individuals, property);
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
    }
}
