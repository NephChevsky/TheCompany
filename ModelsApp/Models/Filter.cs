﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.Models
{
	public class Filter
	{
		public string FieldName { get; set; }
		public string Operator { get; set; }
		public string FieldValue { get; set; }
	}
}
