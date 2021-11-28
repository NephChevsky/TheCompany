using ModelsApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp.DbInterfaces
{
	public interface IDateTimeTrackable
	{
		[DateTimeField]
		public DateTime CreationDateTime { get; set; }

		[DateTimeField]
		public DateTime LastModificationDateTime { get; set; }
	}
}
