using ModelsApp.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class UserRegisterQuery
	{
		public string Email { get; set; }
		public string Password { get; set; }

		public static explicit operator User(UserRegisterQuery user)
		{
			User dbUser = new User();
			dbUser.Login = user.Email;
			dbUser.Email = user.Email;
			dbUser.Password = user.Password;
			return dbUser;
		}
	}
}
