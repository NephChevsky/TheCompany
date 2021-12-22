using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.Models
{
    public class Bindings
    {
        public string DataSource { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Childs { get; set; }

        public Bindings(Bindable bindable)
        {
            DataSource = bindable.DataSource;
            Name = bindable.Name;
            Childs = bindable.Childs;
        }
    }
}
