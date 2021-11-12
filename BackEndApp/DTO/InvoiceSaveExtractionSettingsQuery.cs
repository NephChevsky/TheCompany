using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class InvoiceSaveExtractionSettingsQuery
	{
		public InvoiceExtractionSettings InvoiceSettings { get; set; }
		public LineItemExtractionSettings LineItemSettings { get; set; }
		

		public class InvoiceExtractionSettings
		{
			public List<Field> Fields { get; set; }

			public class Field
			{
				public string Name { get; set; }
				public string Id { get; set; }
				public int X { get; set; }
				public int Y { get; set; }
				public int Width { get; set; }
				public int Height { get; set; }
			}
		}

		public class LineItemExtractionSettings
		{
			public int BoxYMin { get; set; }
			public int BoxYMax { get; set; }
			public int ReferenceXMin { get; set; }
			public int ReferenceXMax { get; set; }
			public int DescriptionXMin { get; set; }
			public int DescriptionXMax { get; set; }
			public int QuantityXMin { get; set; }
			public int QuantityXMax { get; set; }
			public int UnitaryPriceXMin { get; set; }
			public int UnitaryPriceXMax { get; set; }
			public int PriceXMin { get; set; }
			public int PriceXMax { get; set; }
		}
	}
}
