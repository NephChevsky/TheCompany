using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using ModelsApp.Models;
using System.Collections.Generic;
using System.Reflection;

namespace BackEndApp.DTO
{
	public class InvoiceShowResponse<T>
	{
		public List<Field> Fields { get; set; }

		public static implicit operator InvoiceShowResponse<T>(Invoice invoice)
		{
			InvoiceShowResponse<T> result = new InvoiceShowResponse<T>();
			result.Fields = new List<Field>();
			List<PropertyInfo> properties = AttributeHelper.GetAuthorizedProperties<T>(typeof(Invoice));
			properties.ForEach(property =>
			{
				Field field = new Field();
				field.Name = property.Name;
				field.Type = AttributeHelper.GetFieldType(property);
				field.Value = AttributeHelper.GetFieldValue(invoice, property);
				result.Fields.Add(field);
			});
			return result;
		}
	}
}
