using BackEndApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BackEndApp.Controllers
{
	public class UserController : Controller
	{
		private IConfiguration Configuration { get; }

		public UserController(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Login([FromBody] User user)
		{
			if (user == null || user.Login == null || user.Password == null)
			{
				return BadRequest();
			}
			using (var db = new TheCompanyDbContext())
			{
				User dbUser = db.Users.Where(obj => obj.Login == user.Login).SingleOrDefault();
				if (dbUser != null)
				{
					if (BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
					{
						var tokenHandler = new JwtSecurityTokenHandler();
						var key = Encoding.UTF8.GetBytes(Configuration["AppSettings:AuthenticationSecret"]);
						var tokenDescriptor = new SecurityTokenDescriptor
						{
							Subject = new ClaimsIdentity(new Claim[]
							{
								new Claim(ClaimTypes.Name, dbUser.Id.ToString())
							}),
							Expires = DateTime.UtcNow.AddHours(10),
							SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
						};
						var token = tokenHandler.CreateToken(tokenDescriptor);
						dbUser.Token = tokenHandler.WriteToken(token);
						return Ok(dbUser);
					}
					else
					{
						return Unauthorized();
					}
				}
				else
				{
					return Unauthorized();
				}
			}
		}

		[HttpPost]
		[AllowAnonymous]
		public ActionResult Register([FromBody] User user)
		{
			if (user == null || user.Login == null || user.Password == null)
			{
				return BadRequest();
			}
			string salt = BCrypt.Net.BCrypt.GenerateSalt(10);
			string password = user.Password;
			user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);
			user.Email = user.Login;
			using (var db = new TheCompanyDbContext())
			{
				db.Users.Add(user);
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
			user.Password = password;
			return Login(user);
		}
	}
}
