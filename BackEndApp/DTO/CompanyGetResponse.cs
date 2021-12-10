using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using ModelsApp.Models;
using System.Collections.Generic;
using System.Reflection;

namespace BackEndApp.DTO
{
    public class CompanyGetResponse
    {
		public List<Field> Fields { get; set; }

		public static implicit operator CompanyGetResponse(Company company)
		{
			CompanyGetResponse result = new CompanyGetResponse();
			result.Fields = new List<Field>();
			List<PropertyInfo> properties = AttributeHelper.GetAuthorizedProperties<Viewable>(typeof(Company));
			properties.ForEach(property =>
			{
				Field field = new Field();
				field.Name = property.Name;
				field.Type = AttributeHelper.GetFieldType(property);
				field.Value = AttributeHelper.GetFieldValue(company, property);
				result.Fields.Add(field);
			});
			return result;
		}
	}
}
