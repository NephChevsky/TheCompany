using ModelsApp.Attributes;
using ModelsApp.DbInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbInterfaces
{
    public interface IGeneratable : IAttachment
    {
        [BooleanField]
        public bool? ShouldBeGenerated { get; set; }

        [BooleanField]
        public bool? IsGenerated { get; set; }

        [DateTimeField]
        public DateTime GenerationDateTime { get; set; }
    }
}
