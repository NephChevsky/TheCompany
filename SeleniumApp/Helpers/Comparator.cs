using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Newtonsoft.Json;

namespace SeleniumApp.Helpers
{
    public static class Comparator
    {
        public static string DataFolder = @"D:\Dev\TheCompany\SeleniumData";
        public static List<Tuple<object,object>> Expectations = new List<Tuple<object,object>>();

        public static void Equal<T>(string folder, string name, T obj)
        {
            T original = Load<T>(folder, name);
            Dump<T>(name, obj);
            Expectations.Add(new Tuple<object, object>(original, obj));
        }

        public static T Load<T>(string folder, string name)
        {
            string content = File.ReadAllText(Path.Combine(DataFolder, folder, name + ".json"));
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static void Dump<T>(string name, T content)
        {
            string text = JsonConvert.SerializeObject(content, Formatting.Indented);
            if (!Directory.Exists(Path.Combine(DataFolder, "Current")))
            {
                Directory.CreateDirectory(Path.Combine(DataFolder, "Current"));
            }
            File.WriteAllText(Path.Combine(DataFolder, "Current", name + ".json"), text);
        }

        public static void Run()
        {
            Assert.All(Expectations, pair => Assert.Equal(pair.Item1, pair.Item2));
        }
    }
}
