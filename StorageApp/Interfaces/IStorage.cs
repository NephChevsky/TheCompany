using System;
using System.IO;

namespace StorageApp.Interfaces
{
	public interface IStorage
	{
		public Guid Owner { get; set; }
		public bool CreateFile(MemoryStream file, out Guid Id);
		public bool DeleteFile(Guid Id);
		public bool GetFile(Guid Id, out MemoryStream file);
	}
}
