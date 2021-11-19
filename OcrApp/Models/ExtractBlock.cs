using System;
using System.Collections.Generic;
using System.Text;

namespace OcrApp.Models
{
	public class ExtractBlock
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		public string Text { get; set; }

		public ExtractBlock()
		{

		}

		public ExtractBlock(int x, int y, int height, int width, string text)
		{
			X = x;
			Y = y;
			Height = height;
			Width = width;
			Text = text;
		}
	}
}
