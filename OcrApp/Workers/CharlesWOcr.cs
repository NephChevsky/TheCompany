using OcrApp.Interfaces;
using OcrApp.Models;
using System.Collections.Generic;
using System.Drawing;

namespace OcrApp.Workers
{
	public class CharlesWWrapper : IOcr
	{
		public CharlesWWrapper()
		{

		}

		public bool ExtractPDF(string fileName)
		{
			throw new System.NotImplementedException();
		}

		public bool ExtractTextInArea(Rectangle rectangle, out string text)
		{
			throw new System.NotImplementedException();
		}

		public bool ExtractLinesInArea(Rectangle rectangle, out List<ExtractBlock> result)
		{
			throw new System.NotImplementedException();
		}

		public bool GetExtractedBlocks(out List<ExtractBlock> result)
		{
			throw new System.NotImplementedException();
		}
	}
}
