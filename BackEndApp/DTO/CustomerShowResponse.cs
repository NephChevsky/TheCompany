using ModelsApp.DbModels;
using ModelsApp.Helpers;
using ModelsApp.Models;
using System.Collections.Generic;

namespace BackEndApp.DTO
{
    public class CustomerShowResponse<T>
    {
		public List<Field> Fields { get; set; }

		public static implicit operator CustomerShowResponse<T>(Individual customer)
		{
			CustomerShowResponse<T> result = new CustomerShowResponse<T>();
			result.Fields = AttributeHelper.GetAuthorizedPropertiesAsField<T>(customer);
			return result;
		}
	}
}
