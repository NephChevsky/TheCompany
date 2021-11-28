using ModelsApp.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp.DbModels
{
	[Viewable]
	public class Individual : Customer
	{
		[TextField]
		[Viewable]
		public string FirstName { get; set; }

		[TextField]
		[Viewable]
		public string LastName { get; set; }

		[TextField]
		[Viewable]
		public string Email { get; set; }

		[TextField]
		[Viewable]
		public string PhoneNumber { get; set; }

		[TextField]
		[Viewable]
		public string MobilePhoneNumber { get; set; }

		[MultilineTextField]
		[Viewable]
		public string Address { get; set; }
	}
}
