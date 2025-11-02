using gca.Classes;
using System.Windows;

namespace gca
{
    public partial class Autobot
    {

        /// <summary>
        /// not background mode
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void LeftClick(int x, int y)
        {
            WinAPI.SetCursorPos(x, y);
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, UIntPtr.Zero);
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// not background mode
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
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

        /// <summary>
        /// not background mode
        /// </summary>
        /// <param name="amount"></param>
        public void ScrollWheel(int amount)
        {
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_WHEEL, 0, 0, (uint)amount, UIntPtr.Zero);
        }

        /// <summary>
        /// background mode
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="delta"></param>
        public void WheelBackground(IntPtr hWnd, int x, int y, int delta)
        {
            IntPtr lParam = MakeLParam(x, y);
            IntPtr wParam = MakeWParam(0, delta);
            WinAPI.SendMessage(hwnd, WinAPI.WM_MOUSEWHEEL, wParam, lParam);
        }

        public void MoveBackground(IntPtr hWnd, int x, int y)
        {
            IntPtr lParam = MakeLParam(x, y);
            WinAPI.SendMessage(hWnd, WinAPI.WM_MOUSEMOVE, IntPtr.Zero, lParam);
        }

        public IntPtr MakeLParam(int x, int y)
        {
            return (IntPtr)((y << 16) | (x & 0xFFFF));
        }

        public IntPtr MakeWParam(int x, int y) => MakeLParam(x, y);

        private void SendKey(byte keyCode)
        {
            WinAPI.keybd_event(keyCode, 0, WinAPI.KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            Thread.Sleep(50);
            WinAPI.keybd_event(keyCode, 0, WinAPI.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private Bitmap CaptureWindowByTitle(string windowTitle)
        {
            IntPtr hWnd = WinAPI.FindWindow(null!, windowTitle);
            if (hWnd == IntPtr.Zero)
            {
                throw new Exception("Window not found");
            }

            WinAPI.GetWindowRect(hWnd, out WinAPI.RECT rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            Bitmap bmp = null!;

            if (currentScreen == null || currentScreen.Width != width || currentScreen.Height != height)
            {
                bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else
            {
                bmp = currentScreen;
            }

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

    }
}
