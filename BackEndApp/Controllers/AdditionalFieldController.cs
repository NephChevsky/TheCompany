using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelsApp;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace BackEndApp.Controllers
{
	public class AdditionalFieldController : Controller
	{
		private readonly ILogger<AdditionalFieldController> _logger;

		public AdditionalFieldController(ILogger<AdditionalFieldController> logger)
		{
			_logger = logger;
		}

		[HttpPost]
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
	}
}
