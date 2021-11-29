using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using ModelsApp.Models;
using System.Collections.Generic;
using System.Reflection;

namespace BackEndApp.DTO
{
	public class InvoiceShowResponse
	{
		public List<Field> Fields { get; set; }

		public static implicit operator InvoiceShowResponse(Invoice invoice)
		{
			InvoiceShowResponse result = new InvoiceShowResponse();
			result.Fields = new List<Field>();
			List<PropertyInfo> properties = AttributeHelper.GetAuthorizedProperties<Viewable>(typeof(Invoice));
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
