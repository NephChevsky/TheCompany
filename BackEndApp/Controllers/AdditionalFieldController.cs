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
			AdditionalFieldDefinition field = new AdditionalFieldDefinition(query.DataSource, query.Name);

			using (var db = new TheCompanyDbContext(owner))
			{
				db.AdditionalFieldDefinitions.Add(field);
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
	}
}
