using ModelsApp.Attributes;
using ModelsApp.Models;
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

		public static Field GetProperty(object element, string name)
        {
			Field field = GetProperty(element.GetType(), name);
			PropertyInfo property = element.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			field.Value = AttributeHelper.GetFieldValue(element, property);

			return field;
		}

		public static Field GetProperty(Type type, string name)
        {
			PropertyInfo property = type.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			Field field = new Field();
			field.DataSource = type.Name;
			field.Name = property.Name;
			field.Value = "";
			field.Type = AttributeHelper.GetFieldType(property);
			field.Editable = AttributeHelper.CheckAttribute<Editable>(type, property.Name);
			field.AutoCompletable = AttributeHelper.CheckAttribute<AutoCompletable>(type, property.Name);

			if (AttributeHelper.CheckAttribute<Bindable>(type, property.Name))
			{
				Bindable bindable = property.GetCustomAttribute<Bindable>(false);
				field.Bindings = new Bindings(bindable);
			}

			if (field.Type == "ComboField")
			{
				ComboField attr = property.GetCustomAttribute<ComboField>(false);
				field.PossibleValues = attr.Values;
			}

			return field;
		}

		public static List<Field> GetAuthorizedProperties<T>(object element)
		{
			List<Field> result = new List<Field>();
			List<PropertyInfo> properties = GetAuthorizedProperties<T>(element.GetType());
			foreach (PropertyInfo property in properties)
			{
				Field field = GetProperty(element, property.Name);
				result.Add(field);
			}
			return result;
		}

		public static List<PropertyInfo> GetAuthorizedProperties<T>(Type type)
		{
			List<PropertyInfo> result = new List<PropertyInfo>();
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo property in properties)
			{
				if (AttributeHelper.CheckAttribute<T>(type, property))
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
				else if (attr is FileField)
				{
					return "FileField";
				}
				else if (attr is ComboField)
				{
					return "ComboField";
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
					object value = property.GetValue(element);
					if (value != null)
						return value.ToString();
					else
						return "";
				}
				else if (attr is MultilineTextField)
				{
					object value = property.GetValue(element);
					if (value != null)
						return value.ToString();
					else
						return "";
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
					var value = property.GetValue(element);
					if (value != null)
						return property.GetValue(element).ToString();
					else
						return "";
				}
				else if (attr is FileField)
				{
					return property.GetValue(element).ToString();
				}
				else if (attr is ComboField)
				{
					var value = property.GetValue(element);
					if (value != null)
						return property.GetValue(element).ToString();
					else
						return "";
				}
			}
			throw new Exception("Unknown field type for property " + property.Name);
		}
	}
}
