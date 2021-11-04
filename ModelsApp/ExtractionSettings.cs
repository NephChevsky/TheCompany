using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp
{
	public class ExtractionSettings : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		public Guid Id { get; set; }
		public string DataSource { get; set; }
		public bool IsLineItem { get; set; }
		public string Field { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }

		// IOwnable
		public Guid Owner { get; set; }

		// ISoftDeleteable
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		public DateTime CreationDateTime { get; set; }
		public DateTime LastModificationDateTime { get; set; }

		public ExtractionSettings()
		{
		}
		
		public ExtractionSettings(string dataSource, bool isLineItem, string field, int x, int y, int width, int height, Guid owner)
		{
			DataSource = dataSource;
			IsLineItem = isLineItem;
			Field = field;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			Owner = owner;
		}
	}
}
