using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbApp.Models
{
	public partial class User : ISoftDeleteable, IDateTimeTrackable
	{
		public Guid Id { get; set; }

		public string Login { get; set; }

		public string Email { get; set; }

		public string Password { get; set; } // TODO: hide password in http request

		[NotMapped]
		public string Token { get; set; }

		public DateTime LastLoginDateTime { get; set; }

		// ISoftDeleteable
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		public DateTime CreationDateTime { get; set; }

		public DateTime LastModificationDateTime { get; set; }
	}
}
