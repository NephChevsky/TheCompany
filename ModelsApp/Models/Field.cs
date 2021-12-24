using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.Models
{
	public class Field
	{
		public string DataSource { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }
		public List<string> PossibleValues { get; set; }
		public bool Editable { get; set; }
		public bool AutoCompletable { get; set; }
		public Bindings Bindings { get; set; }

		public Field()
        {
			PossibleValues = new List<string>();
		}

		public Field(string dataSource, string name, string type, string value)
        {
			DataSource = dataSource;
			Name = name;
			Type = type;
			Value = value;
			Editable = true;
        }
	}
}
