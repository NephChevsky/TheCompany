using ModelsApp.DbModels;
using ModelsApp.Helpers;
using ModelsApp.Models;
using System.Collections.Generic;
using System.Reflection;

namespace BackEndApp.DTO
{
    public class LineItemShowResponse<T>
    {
		public List<Field> Fields { get; set; }

		public static implicit operator LineItemShowResponse<T>(LineItemDefinition lineItem)
		{
			LineItemShowResponse<T> result = new LineItemShowResponse<T>();
			result.Fields = new List<Field>();
			List<PropertyInfo> properties = AttributeHelper.GetAuthorizedProperties<T>(typeof(LineItemDefinition));
			properties.ForEach(property =>
			{
				Field field = new Field();
				field.Name = property.Name;
				field.Type = AttributeHelper.GetFieldType(property);
				field.Value = AttributeHelper.GetFieldValue(lineItem, property);
				result.Fields.Add(field);
			});
			return result;
		}
	}
}
