using System.Drawing.Imaging;
using System.IO;

namespace gca.Classes
{
    public class ScreenshotCache
    {
        private readonly Queue<(DateTime timespamp, byte[] image)> imageQueue = new();
        private readonly long jpegQuality;

        private TimeSpan cacheInterval;
        private TimeSpan cacheDuration;
        private DateTime lastScreenshot = DateTime.MinValue;

        private int maxCount = 3000;

        public ScreenshotCache(int maxSec = 60, long quality = 20L, int cacheIntervalMs = 200)
        {
            quality = Math.Max(Math.Min(100, quality), 0);
            cacheDuration = TimeSpan.FromSeconds(maxSec);
            jpegQuality = quality;
            cacheInterval = TimeSpan.FromMilliseconds(cacheIntervalMs);
        }

        public void AddScreenshot(Bitmap bmp, bool saveAnyways = false, long overrideQuality = -1L)
        {

            DateTime dt = DateTime.Now;
            if (!saveAnyways && dt - cacheInterval < lastScreenshot)
            {
                return;
            }

            byte[] compressedImage = CompressToJpeg(bmp, overrideQuality == -1L ? jpegQuality : overrideQuality);
            lastScreenshot = DateTime.Now;

            imageQueue.Enqueue((lastScreenshot, compressedImage));
            CleanupOldScreenshots(lastScreenshot);

        }
        private void CleanupOldScreenshots(DateTime currentTime)
        {
            while (imageQueue.Count > 0 && (currentTime - imageQueue.Peek().timespamp) > cacheDuration || imageQueue.Count > maxCount)
            {
                imageQueue.Dequeue();
            }
        }

        public static byte[] CompressToJpeg(Bitmap bmp, long quality)
        {
            using var ms = new MemoryStream();

            var encoder = ImageCodecInfo.GetImageDecoders()
                .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            bmp.Save(ms, encoder, encoderParams);
            return ms.ToArray();
        }

        public List<Bitmap> GetBitmaps()
        {
            var result = new List<Bitmap>();
            foreach (var imgBytes in imageQueue)
            {
                using var ms = new MemoryStream(imgBytes.image);
                result.Add(new Bitmap(ms));
            }
            return result;
        }

        public void SaveAllToFolder(string baseFolderPath)
        {
            string targetFolder = GetUniqueFolderName(baseFolderPath);

            Log.I($"Save screenshots cache ({imageQueue.Count} images) to \"{targetFolder}\\\"");

            Directory.CreateDirectory(targetFolder);

            int index = 0;
            foreach (var imgBytes in imageQueue)
            {
                string filePath = Path.Combine(targetFolder, $"img_{index}_({imgBytes.timespamp:HH.mm.ss.fff}).jpg");
                File.WriteAllBytes(filePath, imgBytes.image);
                index++;
            }
        }

        private string GetUniqueFolderName(string basePath)
        {
            string folderPath = basePath;
            int suffix = 1;
            while (Directory.Exists(folderPath))
            {
                folderPath = basePath + "_" + suffix;
                suffix++;
            }
            return Path.GetFullPath(folderPath);
        }
    }
}
