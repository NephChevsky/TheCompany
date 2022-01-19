﻿using ModelsApp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbModels
{
    public class Invoice : Document
    {   
        public Invoice()
        {

        }

        public Invoice(Guid id, string fileName, long fileSize)
        {
            FileId = id;
            FileName = fileName;
            FileSize = fileSize;
        }
    }
}
