using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbModels
{
    public class AdditionalField : IOwnable, ISoftDeleteable, IDateTimeTrackable
    {
		[IdentifierField]
		public Guid Id { get; set; }

		[IdentifierField]
		public Guid SourceId { get; set; }

		[IdentifierField]
		public Guid FieldId { get; set; }

		[TextField]
		public string Value { get; set; }

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

		public AdditionalField()
        {

        }

		public AdditionalField(Guid sourceId, Guid fieldId, string value)
        {
			SourceId = sourceId;
			FieldId = fieldId;
			Value = value;
        }
	}
}
