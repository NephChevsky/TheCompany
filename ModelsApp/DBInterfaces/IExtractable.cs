using ModelsApp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbInterfaces
{
	public interface IExtractable
	{
		[BooleanField]
		public bool? ShouldBeExtracted { get; set; }

		[BooleanField]
		public bool? IsExtracted { get; set; }

		[IdentifierField]
		public Guid ExtractId { get; set; }

		[DateTimeField]
		public DateTime ExtractDateTime { get; set; }
	}
}
