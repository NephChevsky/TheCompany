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
    public class Document : ILockable, IAttachment, IExtractable, IGeneratable, IOwnable, ISoftDeleteable, IDateTimeTrackable
    {
        [IdentifierField]
        public Guid Id { get; set; }

        [TextField]
        [Viewable]
        [Editable]
        [Extractable]
        public string Number { get; set; }

        [TextField]
        [Viewable]
        [Editable]
        [Extractable]
        [AutoCompletable("Individual", "CustomerNumber")]
        [Bindable("Individual", "CustomerNumber",
                    "FirstName", "RecipientFirstName",
                    "LastName", "RecipientLastName",
                    "Address", "RecipientAddress")]
        public string RecipientNumber { get; set; }

        [IdentifierField]
        public Guid RecipientId { get; set; }

        [TextField]
        [Viewable]
        [Editable]
        [Extractable]
        public string RecipientFirstName { get; set; }

        [TextField]
        [Viewable]
        [Editable]
        [Extractable]
        public string RecipientLastName { get; set; }

        [MultilineTextField]
        [Viewable]
        [Editable]
        [Extractable]
        public string RecipientAddress { get; set; }

        // ILockable
        [TextField]
        public string LockedBy { get; set; }

        // IAttachment
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

        // IGeneratable
        [BooleanField]
        public bool? ShouldBeGenerated { get; set; }

        [BooleanField]
        public bool? IsGenerated { get; set; }

        [DateTimeField]
        public DateTime GenerationDateTime { get; set; }

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
    }
}
