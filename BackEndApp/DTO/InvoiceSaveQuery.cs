using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class InvoiceSaveQuery
	{
		public string Id { get; set; }
		public List<Field> Fields { get; set; }
		public List<LineItem> LineItems { get; set; }

		public class Field
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}

		public class LineItem
		{
			public string Id { get; set; }
			public List<Field> Fields { get; set; }
		}
	}
}
