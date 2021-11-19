using IronOcr;
using Microsoft.Extensions.Configuration;
using OcrApp.Interfaces;
using OcrApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static IronOcr.OcrResult;

namespace OcrApp.Workers
{
	public class IronOcrWrapper : IOcr
	{
		private IConfiguration Configuration { get; }
		public List<ExtractBlock> ExtractedBlocks { get; set; }
		private OcrResult OcrResult { get; set; }
		
		public IronOcrWrapper(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public bool ExtractPDF(string filePath)
		{
			ExtractedBlocks = new List<ExtractBlock>();
			IronTesseract Ocr = new IronTesseract();
			using (OcrInput Input = new OcrInput(filePath))
			{
				//Input.Deskew();
				//Input.DeNoise();
				OcrResult = Ocr.Read(Input);
				ExtractBlock size = new ExtractBlock(OcrResult.Pages[0].ContentArea.X, OcrResult.Pages[0].ContentArea.Y, OcrResult.Pages[0].ContentArea.Height, OcrResult.Pages[0].ContentArea.Width, "");
				ExtractedBlocks.Add(size);
				Array.ForEach(OcrResult.Words, delegate (OcrResult.Word word)
				{
					ExtractBlock extractBlock = new ExtractBlock(word.X, word.Y, word.Height, word.Width, word.Text);
					ExtractedBlocks.Add(extractBlock);
				});
			}
			return true;
		}

		public bool ExtractTextInArea(Rectangle rectangle, out string text)
		{
			text = null;
			foreach (Word tmp in OcrResult.Words)
			{
				bool left = tmp.X + tmp.Width < rectangle.X;
				bool right = tmp.X > rectangle.X + rectangle.Width;
				bool above = tmp.Y > rectangle.Y + rectangle.Height;
				bool below = tmp.Y + tmp.Height < rectangle.Y;
				if (!(left || right || above || below))
				{
					if (text != null)
						text += " ";
					text += tmp.Text;
				}
			}
			return true;
		}

		public bool ExtractLinesInArea(Rectangle rectangle, out List<ExtractBlock> result)
		{
			List<ExtractBlock> currentResult = new List<ExtractBlock>();
			List<Word> list = new List<Word>();
			foreach (Word tmp in OcrResult.Words)
			{
				bool left = tmp.X + tmp.Width < rectangle.X;
				bool right = tmp.X > rectangle.X + rectangle.Width;
				bool above = tmp.Y > rectangle.Y + rectangle.Height;
				bool below = tmp.Y + tmp.Height < rectangle.Y;
				if (!(left || right || above || below))
				{
					list.Add(tmp);
				}
			}
			list.ForEach(currentBlock => {
				ExtractBlock existingBlock = currentResult.Find(aBlock => currentBlock.Y == aBlock.Y);
				if (existingBlock != null)
				{
					existingBlock.Text += " " + currentBlock.Text;
					existingBlock.Width = currentBlock.X + currentBlock.Width - existingBlock.X;
				}
				else
				{
					ExtractBlock newBlock = new ExtractBlock(currentBlock.X, currentBlock.Y, currentBlock.Height, currentBlock.Width, currentBlock.Text);
					currentResult.Add(newBlock);
				}
			});
			currentResult.OrderBy(x => x.Y);
			result = currentResult;
			return true;
		}

		public bool GetExtractedBlocks(out List<ExtractBlock> result)
		{
			result = ExtractedBlocks;
			return true;
		}
	}
}
