using gca_clicker.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
            if (currentScreen != null)
            {
                if(x < 0 || x > currentScreen.Width || y < 0 || y > currentScreen.Height)
                {
                    Debug.WriteLine($"Wrong coordinates: ({x}, {y}). Size of current bitmap: ({currentScreen.Width}, {currentScreen.Height})");
                    return Color.Black;
                }
                return currentScreen.GetPixel(x, y);
            }
            Debug.WriteLine($"Current bitmap is null");
            return Color.Black;
        }

        private void Move(int x, int y)
        {
            WinAPI.SetCursorPos(x, y);
        }

        private void Lclick(int x, int y)
        {
            if (backgroundMode)
            {
                ClickBackground(hwnd, x, y);
            }
            else
            {
                Click(x, y);
            }
        }

        private void DblClick(int x, int y)
        {
            if (backgroundMode)
            {
                ClickBackground(hwnd, x, y);
                Wait(70);
                ClickBackground(hwnd, x, y);
            }
            else
            {
                Click(x, y);
                Wait(70);
                Click(x, y);
            }
        }

        private void Rclick(int x, int y)
        {
            if (backgroundMode)
            {
                RClickBackground(hwnd, x, y);
            }
            else
            {
                RClick(x, y);
            }
        }

        private void RandomClickIn(int x1, int y1, int x2, int y2)
        {
            Random random = new Random();
            Lclick(x1 + (int)((x2 - x1) * random.NextDouble()), y1 + (int)((y2 - y1) * random.NextDouble()));
        }

        private void RandomMoveIn(int x1, int y1, int x2, int y2)
        {
            Random random = new Random();
            Move(x1 + (int)((x2 - x1) * random.NextDouble()), y1 + (int)((y2 - y1) * random.NextDouble()));
        }

        private void RandomDblClickIn(int x1, int y1, int x2, int y2)
        {
            Random random = new Random();
            DblClick(x1 + (int)((x2 - x1) * random.NextDouble()), y1 + (int)((y2 - y1) * random.NextDouble()));
        }

        public bool PixelIn(int x1, int y1, int x2, int y2, Color targetColor, out (int x, int y) ret)
        {
            for (int i = x1; i <= (x2 < currentScreen.Width - 1 ? x2 : currentScreen.Width - 1); i++)
            {
                for (int j = y1; j <= (y2 < currentScreen.Height - 1 ? y2 : currentScreen.Height - 1); j++)
                {
                    if (currentScreen.GetPixel(i, j).ToArgb() == targetColor.ToArgb())
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


        public int PixelCount(int x1, int y1, int x2, int y2, Color targetColor)
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




    }
}
