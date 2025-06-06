using gca_clicker.Classes;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gca_clicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        [DllImport("gca_captcha_solver.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int execute(byte[] data, int width, int height, int channels, int count, int trackThingNum, bool saveScreenshots, bool failMode, out int ans, out double ratio0_1, int testVal);

        public MainWindow()
        {
            InitializeComponent();
        }

        public void ClickBackground(IntPtr hWnd, int x, int y)
        {
            IntPtr lParam = MakeLParam(x, y);
            WinAPI.SendMessage(hWnd, WinAPI.WM_LBUTTONDOWN, (IntPtr)1, lParam);
            WinAPI.SendMessage(hWnd, WinAPI.WM_LBUTTONUP, (IntPtr)0, lParam);
        }

        public IntPtr MakeLParam(int x, int y)
        {
            return (IntPtr)((y << 16) | (x & 0xFFFF));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                Thread.Sleep(1000);

                WinAPI.SetCursorPos(500, 300);
                Thread.Sleep(1000);
                WinAPI.mouse_event(WinAPI.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                WinAPI.mouse_event(WinAPI.MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);

                Thread.Sleep(500);
            });

            thread.IsBackground = true;
            //thread.Start();

            InfoLabel.Content = "";
            IntPtr hwnd = WinAPI.FindWindow(null, WindowName.Text);
            if (hwnd != IntPtr.Zero)
            {

                if (int.TryParse(XCoord.Text, out int x) && int.TryParse(YCoord.Text, out int y))
                {
                    ClickBackground((nint)hwnd, x, y);
                }
                else
                {
                    InfoLabel.Content = "number parse error";
                }

            }
            else
            {
                InfoLabel.Content = "Cannot find window";
            }


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
        Bitmap CaptureWindowByTitle(string windowTitle)
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

        Bitmap CaptureWindow(IntPtr hWnd)
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

                bool succeeded = WinAPI.PrintWindow(hWnd, hdcBitmap, 0); // 0 = full window
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
                WinAPI.SetWindowPos(hwnd, hwnd, 0, 0, 1520, 865,
                    WinAPI.SWP_NOZORDER);

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

                    byte[] bytes = FlattenBitmaps(new Bitmap[] { bmp }, out int w, out int h, out int c);

                    int sum = 0;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        sum += bytes[i];
                    }
                    Debug.WriteLine("Sum: " + sum);

                    int val = execute(bytes, w, h, c, 1, 1, false, false, out int ans, out double ratio, 0);
                    InfoLabel.Content = $"val: {val}, ans: {ans}";
                }
            }
            catch (Exception ex)
            {
                InfoLabel.Content = $"Exception: {ex.Message}";
            }

        }

        public static byte[] FlattenBitmaps(Bitmap[] bitmaps, out int width, out int height, out int channels)
        {
            if (bitmaps.Length == 0) throw new ArgumentException("Empty bitmap array");

            width = bitmaps[0].Width;
            height = bitmaps[0].Height;
            channels = System.Drawing.Image.GetPixelFormatSize(bitmaps[0].PixelFormat) / 8;
            int imageSize = width * height * channels;
            byte[] buffer = new byte[bitmaps.Length * imageSize];

            for (int i = 0; i < bitmaps.Length; i++)
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

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            return int.TryParse(text, out _);
        }

        private void MyTabControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var tabPanel = FindVisualChild<TabPanel>(MyTabControl);
            if (tabPanel != null)
            {
                System.Windows.Point mousePos = e.GetPosition(tabPanel);
                Rect bounds = new Rect(0, 0, tabPanel.ActualWidth, tabPanel.ActualHeight);

                if (bounds.Contains(mousePos))
                {
                    int currentIndex = MyTabControl.SelectedIndex;
                    int totalTabs = MyTabControl.Items.Count;

                    if (e.Delta < 0)
                        currentIndex = (currentIndex + 1) % totalTabs;
                    else if (e.Delta > 0)
                        currentIndex = (currentIndex - 1 + totalTabs) % totalTabs;

                    MyTabControl.SelectedIndex = currentIndex;
                    e.Handled = true;
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t)
                {
                    return t;
                }

                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
    }
}