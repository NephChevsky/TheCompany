using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.Models
{
	public class Invoice : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		public Guid Id { get; set; }

		// IOwnable
		public Guid Owner { get; set; }

		// ISoftDeleteable
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		public DateTime CreationDateTime { get; set; }

		public DateTime LastModificationDateTime { get; set; }
	}
}
