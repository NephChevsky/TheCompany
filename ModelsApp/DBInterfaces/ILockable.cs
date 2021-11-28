using ModelsApp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbInterfaces
{
	public interface ILockable
	{
		[TextField]
		public string LockedBy { get; set; }
	}
}
