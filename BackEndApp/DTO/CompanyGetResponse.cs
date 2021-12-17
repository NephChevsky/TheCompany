using ModelsApp.Attributes;
using ModelsApp.DbModels;
using ModelsApp.Helpers;
using ModelsApp.Models;
using System.Collections.Generic;
using System.Reflection;

namespace BackEndApp.DTO
{
    public class CompanyGetResponse<T>
    {
		public List<Field> Fields { get; set; }

		public static implicit operator CompanyGetResponse<T>(Company company)
		{
			CompanyGetResponse<T> result = new CompanyGetResponse<T>();
			result.Fields = AttributeHelper.GetAuthorizedProperties<T>(company);
			return result;
		}
	}
}
