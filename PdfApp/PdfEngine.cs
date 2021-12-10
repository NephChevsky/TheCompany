using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Drawing.Imaging;
using System.IO;

namespace PdfApp
{
    public class PdfEngine
    {
        private PdfDocument _Document;
        private PdfPage _Page;
        private XGraphics _Gfx;

        private PdfDocument Document {
            get
            {
                if (_Document == null)
                {
                    throw new Exception("No document");
                }
                return _Document;
            }
            set
            {
                _Document = value;
            }
        }
        private PdfPage Page
        {
            get
            {
                if (_Page == null)
                {
                    throw new Exception("No page selected");
                }
                return _Page;
            }
            set
            {
                _Page = value;
            }
        }
        
        private XGraphics Gfx {
            get
            {
                if (_Gfx == null)
                {
                    _Gfx = XGraphics.FromPdfPage(Page);
                }
                return _Gfx;
            }
            set
            {
                _Gfx = value;
            }
        }

        public void CreateDocument()
        {
            Document = new PdfDocument();
        }

        public void InsertPage(int index)
        {
            Page = Document.InsertPage(index);
            Gfx = null;
        }

        public void AddText(string text, double x, double y)
        {
            XFont font = new XFont("Arial", 14, XFontStyle.Regular);
            Gfx.DrawString(text, font, XBrushes.Black, new XRect(x, y, Page.Width, Page.Height), XStringFormats.TopLeft);
        }

        public void InsertImage(MemoryStream stream, double x, double y, double width, double height)
        {
            string tempFileName = Path.GetTempFileName() + ".png";
            FileStream file = new FileStream(tempFileName, FileMode.Create);
            stream.WriteTo(file);
            file.Close();
            XImage image = XImage.FromFile(tempFileName);
            Gfx.DrawImage(image, x, y, width, height);
            File.Delete(tempFileName);
        }

        public MemoryStream ToStream()
        {
            MemoryStream stream = new MemoryStream();
            Document.Save(stream, false);
            return stream;
        }
    }
}
