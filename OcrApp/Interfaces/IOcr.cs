using OcrApp.Models;
using System.Collections.Generic;

namespace OcrApp.Interfaces
{
	public interface IOcr
	{
		public bool ExtractPDF(string fileName);
		public List<ExtractBlock> GetExtractedBlocks(bool addPageSize);
	}
}
