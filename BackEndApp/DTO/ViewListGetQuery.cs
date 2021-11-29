using ModelsApp;
using ModelsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.Models
{
	public class ViewListGetQuery
	{
		public string DataSource { get; set; }
		public List<Filter> Filters { get; set; }
		public List<string> Fields { get; set; }
		public string LinkField { get; set; }
	}
}
