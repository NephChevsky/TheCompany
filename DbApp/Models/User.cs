using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.Models
{
	public partial class User : ISoftDeleteable, IDateTimeTrackable
	{
		public Guid Id { get; set; }

		public string Login { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		[NotMapped]
		public string Token { get; set; }

		public string LastLoginDateTime { get; set; }

		// ISoftDeleteable
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		public DateTime CreationDateTime { get; set; }

		public DateTime LastModificationDateTime { get; set; }
	}
}
