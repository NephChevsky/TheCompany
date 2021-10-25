using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp
{
	public interface IExtractable
	{
		public bool? ShouldBeExtracted { get; set; }
		public bool? IsExtracted { get; set; }
		public Guid ExtractId { get; set; }
		public DateTime ExtractDateTime { get; set; }
	}
}
