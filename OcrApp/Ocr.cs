using System;
using System.Collections.Generic;
using System.Drawing;
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
			else if (Configuration["StorageType"] == "CharlesW")
			{
				Instance = new CharlesWWrapper();
			}
			else
			{
				throw new Exception("No storage type defined in configuration");
			}
		}

		public bool ExtractPDF(string fileName)
		{
			return Instance.ExtractPDF(fileName);
		}

		public bool ExtractTextInArea(Rectangle rectangle, out string text)
		{
			return Instance.ExtractTextInArea(rectangle, out text);
		}

		public bool ExtractLinesInArea(Rectangle rectangle, out List<ExtractBlock> result)
		{
			return Instance.ExtractLinesInArea(rectangle, out result);
		}

		public bool GetExtractedBlocks(out List<ExtractBlock> result)
		{
			return Instance.GetExtractedBlocks(out result);
		}
	}
}
