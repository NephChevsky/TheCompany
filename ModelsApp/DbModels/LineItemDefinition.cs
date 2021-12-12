using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbModels
{
    [Viewable]
    public class LineItemDefinition : IOwnable, ISoftDeleteable, IDateTimeTrackable
    {
		[IdentifierField]
		public Guid Id { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		public string Reference { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		public string Description { get; set; }

		[NumberField]
		[Viewable]
		[Editable]
		public double Price { get; set; }

		// IOwnable
		[IdentifierField]
		public Guid Owner { get; set; }

		// ISoftDeleteable
		[BooleanField]
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		[DateTimeField]
		public DateTime CreationDateTime { get; set; }
		[DateTimeField]
		public DateTime LastModificationDateTime { get; set; }

		public LineItemDefinition()
		{
		}
	}
}
