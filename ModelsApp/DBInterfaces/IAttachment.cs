using ModelsApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp.DbInterfaces
{
	public interface IAttachment
	{
		[IdentifierField]
		public Guid FileId { get; set; }

		[TextField]
		public string FileName { get; set; }

		[NumberField]
		public long FileSize { get; set; }
	}
}
