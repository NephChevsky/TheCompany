﻿using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbModels
{
	public class ExtractionSettings : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		[IdentifierField]
		public Guid Id { get; set; }

		[TextField]
		public string DataSource { get; set; }

		[BooleanField]
		public string Name { get; set; }

		[NumberField]
		public int X { get; set; }

		[NumberField]
		public int Y { get; set; }

		[NumberField]
		public int Height { get; set; }

		[NumberField]
		public int Width { get; set; }

		// IOwnable
		[IdentifierField]
		public Guid Owner { get; set; }

		// ISoftDeleteable
		[BooleanField]
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		[DateTimeField]
		public DateTime CreationDateTime { get; set; }

		[DateTimeField]
		public DateTime LastModificationDateTime { get; set; }

		public ExtractionSettings()
		{
		}
		
		public ExtractionSettings(string dataSource, string name, int x, int y, int width, int height)
		{
			DataSource = dataSource;
			Name = name;
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}
	}
}
