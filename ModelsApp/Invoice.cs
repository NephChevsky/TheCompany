using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModelsApp
{
	public class Invoice : ILockable, IAttachment, IExtractable, IOwnable, ISoftDeleteable, IDateTimeTrackable
	{
		public Guid Id { get; set; }
		public string InvoiceNumber { get; set; }
		public Guid CustomerId { get; set; }
		public string CustomerAddress { get; set; }

		// ILockable
		public string LockedBy { get; set; }

		// IAttachement
		public Guid FileId { get; set; }
		public string FileName { get; set; }
		public long FileSize { get; set; }

		// IExtractable
		public bool? ShouldBeExtracted { get; set; }
		public bool? IsExtracted { get; set; }
		public Guid ExtractId { get; set; }
		public DateTime ExtractDateTime { get; set; }

		// IOwnable
		public Guid Owner { get; set; }

		// ISoftDeleteable
		public bool Deleted { get; set; }

		// IDateTimeTrackable
		public DateTime CreationDateTime { get; set; }
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
