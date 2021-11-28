using ModelsApp.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModelsApp.Helpers
{
	public static class AttributeHelper
	{
		public static bool CheckAttribute<T>(Type type)
		{
			System.Attribute[] attrs = System.Attribute.GetCustomAttributes(type);
			foreach (System.Attribute attr in attrs)
			{
				if (attr is T)
				{
					return true;
				}
			}
			return false;
		}

		public static bool CheckAttribute<T>(Type type, string fieldName)
		{
			PropertyInfo property = type.GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			return CheckAttribute<T>(type, property);
		}

		public static bool CheckAttribute<T>(Type type, PropertyInfo property)
		{
			System.Attribute[] attrs = System.Attribute.GetCustomAttributes(property);
			foreach (System.Attribute attr in attrs)
			{
				if (attr is T)
				{
					return true;
				}
			}
			return false;
		}

		public static List<PropertyInfo> GetAuthorizedProperties<T>(Type type)
		{
			List<PropertyInfo> result = new List<PropertyInfo>();
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo property in properties)
			{
				if (AttributeHelper.CheckAttribute<Extractable>(type, property))
					result.Add(property);
			}
			return result;
		}

		public static List<string> GetAuthorizedPropertiesAsString<T>(Type type)
		{
			List<string> result = new List<string>();
			List<PropertyInfo> properties = GetAuthorizedProperties<T>(type);
			foreach (PropertyInfo property in properties)
			{
				result.Add(property.Name);
			}
			return result;
		}
	}
}
