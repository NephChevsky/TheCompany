using ModelsApp.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class UserRegisterQuery
	{
		public string Login { get; set; }
		public string Password { get; set; }

		public static explicit operator User(UserRegisterQuery user)
		{
			User dbUser = new User();
			dbUser.Login = user.Login;
			dbUser.Email = user.Login;
			dbUser.Password = user.Password;
			return dbUser;
		}
	}
}
