using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp
{
	public class InvoiceLineItem : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		public Guid Id { get; set; }
		public Guid InvoiceId { get; set; }
		public string Reference { get; set; }
		public string Description { get; set; }
		public double Quantity { get; set; }
		public double UnitaryPrice { get; set; }
		public double Price { get; set; }

		// IOwnable
		public Guid Owner { get; set; }

		// ISoftDeleteable
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		public DateTime CreationDateTime { get; set; }
		public DateTime LastModificationDateTime { get; set; }
	}
}
