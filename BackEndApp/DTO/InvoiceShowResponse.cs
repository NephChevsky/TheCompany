using ModelsApp.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class InvoiceShowResponse
	{
		public string InvoiceNumber { get; set; }
		public string CustomerNumber { get; set; }
		public string CustomerAddress { get; set; }
		public DateTime CreationDateTime { get; set; }

		public static implicit operator InvoiceShowResponse(Invoice invoice)
		{
			InvoiceShowResponse result = new InvoiceShowResponse();
			result.InvoiceNumber = invoice.InvoiceNumber;
			result.CustomerNumber = invoice.CustomerNumber;
			result.CustomerAddress = invoice.CustomerAddress;
			result.CreationDateTime = invoice.CreationDateTime;
			return result;
		}
	}
}
