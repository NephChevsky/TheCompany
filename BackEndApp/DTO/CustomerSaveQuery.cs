using System.Collections.Generic;

namespace BackEndApp.DTO
{
    public class CustomerSaveQuery
    {
		public string Id { get; set; }
		public List<Field> Fields { get; set; }

		public class Field
		{
			public string Name { get; set; }
			public string Value { get; set; }
		}
	}
}
