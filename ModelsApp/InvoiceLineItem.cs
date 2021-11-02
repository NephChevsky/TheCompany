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

		public InvoiceLineItem()
		{
		}

		public InvoiceLineItem(Guid invoiceId, Guid owner, string reference, string description, double quantity, double unitaryprice, double price)
		{
			InvoiceId = invoiceId;
			Owner = owner;
			Reference = reference;
			Description = description;
			Quantity = quantity;
			UnitaryPrice = unitaryprice;
			Price = price;
		}
	}
}
