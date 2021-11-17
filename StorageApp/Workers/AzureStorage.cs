using Azure.Storage.Blobs;
using StorageApp.Interfaces;
using System;
using System.IO;

namespace StorageApp.Workers
{
	public class AzureStorage : IStorage
	{
		private string ConnectionString { get; set; }
		public Guid Owner { get; set; }
		private BlobContainerClient Container { get; set; }

		public AzureStorage(string connectionString, Guid owner)
		{
			ConnectionString = connectionString;
			Owner = owner;
			Container = new BlobContainerClient(ConnectionString, Owner.ToString());
			Container.CreateIfNotExists();
		}
		public bool CreateFile(string fileName, MemoryStream file)
		{
			try
			{
				file.Position = 0;
				Container.UploadBlob(fileName, file);
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
				Container.DeleteBlob(fileName);
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
				Container.GetBlobClient(fileName).DownloadTo(file);
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}
