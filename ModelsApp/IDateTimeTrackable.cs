﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp
{
	public interface IDateTimeTrackable
	{
		public DateTime CreationDateTime { get; set; }

		public DateTime LastModificationDateTime { get; set; }
	}
}