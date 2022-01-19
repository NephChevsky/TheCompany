using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsApp.DbModels
{
    public class Quote : Document
    {
        public Quote()
        {

        }

        public Quote(Guid id, string fileName, long fileSize)
        {
            FileId = id;
            FileName = fileName;
            FileSize = fileSize;
        }
    }
}
