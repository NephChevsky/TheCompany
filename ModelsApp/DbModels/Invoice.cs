using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp.DbModels
{
	[Viewable]
	public class Invoice : ILockable, IAttachment, IExtractable, IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		[IdentifierField]
		public Guid Id { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		[Extractable]
		public string InvoiceNumber { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		[Extractable]
		public string CustomerNumber { get; set; }

		[IdentifierField]
		public Guid CustomerId { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		[Extractable]
		public string CustomerFirstName { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		[Extractable]
		public string CustomerLastName { get; set; }

		[TextField]
		[Viewable]
		[Editable]
		[Extractable]
		public string CustomerAddress { get; set; }

		// ILockable
		[TextField]
		public string LockedBy { get; set; }

		// IAttachement
		[IdentifierField]
		public Guid FileId { get; set; }

		[TextField]
		[Viewable]
		public string FileName { get; set; }

		[NumberField]
		public long FileSize { get; set; }

		// IExtractable
		[BooleanField]
		public bool? ShouldBeExtracted { get; set; }

		[BooleanField]
		public bool? IsExtracted { get; set; }

		[IdentifierField]
		public Guid ExtractId { get; set; }

		[DateTimeField]
		public DateTime ExtractDateTime { get; set; }

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

		public Invoice() { }

		public Invoice (Guid owner, Guid id, string fileName, long fileSize)
		{
			Owner = owner;
			FileId = id;
			FileName = fileName;
			FileSize = fileSize;
		}
	}
}
