using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp
{
	public interface ISoftDeleteable
	{
		public bool Deleted { get; set; }
	}
}
