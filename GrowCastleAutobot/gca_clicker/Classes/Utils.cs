using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using gca_clicker.Clicker;
using System.IO;
using System.Security.Cryptography;
using static gca_clicker.Classes.WinAPI;

namespace gca_clicker.Classes
{
    public static class Utils
    {
        public static TimeSpan GetRandomTimeSpan(TimeSpan min, TimeSpan max)
        {
            long minTicks = min.Ticks;
            long maxTicks = max.Ticks;
            long range = maxTicks - minTicks;

            long randomTicks = minTicks + (long)(new Random().NextDouble() * range);
            return TimeSpan.FromTicks(randomTicks);
        }
        public static bool AreColorsSimilar(Color c1, Color c2, int tolerance = 3)
        {
            return Math.Abs(c1.R - c2.R) <= tolerance &&
            Math.Abs(c1.G - c2.G) <= tolerance &&
            Math.Abs(c1.B - c2.B) <= tolerance;
        }

        /// <summary>
        /// input full path
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetAvailableFilePath(string fullPath)
        {
            fullPath = Path.GetFullPath(fullPath);
            string directory = Path.GetDirectoryName(fullPath);

            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string pathWithoutExt = Path.Combine(directory, fileName);
            string finalPath = fullPath;
            int counter = 1;
            while (File.Exists(finalPath))
            {
                finalPath = $"{pathWithoutExt}_{counter}{extension}";
                counter++;
            }
            return Path.GetFullPath(finalPath);
        }

        public static string GetFullPathAndCreate(string relativePath)
        {
            string fullPath = Path.GetFullPath(relativePath);
            string directory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return fullPath;
        }

        public static string GetFullPath(string relativePath)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.GetFullPath(Path.Combine(baseDir, relativePath));
            return fullPath;
        }

        public static void CreateDirectoryForFile(string fullPath)
        {
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static string ScreenshotJpg(Bitmap bitmap, string relativePath, int quality)
        {
            quality = Math.Max(Math.Min(100, quality), 0);

            string fullPath = GetFullPath(relativePath);
            CreateDirectoryForFile(fullPath);
            string finalPath = GetAvailableFilePath(relativePath);

            Log.I($"Save screenshot jpg \"{finalPath}\"");
            byte[] jpeg = ScreenshotCache.CompressToJpeg(bitmap, quality);

            File.WriteAllBytes(finalPath, jpeg);

            return finalPath;
        }

        public static string Screenshot(Bitmap bitmap, string relativePath)
        {
            string fullPath = GetFullPath(relativePath);
            CreateDirectoryForFile(fullPath);
            string finalPath = GetAvailableFilePath(relativePath);

            Log.I($"Save screenshot \"{finalPath}\"");
            bitmap.Save(finalPath);
            return finalPath;
        }

        /// <summary>
        /// Shortcut to Color.FromArgb()
        /// </summary>
        public static Color Col(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        public static int GetSlotIndex((int y, int x) matrixPosition)
        {
            if (matrixPosition.x == 0)
            {
                if (matrixPosition.y == 1) return 12;
                else if (matrixPosition.y == 3) return 13;
                else return 14;
            }
            return matrixPosition.x - 1 + matrixPosition.y * 3;
        }

        public static (int y, int x) GetMatrixPosition(int slotIndex)
        {
            return slotIndex switch
            {
                0 => (0, 1),
                1 => (0, 2),
                2 => (0, 3),
                3 => (1, 1),
                4 => (1, 2),
                5 => (1, 3),
                6 => (2, 1),
                7 => (2, 2),
                8 => (2, 3),
                9 => (3, 1),
                10 => (3, 2),
                11 => (3, 3),
                12 => (1, 0),
                13 => (3, 0),
                14 => (4, 0),
                _ => (0, 0)
            };
        }

        public static (PointF leftUp, PointF rightDown) GetBoundsWithPadding(PointF p1, PointF p2, float padding)
        {
            float minX = Math.Min(p1.X, p2.X) - padding;
            float maxX = Math.Max(p1.X, p2.X) + padding;
            float minY = Math.Min(p1.Y, p2.Y) - padding;
            float maxY = Math.Max(p1.Y, p2.Y) + padding;

            PointF topLeft = new PointF(minX, maxY);
            PointF bottomRight = new PointF(maxX, minY);

            return (topLeft, bottomRight);
        }

        public static bool BitmapsEqual(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Size != bmp2.Size) return false;

            var bd1 = bmp1.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp1.Size), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            var bd2 = bmp2.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp2.Size), ImageLockMode.ReadOnly, bmp2.PixelFormat);

            try
            {
                unsafe
                {
                    byte* p1 = (byte*)bd1.Scan0;
                    byte* p2 = (byte*)bd2.Scan0;
                    int bytes = Math.Abs(bd1.Stride) * bd1.Height;

                    for (int i = 0; i < bytes; i++)
                    {
                        if (p1[i] != p2[i]) return false;
                    }
                }
            }
            finally
            {
                bmp1.UnlockBits(bd1);
                bmp2.UnlockBits(bd2);
            }

            return true;
        }

        public static void CopyBitmap(Bitmap source, Bitmap destination)
        {
            using Graphics g = Graphics.FromImage(destination);
            g.DrawImageUnscaled(source, System.Drawing.Point.Empty);
        }


        public static bool AllBitmapsEqual(List<ScreenshotEntry> history)
        {
            byte[] first = history[0].Hash;
            for (int i = 1; i < history.Count; i++)
            {
                if (!ByteArraysEqual(first, history[i].Hash))
                    return false;
            }
            return true;
        }

        public static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        }


        public static byte[] BmpHash(Bitmap bitmap)
        {
            using (var md5 = MD5.Create())
            {
                BitmapData data = null!;
                try
                {
                    data = bitmap.LockBits(
                        new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb
                    );

                    int width = bitmap.Width;
                    int height = bitmap.Height;
                    int stride = data.Stride;

                    List<byte> includedPixels = new(width * height * 4);

                    unsafe
                    {
                        byte* ptr = (byte*)data.Scan0;
                        for (int y = 0; y < height; y+=3)
                        {
                            for (int x = 0; x < width; x+=3)
                            {
                                if (x < 75 && y < 75)
                                {
                                    continue;
                                }

                                int index = y * stride + x * 4;
                                includedPixels.Add(ptr[index]);
                                includedPixels.Add(ptr[index + 1]);
                                includedPixels.Add(ptr[index + 2]);
                                includedPixels.Add(ptr[index + 3]);
                            }
                        }
                    }
                    return md5.ComputeHash(includedPixels.ToArray());
                }
                finally
                {
                    if (data != null)
                    {
                        bitmap.UnlockBits(data);
                    }
                }
            }
        }



        public static byte[] BitmapsToByteArray(List<Bitmap> bitmaps, out int count, out int width, out int height, out int channels)
        {
            if (bitmaps.Count == 0) throw new ArgumentException("Empty bitmap array");
            count = bitmaps.Count;

            width = bitmaps[0].Width;
            height = bitmaps[0].Height;
            channels = System.Drawing.Image.GetPixelFormatSize(bitmaps[0].PixelFormat) / 8;
            int imageSize = width * height * channels;
            byte[] buffer = new byte[bitmaps.Count * imageSize];

            for (int i = 0; i < bitmaps.Count; i++)
            {
                Bitmap bmp = bitmaps[i];
                var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

                int srcStride = bmpData.Stride;
                IntPtr srcScan0 = bmpData.Scan0;
                int rowLength = width * channels;
                int dstOffset = i * imageSize;

                unsafe
                {
                    byte* srcPtr = (byte*)srcScan0;
                    for (int y = 0; y < height; y++)
                    {
                        Marshal.Copy(new IntPtr(srcPtr + y * srcStride), buffer, dstOffset + y * rowLength, rowLength);
                    }
                }

                bmp.UnlockBits(bmpData);
            }

            return buffer;
        }

        public static Bitmap Colormode(int mode, Bitmap src)
        {
            int colorShift = 1 << mode;
            int mask = 0xFF << mode;

            Bitmap result = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);

            Rectangle rect = new Rectangle(0, 0, src.Width, src.Height);
            BitmapData srcData = src.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dstData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;
                int stride = srcData.Stride;

                for (int y = 0; y < src.Height; y++)
                {
                    byte* srcRow = srcPtr + y * stride;
                    byte* dstRow = dstPtr + y * stride;

                    for (int x = 0; x < src.Width; x++)
                    {
                        int index = x * 3;

                        dstRow[index + 0] = (byte)(((srcRow[index + 0] + colorShift) & mask) - 1);
                        dstRow[index + 1] = (byte)(((srcRow[index + 1] + colorShift) & mask) - 1);
                        dstRow[index + 2] = (byte)(((srcRow[index + 2] + colorShift) & mask) - 1);
                    }
                }
            }

            src.UnlockBits(srcData);
            result.UnlockBits(dstData);

            return result;
        }

        public static Bitmap Colormode(int mode, int x1, int y1, int x2, int y2, Bitmap src)
        {
            int colorShift = 1 << mode;
            int mask = 0xFF << mode;

            x1 = Math.Max(0, Math.Min(src.Width - 1, x1));
            x2 = Math.Max(0, Math.Min(src.Width - 1, x2));
            y1 = Math.Max(0, Math.Min(src.Height - 1, y1));
            y2 = Math.Max(0, Math.Min(src.Height - 1, y2));

            //if (x1 > x2) (x1, x2) = (x2, x1);
            //if (y1 > y2) (y1, y2) = (y2, y1);

            Bitmap result = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, src.Width, src.Height);
            BitmapData srcData = src.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dstData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;
                int stride = srcData.Stride;

                for (int y = 0; y < src.Height; y++)
                {
                    byte* srcRow = srcPtr + y * stride;
                    byte* dstRow = dstPtr + y * stride;

                    for (int x = 0; x < src.Width; x++)
                    {
                        int index = x * 3;

                        byte b = srcRow[index];
                        byte g = srcRow[index + 1];
                        byte r = srcRow[index + 2];

                        if (x >= x1 && x <= x2 && y >= y1 && y <= y2)
                        {
                            b = (byte)(((b + colorShift) & mask) - 1);
                            g = (byte)(((g + colorShift) & mask) - 1);
                            r = (byte)(((r + colorShift) & mask) - 1);
                        }

                        dstRow[index] = b;
                        dstRow[index + 1] = g;
                        dstRow[index + 2] = r;
                    }
                }
            }

            src.UnlockBits(srcData);
            result.UnlockBits(dstData);

            return result;
        }
        public static Bitmap CropBitmap(Bitmap source, int x1, int y1, int x2, int y2)
        {
            int width = x2 - x1;
            int height = y2 - y1;

            Rectangle cropRect = new Rectangle(x1, y1, width, height);
            Bitmap target = source.Clone(cropRect, source.PixelFormat);
            return target;
        }

        public static (int x, int y, int width, int height) GetWindowInfo(IntPtr hWnd)
        {
            if (GetWindowRect(hWnd, out RECT rect))
            {
                int x = rect.Left;
                int y = rect.Top;
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                return (x, y, width, height);
            }

            throw new Exception("Failed to get window rectangle");
        }

        public static void InsertLine(string filePath, int lineNumber, string line)
        {

            List<string> lines = new List<string>(File.ReadAllLines(filePath));

            if (lineNumber > 0 && lineNumber <= lines.Count)
            {
                lines.Insert(lineNumber, line);
                File.WriteAllLines(filePath, lines);
            }
            else
            {
                Log.E("Invalid index.");
            }
        }

        public static void RemoveLine(string filePath, int lineNumber)
        {

            List<string> lines = new List<string>(File.ReadAllLines(filePath));

            if (lineNumber > 0 && lineNumber <= lines.Count)
            {
                lines.RemoveAt(lineNumber - 1);
                File.WriteAllLines(filePath, lines);
            }
            else
            {
                Log.E("Invalid index.");
            }
        }

        public static void ReplaceLine(string filePath, int lineNumber, string newLine)
        {
            List<string> lines = new List<string>(File.ReadAllLines(filePath));

            if (lineNumber > 0 && lineNumber <= lines.Count)
            {
                lines[lineNumber - 1] = newLine;
                File.WriteAllLines(filePath, lines);
            }
            else
            {
                throw new IndexOutOfRangeException("Line number is higher than file has, or less than 0");
            }
        }

        public static string GetLine(string filePath, int lineNumber)
        {
            return File.ReadLines(Cst.DUNGEON_STATISTICS_PATH).Skip(lineNumber - 1).FirstOrDefault()!;
        }

        public static bool[,] MatrixCopy(bool[,] matrix)
        {
            bool[,] clone = new bool[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    clone[i, j] = matrix[i, j];
                }
            }
            return clone;
        }


        public static float Distance(PointF a, PointF b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static Dictionary<T, List<T>> GetNeighbors<T>(T[,] matrix, bool includeDiagonals) where T : notnull
        {
            Dictionary<T, List<T>> result = new();
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    T current = matrix[i, j];
                    List<T> neighbors = new();

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            if (!includeDiagonals && Math.Abs(dx) + Math.Abs(dy) > 1) continue;

                            int ni = i + dx;
                            int nj = j + dy;

                            if (ni >= 0 && ni < rows && nj >= 0 && nj < cols)
                            {
                                neighbors.Add(matrix[ni, nj]);
                            }
                        }
                    }

                    result[current] = neighbors;
                }
            }

            return result;
        }

    }
}
