using gca_clicker.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        public static void LeftClick(int x, int y)
        {
            WinAPI.SetCursorPos(x, y);
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, UIntPtr.Zero);
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, UIntPtr.Zero);
        }

        public static void RightClick(int x, int y)
        {
            WinAPI.SetCursorPos(x, y);
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_RIGHTDOWN, (uint)x, (uint)y, 0, UIntPtr.Zero);
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_RIGHTUP, (uint)x, (uint)y, 0, UIntPtr.Zero);
        }

        public void LeftClickBackground(IntPtr hWnd, int x, int y)
        {
            IntPtr lParam = MakeLParam(x, y);
            WinAPI.SendMessage(hWnd, WinAPI.WM_LBUTTONDOWN, (IntPtr)1, lParam);
            WinAPI.SendMessage(hWnd, WinAPI.WM_LBUTTONUP, (IntPtr)0, lParam);
        }

        public void RightClickBackground(IntPtr hWnd, int x, int y)
        {
            IntPtr lParam = MakeLParam(x, y);
            WinAPI.SendMessage(hWnd, WinAPI.WM_RBUTTONDOWN, (IntPtr)2, lParam);
            WinAPI.SendMessage(hWnd, WinAPI.WM_RBUTTONUP, (IntPtr)0, lParam);
        }

        public IntPtr MakeLParam(int x, int y)
        {
            return (IntPtr)((y << 16) | (x & 0xFFFF));
        }

        private void SendKey(byte keyCode)
        {
            WinAPI.keybd_event(keyCode, 0, WinAPI.KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            Thread.Sleep(50);
            WinAPI.keybd_event(keyCode, 0, WinAPI.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private Bitmap CaptureArea(int x, int y, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
            }
            return bmp;
        }

        private Bitmap CaptureScreen()
        {
            var screenWidth = (int)SystemParameters.PrimaryScreenWidth;
            var screenHeight = (int)SystemParameters.PrimaryScreenHeight;

            Bitmap bmp = new Bitmap(screenWidth, screenHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            }
            return bmp;
        }

        private nint WndFind(string windowName)
        {
            return WinAPI.FindWindow(null, windowName);
        }

        private Bitmap CaptureWindowByTitle(string windowTitle)
        {
            IntPtr hWnd = WinAPI.FindWindow(null, windowTitle);
            if (hWnd == IntPtr.Zero)
            {
                throw new Exception("Window not found");
            }

            WinAPI.GetWindowRect(hWnd, out WinAPI.RECT rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics gfxBmp = Graphics.FromImage(bmp))
            {
                IntPtr hdcBitmap = gfxBmp.GetHdc();
                bool succeeded = WinAPI.PrintWindow(hWnd, hdcBitmap, 0);
                gfxBmp.ReleaseHdc(hdcBitmap);

                if (!succeeded)
                {
                    bmp.Dispose();
                    throw new Exception("PrintWindow failed");
                }
            }

            return bmp;
        }

        private Bitmap CaptureWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                throw new ArgumentException("Invalid HWND");

            if (!WinAPI.GetWindowRect(hWnd, out WinAPI.RECT rect))
                throw new Exception("Could not get window bounds");

            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics gfxBmp = Graphics.FromImage(bmp))
            {
                IntPtr hdcBitmap = gfxBmp.GetHdc();

                bool succeeded = WinAPI.PrintWindow(hWnd, hdcBitmap, 0);
                gfxBmp.ReleaseHdc(hdcBitmap);

                if (!succeeded)
                {
                    bmp.Dispose();
                    throw new Exception("PrintWindow failed");
                }
            }

            return bmp;
        }

        private void SetPosButtonClick(object sender, RoutedEventArgs e)
        {

            InfoLabel.Content = "";
            IntPtr hwnd = WinAPI.FindWindow(null, WindowName.Text);
            if (hwnd != IntPtr.Zero)
            {
                SetDefaultNoxState(hwnd);
            }
            else
            {
                InfoLabel.Content = "Cannot find window";
            }
        }

        private void SaveWindowScreenClick(object sender, RoutedEventArgs e)
        {
            InfoLabel.Content = "";
            IntPtr hwnd = WinAPI.FindWindow(null, WindowName.Text);
            if (hwnd != IntPtr.Zero)
            {

                Bitmap bmp = CaptureWindow(hwnd);
                bmp.Save("WindowScreenshot.png", ImageFormat.Png);
                bmp.Dispose();

            }
            else
            {
                InfoLabel.Content = "Cannot find window";
            }

        }

        private void SaveScreenshotClick(object sender, RoutedEventArgs e)
        {
            InfoLabel.Content = "";
            Bitmap bmp = CaptureScreen();
            bmp.Save("Screenshot.png", ImageFormat.Png);
            bmp.Dispose();
        }



        private void GetscreenBenchmark(object sender, RoutedEventArgs e)
        {
            InfoLabel.Content = "";
            IntPtr hwnd = WinAPI.FindWindow(null, WindowName.Text);
            if (hwnd != IntPtr.Zero)
            {

                Stopwatch sw = Stopwatch.StartNew();

                for (int i = 0; i < 100; i++)
                {
                    Bitmap bmp = CaptureWindow(hwnd);
                }
                sw.Stop();

                InfoLabel.Content = $"Avg time: {(float)sw.ElapsedMilliseconds / 100:F2}ms.";

            }
            else
            {
                InfoLabel.Content = "Cannot find window";
            }
        }

        private void GetResultButton(object sender, RoutedEventArgs e)
        {
            try
            {
                IntPtr hwnd = WinAPI.FindWindow(null, WindowName.Text);
                if (hwnd != IntPtr.Zero)
                {
                    //Bitmap bmp = CaptureWindow(hwnd);
                    Bitmap bmp = CaptureWindow(hwnd);

                    bmp = Colormode(7, 282, 177, 882, 508, bmp);

                    Screenshot(bmp, "image.png");

                    byte[] bytes = BitmapsToByteArray(new(){ bmp }, out int cnt, out int w, out int h, out int c);

                    int sum = 0;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        sum += bytes[i];
                    }
                    Debug.WriteLine("Sum: " + sum);

                    int val = execute(bytes, w, h, c, 1, false, false, out _, out int ans, out double ratio, 0);
                    InfoLabel.Content = $"val: {val}, ans: {ans}";
                }
            }
            catch (Exception ex)
            {
                InfoLabel.Content = $"Exception: {ex.Message}";
            }

        }

    }
}
