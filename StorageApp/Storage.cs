using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using StorageApp.Interfaces;
using StorageApp.Workers;

namespace StorageApp
{
	public class Storage : IStorage
	{
		private IConfiguration Configuration { get; }
		private IStorage Instance { get; set; }
		public Guid Owner { get; set; }

		public Storage(IConfiguration configuration, Guid owner)
		{
			Configuration = configuration;
			Owner = owner;
			if (Configuration["StorageType"] == "Azure")
			{
				Instance = new AzureStorage(Configuration["AzureStorageAccount"], Owner);
			}
			else if (Configuration["StorageType"] == "Local")
			{
				Instance = new LocalStorage(Configuration["RootPath"], Owner);
			}
			else
			{
				throw new Exception("No storage type defined in configuration");
			}
		}

		public bool CreateFile(MemoryStream file, out Guid Id)
		{
			return Instance.CreateFile(file, out Id);
		}

		public bool DeleteFile(Guid id)
		{
			return Instance.DeleteFile(id);
		}

		public bool GetFile(Guid id, out MemoryStream file)
		{
			return Instance.GetFile(id, out file);
		}
	}
}
