using System;
using System.Collections.Generic;
using System.Text;

namespace DbApp.Models
{
	public interface ILockable
	{
		public string LockedBy { get; set; }
	}
}
