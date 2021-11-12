using ModelsApp;
using ModelsApp.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class UserLoginResponse
	{
		public Guid Id { get; set; }
		public string Token { get; set; }

		public UserLoginResponse(User user, string token)
		{
			Id = user.Id;
			Token = token;
		}
	}
}
