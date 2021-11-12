using ModelsApp.DbInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbModels
{
	public class AdditionalField : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		public Guid Id { get; set; }
		public string DataSource { get; set; }
		public string Name { get; set; }

		// IOwnable
		public Guid Owner { get; set; }

		// ISoftDeleteable
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		public DateTime CreationDateTime { get; set; }
		public DateTime LastModificationDateTime { get; set; }

		public AdditionalField()
		{

		}

		public AdditionalField(string dataSource, string name, Guid owner)
		{
			DataSource = dataSource;
			Name = name;
			Owner = owner;
		}
	}
}
