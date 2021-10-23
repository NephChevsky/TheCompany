using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.Models
{
	public interface IOwnable
	{
		public Guid Owner { get; set; }
	}
}
