using IronOcr;
using OcrApp.Interfaces;
using OcrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OcrApp.Workers
{
	public class IronOcrWrapper : IOcr
	{
		private bool Deskew { get; set; }
		private bool Denoise { get; set; }
		private ExtractBlock PageSize { get; set; }
		private List<ExtractBlock> ExtractedBlocks { get; set; }

		public IronOcrWrapper(bool deskew, bool denoise)
		{
			Deskew = deskew;
			Denoise = denoise;
		}

		public bool ExtractPDF(string filePath)
		{
			ExtractedBlocks = new List<ExtractBlock>();
			IronTesseract Ocr = new IronTesseract();
			using (OcrInput Input = new OcrInput(filePath))
			{
				if (Deskew)
					Input.Deskew();
				if (Denoise)
					Input.DeNoise();
				OcrResult result = Ocr.Read(Input);
				PageSize = new ExtractBlock(result.Pages[0].ContentArea.X, result.Pages[0].ContentArea.Y, result.Pages[0].ContentArea.Height, result.Pages[0].ContentArea.Width, null);
				Array.ForEach(result.Words, delegate (OcrResult.Word word)
				{
					ExtractBlock extractBlock = new ExtractBlock(word.X, word.Y, word.Height, word.Width, word.Text);
					ExtractedBlocks.Add(extractBlock);
				});
			}
			return true;
		}

		public List<ExtractBlock> GetExtractedBlocks(bool addPageSize)
		{
			List<ExtractBlock> result = ExtractedBlocks.ToList();
			if (addPageSize)
			{
				result.Insert(0, PageSize);
			}
			return result;
		}
	}
}
