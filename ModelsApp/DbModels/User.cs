using ModelsApp.DbInterfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelsApp.DbModels
{
	public class User : ISoftDeleteable, IDateTimeTrackable
	{
		public Guid Id { get; set; }

		public string Login { get; set; }

		public string Email { get; set; }

		public string Password { get; set; } // TODO: hide password in http request

		public DateTime LastLoginDateTime { get; set; }

		// ISoftDeleteable
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		public DateTime CreationDateTime { get; set; }

		public DateTime LastModificationDateTime { get; set; }

		public User()
		{

		}
	}
}
