using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbModels
{
	[Viewable]
	public class AdditionalField : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		[IdentifierField]
		public Guid Id { get; set; }

		[TextField]
		public string DataSource { get; set; }

		[TextField]
		[Viewable]
		public string Name { get; set; }

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

		public AdditionalField(string dataSource, string name)
		{
			DataSource = dataSource;
			Name = name;
		}
	}
}
