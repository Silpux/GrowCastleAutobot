using gca_clicker.Classes;
using gca_clicker.Clicker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static gca_clicker.Classes.WinAPI;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        private void Getscreen()
        {
            currentScreen = backgroundMode ? CaptureWindow(hwnd) : CaptureScreen();
        }

        private Color Pxl(int x, int y)
        {
            if(currentScreen is null)
            {
                Getscreen();
            }
            if(x < 0 || y < 0)
            {
                Log.E($"Wrong coordinates to take pixel: ({x}, {y})");
                return Color.Black;
            }
            if (x >= currentScreen!.Width || y >= currentScreen.Height)
            {
                Log.E($"Wrong coordinates: ({x}, {y}). Size of current bitmap: ({currentScreen.Width}, {currentScreen.Height})");

                if (!CheckNoxState())
                {
                    Log.E($"Getscreen again");
                    Getscreen();
                    if(x >= currentScreen.Width || y >= currentScreen.Height)
                    {
                        Log.E($"Point is still outside of bounds");
                        return Color.Black;
                    }
                }
                else
                {
                    Log.E($"Nox was not minimized");
                    return Color.Black;
                }

            }
            return currentScreen.GetPixel(x, y);

        }

        public static bool AreColorsSimilar(Color c1, Color c2, int tolerance = 3)
        {
            return Math.Abs(c1.R - c2.R) <= tolerance &&
            Math.Abs(c1.G - c2.G) <= tolerance &&
            Math.Abs(c1.B - c2.B) <= tolerance;
        }

        public static void Screenshot(Bitmap bitmap, string relativePath)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(baseDir, relativePath);
            string directory = Path.GetDirectoryName(fullPath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

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

            finalPath = Path.GetFullPath(finalPath);
            Log.I($"Save screenshot \"{finalPath}\"");
            bitmap.Save(finalPath);
        }

        /// <summary>
        /// Shortcut to Color.FromArgb()
        /// </summary>
        private Color Col(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Move cursor to coords
        /// </summary>
        private void Move(int x, int y)
        {
            if (!backgroundMode)
            {
                WinAPI.SetCursorPos(x, y);
            }
        }

        private void LClick(int x, int y)
        {
            if (backgroundMode)
            {
                LeftClickBackground(hwnd, x, y);
            }
            else
            {
                LeftClick(x, y);
            }
        }

        private void DblClick(int x, int y)
        {
            if (backgroundMode)
            {
                LeftClickBackground(hwnd, x, y);
                Wait(70);
                LeftClickBackground(hwnd, x, y);
            }
            else
            {
                LeftClick(x, y);
                Wait(70);
                LeftClick(x, y);
            }
        }

        private void RClick(int x, int y)
        {
            if (backgroundMode)
            {
                RightClickBackground(hwnd, x, y);
            }
            else
            {
                RightClick(x, y);
            }
        }

        private void RandomClickIn(int x1, int y1, int x2, int y2)
        {
            LClick(x1 + (int)((x2 - x1) * rand.NextDouble()), y1 + (int)((y2 - y1) * rand.NextDouble()));
        }

        private void RandomMoveIn(int x1, int y1, int x2, int y2)
        {
            Move(x1 + (int)((x2 - x1) * rand.NextDouble()), y1 + (int)((y2 - y1) * rand.NextDouble()));
        }

        private void RandomDblClickIn(int x1, int y1, int x2, int y2)
        {
            DblClick(x1 + (int)((x2 - x1) * rand.NextDouble()), y1 + (int)((y2 - y1) * rand.NextDouble()));
        }

        private void RandomWait(int min, int max)
        {
            int wait = rand.Next(min, max + 1);
            Log.T($"Wait {wait} ms.");
            Wait(wait);
        }


        public bool PixelIn(int x1, int y1, int x2, int y2, Color color, out (int x, int y) ret)
        {
            for (int j = y1; j <= (y2 < currentScreen.Height - 1 ? y2 : currentScreen.Height - 1); j++)
            {
                for (int i = x1; i <= (x2 < currentScreen.Width - 1 ? x2 : currentScreen.Width - 1); i++)
                {
                    if (currentScreen.GetPixel(i, j).ToArgb() == color.ToArgb())
                    {
                        ret.x = i;
                        ret.y = j;
                        return true;
                    }
                }
            }

            ret.x = -1;
            ret.y = -1;
            return false;
        }

        public bool PixelIn(int x1, int y1, int x2, int y2, Color color)
        {
            for (int j = y1; j <= (y2 < currentScreen.Height - 1 ? y2 : currentScreen.Height - 1); j++)
            {
                for (int i = x1; i <= (x2 < currentScreen.Width - 1 ? x2 : currentScreen.Width - 1); i++)
                {
                    if (currentScreen.GetPixel(i, j).ToArgb() == color.ToArgb())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public int PxlCount(int x1, int y1, int x2, int y2, Color targetColor)
        {
            int count = 0;
            for (int i = x1; i <= (x2 < currentScreen.Width - 1 ? x2 : currentScreen.Width - 1); i++)
            {
                for (int j = y1; j <= (y2 < currentScreen.Height - 1 ? y2 : currentScreen.Height - 1); j++)
                {
                    if (currentScreen.GetPixel(i, j).ToArgb() == targetColor.ToArgb())
                    {
                        count++;
                    }
                }
            }

            return count;
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


    }
}
