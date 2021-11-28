using ModelsApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp.DbInterfaces
{
	public interface IOwnable
	{
		[IdentifierField]
		public Guid Owner { get; set; }
	}
}
