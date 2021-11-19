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
		public bool CreateFile(MemoryStream file, out Guid Id)
		{
			try
			{
				file.Position = 0;
				Id = Guid.NewGuid();
				Container.UploadBlob(Id.ToString(), file);
			}
			catch
			{
				Id = Guid.Empty;
				return false;
			}
			return true;
		}

		public bool DeleteFile(Guid id)
		{
			try
			{
				Container.DeleteBlob(id.ToString());
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool GetFile(Guid id, out MemoryStream file)
		{
			file = new MemoryStream();
			try
			{
				Container.GetBlobClient(id.ToString()).DownloadTo(file);
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}
