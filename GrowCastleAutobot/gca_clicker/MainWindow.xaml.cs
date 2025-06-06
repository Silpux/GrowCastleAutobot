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
using System.Windows.Interop;
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

        private bool _isListeningForShortcut = false;

        [DllImport("gca_captcha_solver.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int execute(byte[] data, int width, int height, int channels, int count, int trackThingNum, bool saveScreenshots, bool failMode, out int ans, out double ratio0_1, int testVal);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);



        private const int MOD_ALT = 0x1;
        private const int MOD_CONTROL = 0x2;
        private const int MOD_SHIFT = 0x4;
        private const int MOD_WIN = 0x8;

        private const int WM_HOTKEY = 0x0312;

        private IntPtr windowHandle;
        private HwndSource source;
        private const int HOTKEY_ID = 9000;

        private uint _currentModifiers = 0;
        private uint _currentKey = 0;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closed += OnClosed;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            windowHandle = helper.Handle;
            source = HwndSource.FromHwnd(windowHandle);
            source.AddHook(HwndHook);

            RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_ALT, (uint)KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.F1));
        }

        private void OnClosed(object sender, EventArgs e)
        {
            source.RemoveHook(HwndHook);
            UnregisterHotKey(source.Handle, HOTKEY_ID);
        }

        private void ShortcutBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            UnregisterHotKey(windowHandle, HOTKEY_ID);
            ShortcutBox.Focus();
            _isListeningForShortcut = true;
            ShortcutBox.Text = "Press new shortcut...";
            e.Handled = true;
        }

        private void ShortcutBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!_isListeningForShortcut)
            {
                ShortcutBox.SelectAll();
            }
        }

        private void ShortcutBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_isListeningForShortcut)
            {
                _isListeningForShortcut = false;
                if (string.IsNullOrEmpty(ShortcutBox.Text) || ShortcutBox.Text == "Press new shortcut...")
                    ShortcutBox.Text = "Alt+F1";

                SaveShortcut(ShortcutBox.Text);
            }
        }

        private void ShortcutBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!_isListeningForShortcut)
                return;

            e.Handled = true;

            Key key = e.Key == Key.System ? e.SystemKey : e.Key;

            if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LWin || key == Key.RWin)
                return;

            StringBuilder sb = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) sb.Append("Ctrl+");
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) sb.Append("Shift+");
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0) sb.Append("Alt+");
            if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0) sb.Append("Win+");

            string keyName = key.ToString();
            if (keyName.StartsWith("Oem"))
            {
                keyName = GetFriendlyOemKeyName(key);
            }

            sb.Append(keyName);

            ShortcutBox.Text = sb.ToString();
            _isListeningForShortcut = false;

            SaveShortcut(ShortcutBox.Text);
        }

        private string GetFriendlyOemKeyName(Key key)
        {
            return key switch
            {
                Key.OemPlus => "+",
                Key.OemMinus => "-",
                Key.OemComma => ",",
                Key.OemPeriod => ".",
                Key.OemQuestion => "/",
                Key.OemSemicolon => ";",
                Key.OemQuotes => "'",
                Key.OemOpenBrackets => "[",
                Key.OemCloseBrackets => "]",
                Key.OemPipe => "\\",
                _ => key.ToString(),
            };
        }

        private void SaveShortcut(string shortcut)
        {

            ParseShortcut(shortcut, out uint modifiers, out uint key);

            UnregisterHotKey(windowHandle, HOTKEY_ID);

            bool success = RegisterHotKey(windowHandle, HOTKEY_ID, modifiers, key);
            if (!success)
            {
                MessageBox.Show("Failed to register hotkey. Choose another, this may be in use already");
                RegisterHotKey(windowHandle, HOTKEY_ID, _currentModifiers, _currentKey);
                return;
            }

            _currentModifiers = modifiers;
            _currentKey = key;

            Console.WriteLine($"Registered new global hotkey: {shortcut}");
        }





        private void ParseShortcut(string shortcutText, out uint modifiers, out uint key)
        {
            modifiers = 0;
            key = 0;

            var parts = shortcutText.Split('+', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var p = part.Trim();

                switch (p.ToLower())
                {
                    case "ctrl":
                        modifiers |= MOD_CONTROL;
                        break;
                    case "shift":
                        modifiers |= MOD_SHIFT;
                        break;
                    case "alt":
                        modifiers |= MOD_ALT;
                        break;
                    case "win":
                        modifiers |= MOD_WIN;
                        break;
                    default:
                        key = (uint)KeyInterop.VirtualKeyFromKey(ParseKeyFromString(p));
                        break;
                }
            }
        }


        private Key ParseKeyFromString(string keyString)
        {
            if (keyString.Length == 1 && char.IsLetter(keyString[0]))
            {
                return (Key)Enum.Parse(typeof(Key), keyString, true);
            }

            return keyString.ToUpper() switch
            {
                "+" => Key.OemPlus,
                "-" => Key.OemMinus,
                "," => Key.OemComma,
                "." => Key.OemPeriod,
                "/" => Key.OemQuestion,
                ";" => Key.OemSemicolon,
                "'" => Key.OemQuotes,
                "[" => Key.OemOpenBrackets,
                "]" => Key.OemCloseBrackets,
                "\\" => Key.OemPipe,
                _ => (Key)Enum.Parse(typeof(Key), keyString, true)
            };
        }






        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == HOTKEY_ID)
                {
                    OnHotKeyPressed();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            MessageBox.Show("Global hotkey pressed!");
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