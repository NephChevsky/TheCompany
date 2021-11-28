using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

		public class Field
		{
			public string Name { get; set; }
			public string Type { get; set; }
			public string Value { get; set; }
		}
	}
}
