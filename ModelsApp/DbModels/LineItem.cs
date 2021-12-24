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
		[AutoCompletable("LineItemDefinition", "Reference")]
		[Bindable("LineItemDefinition", "Reference", "Description", "Description", "Quantity", "Quantity", "UnitaryPrice", "UnitaryPrice", "Price", "Price")]
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

		[ComboField("kg", "m2", "m3", "L", "U", "m", "mL")]
		[Viewable]
		[Editable]
		[Extractable]
		public string Unit { get; set; }

		[NumberField]
		[Viewable]
		[Editable]
		[Extractable]
		public double? VAT { get; set; }

		[NumberField]
		[Viewable]
		[Editable]
		[Extractable]
		public double? PriceNoVAT { get; set; }

		[NumberField]
		[Viewable]
		[Editable]
		[Extractable]
		public double? PriceVAT { get; set; }

		[NumberField]
		[Viewable]
		[Editable]
		[Extractable]
		public double TotalPrice { get; set; }

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

		public LineItem(Guid invoiceId)
		{
			InvoiceId = invoiceId;
		}

		public LineItem(Guid invoiceId, string reference, string description, double quantity, string unit, double vat, double pricevat, double pricenovat, double totalprice, DateTime creationDateTime)
		{
			InvoiceId = invoiceId;
			Reference = reference;
			Description = description;
			Quantity = quantity;
			Unit = unit;
			VAT = vat;
			PriceVAT = pricevat;
			PriceNoVAT = pricenovat;
			TotalPrice = totalprice;
			CreationDateTime = creationDateTime;
		}
	}
}
