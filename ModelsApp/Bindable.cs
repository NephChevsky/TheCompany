using System;
using System.Collections.Generic;

namespace ModelsApp
{
    public class Bindable : Attribute
    {
        public string DataSource { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Childs { get; set; }

        public Bindable(string dataSource, string name, params string[] childs)
        {
            DataSource = dataSource;
            Name = name;
            if (childs.Length %2 != 0)
                throw new Exception("Incomplete binding map");
            Childs = new Dictionary<string, string>();
            for (int i = 0; i < childs.Length; i++)
            {
                Childs.Add(childs[i], childs[i + 1]);
                i++;
            }
        }
    }
}
