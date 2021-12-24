using ModelsApp.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class AdditionalFieldGetResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public static explicit operator AdditionalFieldGetResponse(AdditionalFieldDefinition v)
		{
			AdditionalFieldGetResponse newItem = new AdditionalFieldGetResponse();
			newItem.Id = v.Id;
			newItem.Name = v.Name;
			return newItem;
		}
	}
}
