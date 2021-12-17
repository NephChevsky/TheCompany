using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.Attributes
{
    public class AutoCompletable : Attribute
    {
        public string DataSource { get; set; }
        public string Name { get; set; }

        public AutoCompletable(string dataSource, string name)
        {
            DataSource = dataSource;
            Name = name;
        }
    }
}
