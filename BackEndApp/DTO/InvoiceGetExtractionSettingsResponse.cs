using ModelsApp.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class InvoiceGetExtractionSettingsResponse
	{
		public string DataSource { get; set; }
		public string Name { get; set; }
		public string Id { get; set; }
		public int? X { get; set; }
		public int? Y { get; set; }
		public int? Width { get; set; }
		public int? Height { get; set; }

		public static implicit operator InvoiceGetExtractionSettingsResponse(ExtractionSettings settings)
		{
			InvoiceGetExtractionSettingsResponse result = new InvoiceGetExtractionSettingsResponse();
			result.DataSource = settings.DataSource;
			result.Name = settings.Name;
			result.X = settings.X;
			result.Y = settings.Y;
			result.Width = settings.Width;
			result.Height = settings.Height;
			return result;
		}
	}
}
