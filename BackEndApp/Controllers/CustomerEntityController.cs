using BackEndApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackEndApp.Controllers
{
	public class CustomerEntityController : Controller
	{
		[HttpPost]
		public ActionResult Create([FromBody] Individual entity)
		{
			if (entity == null)
			{
				return BadRequest();
			}

			using (var db = new TheCompanyDbContext())
			{
				entity.Owner = Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
				db.Customers_Individual.Add(entity);
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

			return Ok();
		}
	}
}
