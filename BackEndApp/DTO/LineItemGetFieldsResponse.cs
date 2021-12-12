using Microsoft.AspNetCore.Mvc;
using ModelsApp.Models;
using System.Collections.Generic;

namespace BackEndApp.DTO
{
    public class LineItemGetFieldsResponse
    {
        public List<Field> Fields { get; set; }

        public LineItemGetFieldsResponse()
        {
            Fields = new List<Field>();
        }
    }
}
