using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;

namespace ModelsApp.DbModels
{
	public class File : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		[IdentifierField]
		public Guid Id { get; set; }

		[TextField]
		public string FilePath { get; set; }

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

		public File()
		{

		}

		public File(Guid id, string filePath)
		{
			Id = id;
			FilePath = filePath;
		}
	}
}
