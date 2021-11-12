using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp.DbInterfaces
{
	public interface IAttachment
	{
		public Guid FileId { get; set; }
		public string FileName { get; set; }
		public long FileSize { get; set; }
	}
}
