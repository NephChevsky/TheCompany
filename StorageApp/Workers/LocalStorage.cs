using StorageApp.Interfaces;
using System;
using System.IO;

namespace StorageApp.Workers
{
	class LocalStorage : IStorage
	{
		private string RootPath { get; set; }
		public Guid Owner { get; set; }

		public LocalStorage(string rootPath, Guid owner)
		{
			RootPath = rootPath;
			Owner = owner;
		}
		
		public bool CreateFile(string fileName, MemoryStream file)
		{
			throw new System.NotImplementedException();
		}

		public bool DeleteFile(string fileName)
		{
			throw new System.NotImplementedException();
		}

		public bool GetFile(string fileName, out MemoryStream file)
		{
			throw new System.NotImplementedException();
		}
	}
}
