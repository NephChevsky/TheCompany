using DbApp.Models;
using StorageApp.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace StorageApp.Workers
{
	class LocalStorage : IStorage
	{
		private string RootPath { get; set; }
		public Guid Owner { get; set; }
		private string ClientPath
		{
			get
			{
				return Path.Combine(RootPath, Owner.ToString());
			}
		}

		public LocalStorage(string rootPath, Guid owner)
		{
			RootPath = rootPath;
			Owner = owner;
		}
		
		public bool CreateFile(MemoryStream file, out Guid Id)
		{
			string day = DateTime.Now.ToString("yyyy-MM-dd");
			string path = Path.Combine(ClientPath, day);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			using (var db = new TheCompanyDbContext(Owner))
			{
				Id = Guid.NewGuid();
				string filePath = Path.Combine(ClientPath, day, Id.ToString() + ".file");
				using (var fs = new FileStream(filePath, FileMode.Create))
				{
					file.Seek(0, SeekOrigin.Begin);
					file.CopyTo(fs);
				}
				ModelsApp.DbModels.File dbFile = new ModelsApp.DbModels.File(Id, filePath);
				db.Files.Add(dbFile);
				db.SaveChanges();
			}
			return true;
		}

		public bool DeleteFile(Guid id)
		{
			using (var db = new TheCompanyDbContext(Owner))
			{
				ModelsApp.DbModels.File dbFile = db.Files.Where(x => x.Id == id).SingleOrDefault();
				if (dbFile != null)
				{
					string newPath = Path.Combine(ClientPath, "Deleted", Path.GetDirectoryName(dbFile.FilePath).Replace(ClientPath + "\\", ""), Path.GetFileName(dbFile.FilePath));
					if (!Directory.Exists(Path.GetDirectoryName(newPath)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(newPath));
					}
					File.Move(dbFile.FilePath, newPath);
					db.Files.Remove(dbFile);
					db.SaveChanges();
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public bool GetFile(Guid id, out MemoryStream file)
		{
			file = new MemoryStream();
			using (var db = new TheCompanyDbContext(Owner))
			{
				ModelsApp.DbModels.File dbFile = db.Files.Where(x => x.Id == id).SingleOrDefault();
				if (dbFile != null)
				{
					using (FileStream physicalFile = new FileStream(dbFile.FilePath, FileMode.Open, FileAccess.Read))
					{
						byte[] bytes = new byte[physicalFile.Length];
						physicalFile.Read(bytes, 0, (int)physicalFile.Length);
						file.Write(bytes, 0, (int)physicalFile.Length);
					}
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
}
