using gca_clicker.Classes;
using gca_clicker.Clicker;
using gca_clicker.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static gca_clicker.Classes.Utils;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {


        private void TestButton(object sender, RoutedEventArgs e)
        {

            backgroundMode = false;
            nint hWnd = WndFind(WindowName.Text);

            if (hWnd != IntPtr.Zero)
            {
                hwnd = hWnd;
                SendKey(Keys.Escape);

            }
            else
            {
                WinAPI.ForceBringWindowToFront(this);
                MessageBox.Show($"Window not found: {WindowName.Text}");
            }



        }

        private void ResetInfoLabel_Click(object sender, RoutedEventArgs e)
        {
            InfoLabel.Content = "";
        }


        private void DoClickWithCoords_Click(object sender, RoutedEventArgs e)
        {
            InfoLabel.Content = "";
            IntPtr hwnd = WinAPI.FindWindow(null!, WindowName.Text);
            if (hwnd != IntPtr.Zero)
            {
                if (int.TryParse(XCoordDoClickTextBox.Text, out int x) && int.TryParse(YCoordDoClickTextBox.Text, out int y))
                {
                    LeftClickBackground((nint)hwnd, x, y);
                }
                else
                {
                    InfoLabel.Content = "Number parse error";
                }
            }
            else
            {
                InfoLabel.Content = $"Window not found: {WindowName.Text}";
            }
        }


        private void TestMouseMovement_Click(object sender, RoutedEventArgs e)
        {

            TestMode testMode = TestMode.TestMouseMovement1;
            string tag = (sender as Button)!.Tag.ToString()!;
            if (tag == "2")
            {
                testMode = TestMode.TestMouseMovement2;
            }
            else if (tag == "3")
            {
                testMode = TestMode.TestMouseMovement3;
            }

            Log.V($"{nameof(TestMouseMovement_Click)}: {testMode}");
            StartThread(testMode);
        }

        private void MoveMouseTest_Click(object sender, RoutedEventArgs e)
        {
            InfoLabel.Content = "";

            TestMode testMode = TestMode.TestMouseMove;

            Log.V($"{nameof(MoveMouseTest_Click)}: {testMode}");
            StartThread(testMode);
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

                    byte[] bytes = BitmapsToByteArray(new() { bmp }, out int cnt, out int w, out int h, out int c);

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






    }
}
