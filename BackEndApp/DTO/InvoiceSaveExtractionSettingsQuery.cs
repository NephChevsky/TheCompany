using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class InvoiceSaveExtractionSettingsQuery
	{
		public List<Field> InvoiceSettings { get; set; }
		public List<Field> LineItemSettings { get; set; }

		public class Field
		{
			public string DataSource { get; set; }
			public string Name { get; set; }
			public string Id { get; set; }
			public int X { get; set; }
			public int Y { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
		}
	}
}
