using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.IO;

namespace PdfApp
{
    public class PdfEngine
    {
        public PdfDocument Document {get; set;}
        public PdfPage Page { get; set; }

        public bool CreateDocument()
        {
            Document = new PdfDocument();
            return true;
        }

        public bool InsertPage(int index)
        {
            Page = Document.InsertPage(index);
            return true;
        }

        public bool AddText(string text, double x, double y)
        {
            XGraphics gfx = XGraphics.FromPdfPage(Page);
            XFont font = new XFont("Arial", 14, XFontStyle.Regular);
            gfx.DrawString(text, font, XBrushes.Black, new XRect(x, y, Page.Width, Page.Height), XStringFormats.TopLeft);
            return true;
        }

        public MemoryStream ToStream()
        {
            MemoryStream stream = new MemoryStream();
            Document.Save(stream, false);
            return stream;
        }
    }
}
