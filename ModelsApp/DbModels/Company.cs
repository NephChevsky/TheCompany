using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbModels
{
    public class Company : IOwnable, ISoftDeleteable, IDateTimeTrackable
    {
        [IdentifierField]
        public Guid Id { get; set; }

        [TextField]
        [Viewable]
        [Editable]
        public string Name { get; set; }

        [MultilineTextField]
        [Viewable]
        [Editable]
        public string Address { get; set; }

        [TextField]
        [Viewable]
        [Editable]
        public string PhoneNumber { get; set; }

        [TextField]
        [Viewable]
        [Editable]
        public string MobilePhoneNumber { get; set; }

        [TextField]
        [Viewable]
        [Editable]
        public string Siret { get; set; }

        [FileField]
        [Viewable]
        [Editable]
        public Guid Logo { get; set; }

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
