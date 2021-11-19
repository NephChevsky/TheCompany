using OcrApp.Models;
using System.Collections.Generic;
using System.Drawing;

namespace OcrApp.Interfaces
{
	public interface IOcr
	{
		public bool ExtractPDF(string fileName);
		public bool ExtractTextInArea(Rectangle rectangle, out string text);
		public bool ExtractLinesInArea(Rectangle rectangle, out List<ExtractBlock> result);
		public bool GetExtractedBlocks(out List<ExtractBlock> result);
	}
}
