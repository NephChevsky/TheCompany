using BackEndApp.DTO;
using DbApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ModelsApp;
using ModelsApp.DbModels;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BackEndApp.Controllers
{
	[Route("[controller]")]
	public class UserController : ControllerBase
	{
		private readonly ILogger<UserController> _logger;
		private IConfiguration Configuration { get; }


		public UserController(IConfiguration configuration, ILogger<UserController> logger)
		{
			Configuration = configuration;
			_logger = logger;
		}

		[HttpPost("Login")]
		[AllowAnonymous]
		public ActionResult Login([FromBody] UserLoginQuery user)
		{
			_logger.LogInformation("Start of Login method");
			if (string.IsNullOrEmpty(user.Login) || string.IsNullOrEmpty(user.Password))
			{
				_logger.LogInformation("End of Login method");
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
						string strToken = tokenHandler.WriteToken(token);
						dbUser.LastLoginDateTime = DateTime.Now;
						db.SaveChanges();
						_logger.LogInformation("End of Login method");
						return Ok(new UserLoginResponse(dbUser, strToken));
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

		[HttpPost("Register")]
		[AllowAnonymous]
		public ActionResult Register([FromBody] UserRegisterQuery user)
		{
			_logger.LogInformation("Start of Register method");
			if (string.IsNullOrEmpty(user.Login) || string.IsNullOrEmpty(user.Password))
			{
				return BadRequest();
			}
			string salt = BCrypt.Net.BCrypt.GenerateSalt(10);
			string password = user.Password;
			user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);
			using (var db = new TheCompanyDbContext())
			{
				db.Users.Add((User) user);
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
			_logger.LogInformation("End of Register method");
			return Login((UserLoginQuery) user);
		}
	}
}
