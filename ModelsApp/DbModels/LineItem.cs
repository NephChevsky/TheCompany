using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbModels
{
	[Viewable]
	public class LineItem : IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		[IdentifierField]
		public Guid Id { get; set; }

		[IdentifierField]
		public Guid InvoiceId { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		[Extractable]
		public string Reference { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		[Extractable]
		public string Description { get; set; }

		[NumberField]
		[Viewable]
		[Editable]
		[Extractable]
		public double Quantity { get; set; }

		[NumberField]
		[Viewable]
		[Editable]
		[Extractable]
		public double UnitaryPrice { get; set; }

		[NumberField]
		[Viewable]
		[Editable]
		[Extractable]
		public double Price { get; set; }

		// IOwnable
		[IdentifierField]
		public Guid Owner { get; set; }

		// ISoftDeleteable
		[BooleanField]
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		[DateTimeField]
		public DateTime CreationDateTime { get; set; }
		[DateTimeField]
		public DateTime LastModificationDateTime { get; set; }

		public LineItem()
		{
		}

		public LineItem(Guid invoiceId, Guid owner)
		{
			InvoiceId = invoiceId;
			Owner = owner;
		}

		public LineItem(Guid invoiceId, Guid owner, string reference, string description, double quantity, double unitaryprice, double price, DateTime creationDateTime)
		{
			InvoiceId = invoiceId;
			Owner = owner;
			Reference = reference;
			Description = description;
			Quantity = quantity;
			UnitaryPrice = unitaryprice;
			Price = price;
			CreationDateTime = creationDateTime;
		}
	}
}
