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
			result.Fields = AttributeHelper.GetAuthorizedProperties<T>(lineItem);
			return result;
		}
	}
}
