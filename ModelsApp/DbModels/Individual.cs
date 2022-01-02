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
		[Editable]
		public string FirstName { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		public string LastName { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		public string Email { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		public string PhoneNumber { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		public string MobilePhoneNumber { get; set; }

		[MultilineTextField]
		[Viewable]
		[Editable]
		public string Address { get; set; }
	}
}
