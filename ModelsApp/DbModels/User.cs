using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelsApp.DbModels
{
	public class User : ISoftDeleteable, IDateTimeTrackable
	{
		[IdentifierField]
		public Guid Id { get; set; }

		[TextField]
		public string Login { get; set; }

		[TextField]
		public string Email { get; set; }

		[TextField]
		public string Password { get; set; } // TODO: hide password in http request

		[DateTimeField]
		public DateTime LastLoginDateTime { get; set; }

		// ISoftDeleteable
		[BooleanField]
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		[DateTimeField]
		public DateTime CreationDateTime { get; set; }

		[DateTimeField]
		public DateTime LastModificationDateTime { get; set; }

		public User()
		{

		}
	}
}
