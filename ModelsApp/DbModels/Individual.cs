using ModelsApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp.DbModels
{
	public class Individual : Customer
	{
		[TextField]
		public string FirstName { get; set; }

		[TextField]
		public string LastName { get; set; }

		[TextField]
		public string Email { get; set; }

		[TextField]
		public string PhoneNumber { get; set; }

		[TextField]
		public string MobilePhoneNumber { get; set; }

		[MultilineTextField]
		public string Address { get; set; }
	}
}
