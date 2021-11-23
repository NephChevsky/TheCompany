using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Extensions.Configuration;
using OcrApp.Interfaces;
using OcrApp.Models;
using OcrApp.Workers;

namespace OcrApp
{
	public class Ocr : IOcr
	{
		private IConfiguration Configuration { get; }
		private IOcr Instance { get; set; }

		public Ocr(IConfiguration configuration)
		{
			Configuration = configuration;
			if (Configuration["OcrType"] == "Iron")
			{
				Instance = new IronOcrWrapper(Configuration["Deskew"] == "true", Configuration["Denoise"] == "true");
			}
			else if (Configuration["OcrType"] == "CharlesW")
			{
				Instance = new CharlesWWrapper(Configuration["ModelsPath"]);
			}
			else
			{
				throw new Exception("No Ocr type defined in configuration");
			}
		}

		public bool ExtractPDF(string fileName)
		{
			return Instance.ExtractPDF(fileName);
		}

		public bool ExtractTextInArea(Rectangle rectangle, out string text)
		{
			text = null;
			foreach (ExtractBlock tmp in Instance.GetExtractedBlocks(false))
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
			foreach (ExtractBlock tmp in Instance.GetExtractedBlocks(false))
			{
				bool left = tmp.X + tmp.Width < rectangle.X;
				bool right = tmp.X > rectangle.X + rectangle.Width;
				bool above = tmp.Y > rectangle.Y + rectangle.Height;
				bool below = tmp.Y + tmp.Height < rectangle.Y;
				if (!(left || right || above || below))
				{
					ExtractBlock existingBlock = currentResult.Find(aBlock => tmp.Y == aBlock.Y);
					if (existingBlock != null)
					{
						existingBlock.Text += " " + tmp.Text;
						existingBlock.Width = tmp.X + tmp.Width - existingBlock.X;
					}
					else
					{
						currentResult.Add(tmp);
					}
				}
			}
			currentResult.OrderBy(x => x.Y);
			result = currentResult;
			return true;
		}

		public List<ExtractBlock> GetExtractedBlocks(bool addPageSize)
		{
			return Instance.GetExtractedBlocks(addPageSize);
		}
	}
}
