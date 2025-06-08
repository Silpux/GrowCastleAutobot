using gca_clicker.Classes;
using gca_clicker.Clicker;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

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
            Loaded += OnLoaded;
            Closed += OnClosed;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            windowHandle = helper.Handle;
            source = HwndSource.FromHwnd(windowHandle);
            source.AddHook(HwndHook);

            WinAPI.RegisterHotKey(helper.Handle, HOTKEY_START_ID, WinAPI.MOD_ALT, (uint)KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.F1));
            WinAPI.RegisterHotKey(helper.Handle, HOTKEY_STOP_ID, WinAPI.MOD_ALT, (uint)KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.F2));
        }

        private void OnClosed(object sender, EventArgs e)
        {
            OnStopHotkey();
            source.RemoveHook(HwndHook);
            WinAPI.UnregisterHotKey(source.Handle, HOTKEY_START_ID);
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

        private void TestButton(object sender, RoutedEventArgs e)
        {

            backgroundMode = false;
            nint hWnd = WndFind(WindowName.Text);

            if (hWnd != IntPtr.Zero)
            {

                //(int x, int y, int width, int height) info = GetWindowInfo(hWnd);

                //Debug.WriteLine(info);
                //Getscreen();
                //Debug.WriteLine(Pxl(714, 120));
                //WinAPI.RestoreWindow(hWnd);

                hwnd = hWnd;

                //InfoLabel.Content = "Crystals: " + CountCrystals(true);

                //System.Drawing.Color col = System.Drawing.Color.FromArgb(1, 1, 1, 1);

                //Debug.WriteLine(GetLine(Cst.DUNGEON_STATISTICS_PATH, 2));

                //InsertLine(Cst.DUNGEON_STATISTICS_PATH, 2, "444");

                //RemoveLine(Cst.DUNGEON_STATISTICS_PATH, 2);



            }



        }
    }
}