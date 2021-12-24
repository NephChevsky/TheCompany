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
			result.Fields = AttributeHelper.GetAuthorizedPropertiesAsField<T>(invoice);
			return result;
		}
	}
}
