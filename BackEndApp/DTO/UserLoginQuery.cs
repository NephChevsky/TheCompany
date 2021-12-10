using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class UserLoginQuery
	{
		public string Email { get; set; }
		public string Password { get; set; }

		public static explicit operator UserLoginQuery(UserRegisterQuery user)
		{
			UserLoginQuery newUser = new UserLoginQuery();
			newUser.Email = user.Email;
			newUser.Password = user.Password;
			return newUser;
		}
	}
}
