using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbInterfaces
{
	public interface ILockable
	{
		public string LockedBy { get; set; }
	}
}
