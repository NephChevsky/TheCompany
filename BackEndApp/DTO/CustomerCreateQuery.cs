using ModelsApp.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndApp.DTO
{
	public class CustomerCreateQuery
	{
		public string CustomerId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string MobilePhoneNumber { get; set; }
		public string Address { get; set; }

		public static explicit operator Individual(CustomerCreateQuery customer)
		{
			Individual newCustomer = new Individual();
			newCustomer.CustomerId = customer.CustomerId;
			newCustomer.FirstName = customer.FirstName;
			newCustomer.LastName = customer.LastName;
			newCustomer.Email = customer.Email;
			newCustomer.PhoneNumber = customer.PhoneNumber;
			newCustomer.MobilePhoneNumber = customer.MobilePhoneNumber;
			newCustomer.Address = customer.Address;
			return newCustomer;
		}
	}
}
