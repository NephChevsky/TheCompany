using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp.DbModels
{
	public class Customer : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		[IdentifierField]
		public Guid Id { get; set; }

		[NumberField]
		public int Type { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		public string CustomerNumber { get; set; }

		// IOwnable
		[IdentifierField]
		public Guid Owner { get; set; }

		// ISoftDeleteable
		[BooleanField]
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		[DateTimeField]
		[Viewable]
		public DateTime CreationDateTime { get; set; }

		[DateTimeField]
		[Viewable]
		public DateTime LastModificationDateTime { get; set; }
	}
}
