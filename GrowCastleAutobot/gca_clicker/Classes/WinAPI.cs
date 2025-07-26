using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace gca_clicker.Classes
{
    public class WinAPI
    {

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void ForceBringWindowToFront(Window window)
        {
            nint hWnd = 0;

            window.Dispatcher.Invoke(() =>
            {
                hWnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
                if(hWnd == 0)
                {
                    return;
                }

                window.Topmost = true;
                window.Topmost = false;
                window.Activate();
            });


            ShowWindow(hWnd, SW_RESTORE);
            SetForegroundWindow(hWnd);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);


        public const int SW_MINIMIZE = 6;
        public const int SW_MAXIMIZE = 3;
        public const int SW_RESTORE = 9;

        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWNORMAL = 1;

        public const int KEYEVENTF_KEYDOWN = 0x0000;
        public const int KEYEVENTF_KEYUP = 0x0002;

        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;

        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;

        public const int WM_MOUSEMOVE = 0x0200;

        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;

        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;

        public const uint WM_MOUSEWHEEL = 0x020A;

        public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;

        public const uint MOUSEEVENTF_WHEEL = 0x0800;
        public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        public static readonly int width = GetSystemMetrics(SM_CXSCREEN);
        public static readonly int height = GetSystemMetrics(SM_CYSCREEN);

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static void MinimizeWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_MINIMIZE);
        }

        public static void MaximizeWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_MAXIMIZE);
        }

        public static void RestoreWindow(IntPtr hWnd)
        {
            ShowWindow(hWnd, SW_RESTORE);
        }

        [DllImport("user32.dll")]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public static string GetWindowState(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));

            if (GetWindowPlacement(hWnd, ref placement))
            {
                switch (placement.showCmd)
                {
                    case SW_SHOWMINIMIZED:
                        return "Minimized";
                    case SW_SHOWMAXIMIZED:
                        return "Maximized";
                    case SW_SHOWNORMAL:
                    default:
                        return "Normal";
                }
            }

            return "Unknown";
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public RECT rcNormalPosition;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        public static readonly IntPtr HWND_TOP = IntPtr.Zero;

        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_NOMOVE = 0x0002;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int MOD_ALT = 0x1;
        public const int MOD_CONTROL = 0x2;
        public const int MOD_SHIFT = 0x4;
        public const int MOD_WIN = 0x8;

        public const int WM_HOTKEY = 0x0312;

    }
}
