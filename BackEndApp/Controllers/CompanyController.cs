using BackEndApp.DTO;
using DbApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using StorageApp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace BackEndApp.Controllers
{
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private IConfiguration Configuration { get; }

        public CompanyController(IConfiguration configuration, ILogger<CompanyController> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        [HttpGet("Get")]
        public ActionResult Get()
        {
            _logger.LogInformation("Start of Get method");
            using (var db = new TheCompanyDbContext())
            {
                Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                Company dbCompany = db.Companies.Where(x => x.Owner == owner).SingleOrDefault();
                if (dbCompany == null)
                {
                    dbCompany = new Company();
                }

                _logger.LogInformation("End of Get method");
                return Ok((CompanyGetResponse)dbCompany);
            }
        }

        [HttpPost("Save")]
        public ActionResult Save([FromBody] CompanySaveQuery query)
        {
            _logger.LogInformation("Start of Save method");
            if (query == null || query.Fields == null || query.Fields.Count == 0)
            {
                return BadRequest();
            }

            using (var db = new TheCompanyDbContext())
            {
                Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                Company dbCompany = db.Companies.Where(x => x.Owner == owner).SingleOrDefault();
                Company currentCompany;
                if (dbCompany == null)
                {
                    currentCompany = new Company();
                    currentCompany.Owner = owner;
                }
                else
                {
                    currentCompany = dbCompany;
                }

                Type type = typeof(Company);
                bool notEditableField = false;
                query.Fields.ForEach(x =>
                {
                    PropertyInfo property = type.GetProperty(x.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (AttributeHelper.CheckAttribute<Editable>(type, property))
                    {
                        if (property.PropertyType == typeof(DateTime))
                        {
                            property.SetValue(currentCompany, DateTime.Parse(x.Value));
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(currentCompany, x.Value);
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            property.SetValue(currentCompany, Double.Parse(x.Value));
                        }
                        else if (property.PropertyType == typeof(Guid))
                        {
                            property.SetValue(currentCompany, Guid.Parse(x.Value));
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

                if (dbCompany == null)
                {
                    db.Add(currentCompany);
                }

                db.SaveChanges();

                _logger.LogInformation("End of Save method");
                return Ok();
            }
        }

        [HttpPost("SaveLogo")]
        public ActionResult SaveLogo([FromForm] IFormFile file)
        {
            _logger.LogInformation("Start of SaveLogo method");

            if (file == null)
            {
                return BadRequest();
            }

            if (file.FileName.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0
                || file.FileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
            {
                return UnprocessableEntity("InvalidCharacters");
            }

            using (var db = new TheCompanyDbContext())
            {
                Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
                Company dbCompany = db.Companies.Where(x => x.Owner == owner).SingleOrDefault();
                if (dbCompany == null)
                {
                    return UnprocessableEntity();
                }

                Storage storage = new Storage(Configuration.GetSection("Storage"), owner);
                MemoryStream stream = new MemoryStream();
                file.OpenReadStream().CopyTo(stream);
                Guid logoId;
                if (!storage.CreateFile(stream, out logoId))
                {
                    return UnprocessableEntity();
                }
                if (dbCompany.Logo != Guid.Empty)
                {
                    storage.DeleteFile(dbCompany.Logo);
                }
                dbCompany.Logo = logoId;
                db.SaveChanges();

                _logger.LogInformation("End of SaveLogo method");
                return Ok();
            }
        }
    }
}
