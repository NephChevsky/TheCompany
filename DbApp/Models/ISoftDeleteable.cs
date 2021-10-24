using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbApp.Models
{
	public interface ISoftDeleteable
	{
		public bool Deleted { get; set; }
	}
}
