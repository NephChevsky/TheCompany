using BackEndApp.DTO;
using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelsApp;
using ModelsApp.DbModels;
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
		public ActionResult Add([FromBody] AdditionalFieldAddQuery query)
		{
			_logger.LogInformation("Start of Add method");

			if (query.DataSource == null && query.Name == null)
				return BadRequest();

			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			AdditionalField field = new AdditionalField(query.DataSource, query.Name);

			using (var db = new TheCompanyDbContext(owner))
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
			List<AdditionalFieldGetResponse> result = new List<AdditionalFieldGetResponse>();
			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			using (var db = new TheCompanyDbContext(owner))
			{
				List<AdditionalField> dbAdditionalFields = db.AdditionalFields.Where(x => x.DataSource == dataSource).OrderBy(x => x.Name).ToList();
				dbAdditionalFields.ForEach(additionalField =>
				{
					result.Add((AdditionalFieldGetResponse)additionalField);
				});
				_logger.LogInformation("End of Get method");
				return Ok(result);
			}
		}
	}
}
