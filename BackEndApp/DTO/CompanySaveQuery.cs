using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BackEndApp.DTO
{
	public class CompanySaveQuery
	{
		public List<Field> Fields { get; set; }

		public class Field
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}
	}
}
