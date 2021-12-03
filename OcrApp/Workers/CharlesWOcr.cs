using MagickApp;
using OcrApp.Interfaces;
using OcrApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tesseract;

namespace OcrApp.Workers
{
	public class CharlesWWrapper : IOcr
	{
        private string ModelsPath;
        private ExtractBlock PageSize { get; set; }
        public List<ExtractBlock> ExtractedBlocks { get; set; }

        public CharlesWWrapper(string modelsPath)
		{
            ModelsPath = modelsPath;

        }

		public bool ExtractPDF(string fileName)
		{
            bool debugMode = false;
#if DEBUG
            debugMode = true;
#endif
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            MemoryStream ms = new MemoryStream();
            fs.CopyTo(ms);
            MagickEngine magickEngine = new MagickEngine();
            magickEngine.GrayScale = true;
            magickEngine.Erode = true;
            magickEngine.Dilate = true;
            magickEngine.GaussianBlur = true;
            MemoryStream output = magickEngine.ConvertToPng(ms, 1);
            using (TesseractEngine engine = new TesseractEngine(ModelsPath, "fra", EngineMode.Default))
            {
                engine.SetVariable("image_default_resolution", 300);
                if (debugMode)
				{
                    engine.SetVariable("tessedit_write_images", true);
                }
                using (Pix img = Pix.LoadFromMemory(output.ToArray()))
                {
                    using (Page page = engine.Process(img))
                    {
                        ExtractedBlocks = new List<ExtractBlock>();
                        PageSize = new ExtractBlock(page.RegionOfInterest.X1, page.RegionOfInterest.Y1, page.RegionOfInterest.Height, page.RegionOfInterest.Width, null);
                        using (var iter = page.GetIterator())
                        {
                            iter.Begin();
                            do
                            {
                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            Rect boundingBox;
                                            iter.TryGetBoundingBox(PageIteratorLevel.Word, out boundingBox);
                                            ExtractBlock extractBlock = new ExtractBlock(boundingBox.X1, boundingBox.Y1, boundingBox.Height, boundingBox.Width, iter.GetText(PageIteratorLevel.Word));
                                            ExtractedBlocks.Add(extractBlock);
                                        } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                                    } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                            } while (iter.Next(PageIteratorLevel.Block));
                        }
                    }
                }
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
