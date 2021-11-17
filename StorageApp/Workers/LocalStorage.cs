using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StorageApp
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
			try
			{
				
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool DeleteFile(string fileName)
		{
			try
			{
				
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool GetFile(string fileName, out MemoryStream file)
		{
			file = new MemoryStream();
			try
			{
				
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}
