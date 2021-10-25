using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp
{
	public interface ILockable
	{
		public string LockedBy { get; set; }
	}
}
