﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbApp.Models
{
	public class Individual : CustomerEntity
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string MobilePhoneNumber { get; set; }
		public Address Address { get; set; }
	}
}
