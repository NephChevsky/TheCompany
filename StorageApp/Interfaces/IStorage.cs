using System;
using System.IO;

namespace StorageApp.Interfaces
{
	public interface IStorage
	{
		public Guid Owner { get; set; }
		public bool CreateFile(string fileName, MemoryStream file);
		public bool DeleteFile(string fileName);
		public bool GetFile(string fileName, out MemoryStream file);
	}
}
