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

		public static string GetFieldType(PropertyInfo property)
		{
			System.Attribute[] attrs = System.Attribute.GetCustomAttributes(property);
			foreach (System.Attribute attr in attrs)
			{
				if (attr is TextField)
				{
					return "TextField";
				}
				else if (attr is MultilineTextField)
				{
					return "MultilineTextField";
				}
				else if (attr is BooleanField)
				{
					return "BooleanField";
				}
				else if (attr is DateTimeField)
				{
					return "DateTimeField";
				}
				else if (attr is IdentifierField)
				{
					return "IdentifierField";
				}
				else if (attr is NumberField)
				{
					return "NumberField";
				}
			}
			throw new Exception("Unknown field type for property " + property.Name);
		}

		public static string GetFieldValue(object element, PropertyInfo property)
		{
			System.Attribute[] attrs = System.Attribute.GetCustomAttributes(property);
			foreach (System.Attribute attr in attrs)
			{
				if (attr is TextField)
				{
					return property.GetValue(element).ToString();
				}
				else if (attr is MultilineTextField)
				{
					return property.GetValue(element).ToString();
				}
				else if (attr is BooleanField)
				{
					return property.GetValue(element).ToString();
				}
				else if (attr is DateTimeField)
				{
					return property.GetValue(element).ToString();
				}
				else if (attr is IdentifierField)
				{
					return property.GetValue(element).ToString();
				}
				else if (attr is NumberField)
				{
					return property.GetValue(element).ToString();
				}
			}
			throw new Exception("Unknown field type for property " + property.Name);
		}
	}
}
