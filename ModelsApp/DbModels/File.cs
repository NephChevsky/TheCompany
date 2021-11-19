using ModelsApp.DbInterfaces;
using System;

namespace ModelsApp.DbModels
{
	public class File : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		public Guid Id { get; set; }
		public string FilePath { get; set; }

		// IOwnable
		public Guid Owner { get; set; }

		// ISoftDeleteable
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		public DateTime CreationDateTime { get; set; }
		public DateTime LastModificationDateTime { get; set; }

		public File()
		{

		}

		public File(Guid id, string filePath, Guid owner)
		{
			Id = id;
			FilePath = filePath;
			Owner = owner;
		}
	}
}
