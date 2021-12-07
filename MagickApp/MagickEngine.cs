using ImageMagick;
using System;
using System.IO;

namespace MagickApp
{
	public class MagickEngine
	{
        public bool GrayScale { get; set; } = false;
        public bool Dilate { get; set; } = false;
        public bool Erode { get; set; } = false;
        public bool GaussianBlur { get; set; } = false;

        public MemoryStream ConvertToPng(MemoryStream file, int page)
		{
            // TODO : use GS dll instead
            //MagickNET.SetGhostscriptDirectory(@"C:\MyProgram\Ghostscript");
            //MagickNET.SetTempDirectory(@"C:\MyProgram\MyTempFiles");
            var settings = new MagickReadSettings();
            settings.FrameIndex = page-1;
            settings.Density = new Density(300, 300);
            if (GrayScale)
			{
                settings.ColorSpace = ColorSpace.Gray;
            }

            using (var image = new MagickImage())
            {
                file.Position = 0;
                image.Read(file, settings);
                if (Erode)
                {
                    image.Morphology(MorphologyMethod.Erode, Kernel.Octagon, "1");
                }
                if (Dilate)
                {
                    image.Morphology(MorphologyMethod.Dilate, Kernel.Octagon, "1");
                }
                if (GaussianBlur)
                {
                    image.GaussianBlur(5);
                }
                MemoryStream stream = new MemoryStream();
                image.Write(stream, MagickFormat.Png);
                return stream;
            }
        }
	}
}
