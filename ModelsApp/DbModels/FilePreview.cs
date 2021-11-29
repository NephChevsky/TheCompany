using ModelsApp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbModels
{
	public class FilePreview
	{
		[IdentifierField]
		public Guid Id { get; set; }

		[IdentifierField]
		public Guid FileId { get; set; }

		[NumberField]
		public int Page { get; set; }

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
	}
}
