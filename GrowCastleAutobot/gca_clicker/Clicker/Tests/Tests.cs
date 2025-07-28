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

            TestMode testMode = TestMode.MouseMovement1;
            string tag = (sender as Button)!.Tag.ToString()!;
            if (tag == "2")
            {
                testMode = TestMode.MouseMovement2;
            }
            else if (tag == "3")
            {
                testMode = TestMode.MouseMovement3;
            }

            Log.V($"{nameof(TestMouseMovement_Click)}: {testMode}");
            StartThread(testMode);
        }

        private void MoveMouseTest_Click(object sender, RoutedEventArgs e)
        {
            InfoLabel.Content = "";

            TestMode testMode = TestMode.MouseMove;

            Log.V($"{nameof(MoveMouseTest_Click)}: {testMode}");
            StartThread(testMode);
        }

        private void CrystalsCountTest_Click(object sender, RoutedEventArgs e)
        {
            CrystalsCountLabel.Content = "";

            TestMode testMode = TestMode.CrystalsCount;

            Log.V($"{nameof(CrystalsCountTest_Click)}: {testMode}");
            StartThread(testMode);
        }

        private void RestartTest_Click(object sender, RoutedEventArgs e)
        {
            RestartTestLabel.Content = "Do restart";
            TestMode testMode = TestMode.Restart;
            Log.V($"{nameof(RestartTest_Click)}: {testMode}");
            StartThread(testMode);
        }
        private void ResetTest_Click(object sender, RoutedEventArgs e)
        {
            RestartTestLabel.Content = "Do reset";
            TestMode testMode = TestMode.Reset;
            Log.V($"{nameof(ResetTest_Click)}: {testMode}");
            StartThread(testMode);
        }
        private void CleanupTest_Click(object sender, RoutedEventArgs e)
        {
            RestartTestLabel.Content = "Do cleanup";
            TestMode testMode = TestMode.Cleanup;
            Log.V($"{nameof(CleanupTest_Click)}: {testMode}");
            StartThread(testMode);
        }

        private void UpgradeHeroTest_Click(object sender, RoutedEventArgs e)
        {
            UpgradeTestLabel.Content = "Upgrade hero";
            TestMode testMode = TestMode.UpgradeHero;
            Log.V($"{nameof(UpgradeHeroTest_Click)}: {testMode}");
            StartThread(testMode);
        }

        private void UpgradeCastleTest_Click(object sender, RoutedEventArgs e)
        {
            UpgradeTestLabel.Content = "Upgrade castle";
            TestMode testMode = TestMode.UpgradeCastle;
            Log.V($"{nameof(UpgradeCastleTest_Click)}: {testMode}");
            StartThread(testMode);
        }

        private void DoOnlineActionsTest_Click(object sender, RoutedEventArgs e)
        {
            TestMode testMode = TestMode.OnlineActions;
            Log.V($"{nameof(DoOnlineActionsTest_Click)}: {testMode}");
            StartThread(testMode);
        }
        private void ShowGameStatusTest_Click(object sender, RoutedEventArgs e)
        {
            TestMode testMode = TestMode.ShowGameStatus;
            Log.V($"{nameof(ShowGameStatusTest_Click)}: {testMode}");
            StartThread(testMode);
        }
        private void SolveCaptchaTest_Click(object sender, RoutedEventArgs e)
        {
            TestMode testMode = TestMode.SolveCaptcha;
            Dispatcher.Invoke(() =>
            {
                SolveCaptchaTestLabel.Content = "";
            });
            Log.V($"{nameof(SolveCaptchaTest_Click)}: {testMode}");
            StartThread(testMode);
        }

        private void SaveWindowScreen_Click(object sender, RoutedEventArgs e)
        {
            SaveWindowScreenLabel.Content = "";
            IntPtr hwnd = WinAPI.FindWindow(null, WindowName.Text);
            if (hwnd != IntPtr.Zero)
            {
                Bitmap bmp = CaptureWindow(hwnd);
                string path = Screenshot(bmp, Cst.SCREENSHOT_TEST_WINDOW_PATH);
                SaveWindowScreenLabel.Content = path;
                bmp.Dispose();
            }
            else
            {
                SaveWindowScreenLabel.Content = "Cannot find window";
            }

        }
        private void OpenTestScreen_Click(object sender, RoutedEventArgs e)
        {

            string path = "";

            if(sender == OpenWindowScreenButton)
            {
                path = SaveWindowScreenLabel.Content.ToString()!;
            }
            else if(sender == OpenCompleteScreenButton)
            {
                path = SaveCompleteScreenLabel.Content.ToString()!;
            }
            else if (sender == OpenScreenJpgButton)
            {
                path = SaveScreenJpgLabel.Content.ToString()!;
            }

            if (string.IsNullOrWhiteSpace(path) || path.Length >= 260)
            {
                goto OpenDefaultPath;
            }

            foreach (char c in Path.GetInvalidPathChars())
            {
                if (path.Contains(c))
                {
                    goto OpenDefaultPath;
                }
            }

            try
            {
                string fullPath = Path.GetFullPath(path);

                Process.Start("explorer.exe", $"/select,\"{fullPath}\"");
            }
            catch
            {
                ;
            }

            return;

        OpenDefaultPath:

            Process.Start("explorer.exe", GetFullPathAndCreate(Cst.SCREENSHOT_TEST_PATH));

        }
        private void SaveCompleteScreen_Click(object sender, RoutedEventArgs e)
        {
            SaveCompleteScreenLabel.Content = "";
            Bitmap bmp = CaptureScreen();
            string path = Screenshot(bmp, Cst.SCREENSHOT_TEST_SCREEN_PATH);
            SaveCompleteScreenLabel.Content = path;
            bmp.Dispose();
        }

        private void SaveJpgScreen_Click(object sender, RoutedEventArgs e)
        {

            SaveScreenJpgLabel.Content = "";
            IntPtr hwnd = WinAPI.FindWindow(null, WindowName.Text);
            if (hwnd != IntPtr.Zero)
            {
                Bitmap bmp = CaptureWindow(hwnd);

                if(!int.TryParse(QualityImageTestTextBox.Text, out int result))
                {
                    SaveScreenJpgLabel.Content = "Wrong quality value";
                    return;
                }

                int quality = Math.Max(Math.Min(100, result), 0);

                string path = ScreenshotJpg(bmp, Cst.SCREENSHOT_TEST_JPG_SCREEN_PATH, quality);

                SaveScreenJpgLabel.Content = path;
                bmp.Dispose();
            }
            else
            {
                SaveScreenJpgLabel.Content = "Cannot find window";
            }

        }




        private void GetscreenBenchmark(object sender, RoutedEventArgs e)
        {
            GetscreenBenchmarkTestLabel.Content = "";
            IntPtr hwnd = WinAPI.FindWindow(null!, WindowName.Text);
            if (hwnd != IntPtr.Zero)
            {

                Stopwatch sw = Stopwatch.StartNew();

                for (int i = 0; i < 100; i++)
                {
                    Bitmap bmp = CaptureWindow(hwnd);
                }
                sw.Stop();

                GetscreenBenchmarkTestLabel.Content = $"Avg time: {(float)sw.ElapsedMilliseconds / 100:F2}ms.";

            }
            else
            {
                GetscreenBenchmarkTestLabel.Content = $"Cannot find window: {WindowName.Text}";
            }
        }






    }
}
