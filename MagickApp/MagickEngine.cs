using ImageMagick;
using System;
using System.IO;

namespace MagickApp
{
	public class MagickEngine
	{
		public byte[] ConvertToPng(MemoryStream file, int page)
		{
            // TODO : use GS dll instead
            //MagickNET.SetGhostscriptDirectory(@"C:\MyProgram\Ghostscript");
            //MagickNET.SetTempDirectory(@"C:\MyProgram\MyTempFiles");
            var settings = new MagickReadSettings();
            settings.FrameIndex = page-1;
            settings.Density = new Density(300, 300);

            using (var image = new MagickImageCollection())
            {
                file.Position = 0;
                image.Read(file, settings);
                MemoryStream stream = new MemoryStream();
                image.Write(stream, MagickFormat.Png);
                return stream.ToArray(); // TODO: should return a memory stream IMO
            }
        }
	}
}
