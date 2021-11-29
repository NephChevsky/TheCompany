using ModelsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class ViewListGetResponse
	{
		public List<Item> Items { get; set; }
		public List<Field> FieldsData { get; set; }

		public class Item
		{
			public string LinkValue { get; set; }
			public List<Field> Fields { get; set; }
		}
	}
}
