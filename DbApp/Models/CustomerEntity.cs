using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbApp.Models
{
	public partial class CustomerEntity : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		public Guid Id { get; set; }

		public int Type { get; set; }

		public string CustomerId { get; set; }

		// IOwnable
		public Guid Owner { get; set; }

		// ISoftDeleteable
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		public DateTime CreationDateTime { get; set; }

		public DateTime LastModificationDateTime { get; set; }
	}
}
