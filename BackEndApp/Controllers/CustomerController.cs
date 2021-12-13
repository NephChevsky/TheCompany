using BackEndApp.DTO;
using DbApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelsApp;
using ModelsApp.DbModels;
using System;
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

		[HttpPost("Create")]
		public ActionResult Create([FromBody] CustomerCreateQuery customer)
		{
			_logger.LogInformation("Start of Create method");
			if (string.IsNullOrEmpty(customer.CustomerNumber) || string.IsNullOrEmpty(customer.LastName))
			{
				return BadRequest();
			}

			Guid owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
			using (var db = new TheCompanyDbContext(owner))
			{
				Individual newCustomer = (Individual) customer;
				db.Individuals.Add(newCustomer);
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

			_logger.LogInformation("End of Create method");
			return Ok();
		}
	}
}
