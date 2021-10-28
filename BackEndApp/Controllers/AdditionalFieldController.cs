using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelsApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace BackEndApp.Controllers
{
	[Route("[controller]")]
	public class AdditionalFieldController : ControllerBase
	{
		private readonly ILogger<AdditionalFieldController> _logger;

		public AdditionalFieldController(ILogger<AdditionalFieldController> logger)
		{
			_logger = logger;
		}

		[HttpPost("Add")]
		public ActionResult Add([FromBody] JsonElement json)
		{
			_logger.LogInformation("Start of Add method");

			string dataSource = json.GetProperty("dataSource").GetString();
			string name = json.GetProperty("name").GetString();
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			AdditionalField field = new AdditionalField(dataSource, name, owner);

			using (var db = new TheCompanyDbContext())
			{
				db.AdditionalFields.Add(field);
				try
				{
					db.SaveChanges();
				}
				catch (DbUpdateException e)
				when ((e.InnerException as SqlException)?.Number == 2601 || (e.InnerException as SqlException)?.Number == 2627)
				{
					return Conflict("AlreadyExists");
				}
				catch (DbUpdateException e)
				{
					string message = e.Message;
					return Problem("DbRequestFailed");
				}
			}

			_logger.LogInformation("End of Add method");
			return Ok();
		}

		[HttpGet("Get/{dataSource}")]
		public ActionResult Get(string dataSource)
		{
			_logger.LogInformation("Start of Get method");
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			using (var db = new TheCompanyDbContext())
			{
				List<AdditionalField> dbAdditionalFields = db.AdditionalFields.Where(x => x.Owner == owner && x.DataSource == dataSource).OrderBy(x => x.Name).ToList();
				_logger.LogInformation("End of Get method");
				return Ok(dbAdditionalFields);
			}
		}
	}
}
