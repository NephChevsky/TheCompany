using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelsApp.Attributes
{
    public class ComboField : Attribute
    {
        public List<string> Values { get; set; }

        public ComboField(params string[] values)
        {
            Values = values.ToList();
        }
    }
}
