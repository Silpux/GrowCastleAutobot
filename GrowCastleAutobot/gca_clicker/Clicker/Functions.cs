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
using static gca_clicker.Classes.Utils;
using gca_clicker.Classes.MouseMove;
using System.Printing;
using gca_clicker.Enums;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {


        private readonly List<ScreenshotEntry> frameHistory = new();
        private readonly TimeSpan freezeDuration = TimeSpan.FromSeconds(2);
        private readonly TimeSpan minTimeToDetectFreeze = TimeSpan.FromSeconds(0.5);

        private System.Drawing.Size lastSize = System.Drawing.Size.Empty;

        private bool freezeDetectionEnabled = true;

        public void CheckForFreeze(Bitmap currentScreen, DateTime timestamp)
        {
            if (!freezeDetectionEnabled)
            {
                return;
            }

            System.Drawing.Size size = currentScreen.Size;

            frameHistory.RemoveAll(entry => (timestamp - entry.Timestamp) > freezeDuration);

            if (size != lastSize)
            {
                frameHistory.Clear();
                lastSize = size;
            }

            frameHistory.Add(new ScreenshotEntry { Hash = BmpHash(currentScreen), Timestamp = timestamp });

            if (frameHistory.Count >= 3 && AllBitmapsEqual(frameHistory) && frameHistory[^1].Timestamp - frameHistory[0].Timestamp >= minTimeToDetectFreeze)
            {
                OnWindowFreezeDetected();
            }
        }

        private void OnWindowFreezeDetected()
        {

            if (screenshotOnFreezing)
            {
                Screenshot(currentScreen, Cst.SCREENSHOT_ON_FREEZE_PATH);
            }

            Log.C($"Freeze detected. Will reset");
            Thread.Sleep(100);
            Reset();

            restartRequested = true;
            Halt();
        }

        /// <summary>
        /// getscreen
        /// </summary>
        private void G(bool saveScreen = false) => Getscreen(saveScreen);

        private void Getscreen(bool saveScreen = false)
        {
            CheckNoxState();
            if (backgroundMode)
            {
                currentScreen = CaptureWindow(hwnd);
                if (monitorFreezing)
                {
                    CheckForFreeze(currentScreen, DateTime.Now);
                }
            }
            else
            {
                currentScreen = CaptureScreen();
            }
            screenshotCache.AddScreenshot(currentScreen, saveScreen);
        }

        private void ScreenshotError(bool save, string relativePath)
        {
            Dispatcher.Invoke(() =>
            {
                MyTabControl.Background = Cst.ErrorBackgrounColor;
            });
            if (save)
            {
                Screenshot(currentScreen, relativePath);
            }
            if (saveScreenshotsOnError)
            {
                screenshotCache.SaveAllToFolder(Cst.SCREENSHOT_ERROR_SCREEN_CACHE_PATH);
            }
        }

        private Bitmap CaptureArea(int x, int y, int width, int height)
        {

            Bitmap bmp = null!;
            if (currentScreen == null || currentScreen.Width != width || currentScreen.Height != height)
            {
                bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else
            {
                bmp = currentScreen;
            }
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
            }
            return bmp;
        }

        private Bitmap CaptureWindow(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid HWND");
            }

            if (!WinAPI.GetWindowRect(hWnd, out WinAPI.RECT rect))
            {
                throw new Exception("Could not get window bounds");
            }

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

        private Bitmap CaptureScreen()
        {
            Bitmap bmp = null!;

            if (currentScreen == null || currentScreen.Width != WinAPI.width || currentScreen.Height != WinAPI.height)
            {
                bmp = new Bitmap(WinAPI.width, WinAPI.height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else
            {
                bmp = currentScreen;
            }
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            }
            return bmp;
        }

        private nint WndFind(string windowName)
        {
            return WinAPI.FindWindow(null!, windowName);
        }

        /// <summary>
        /// Pxl
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Color P(int x, int y) => Pxl(x, y);

        private Color Pxl(int x, int y)
        {
            if(currentScreen is null)
            {
                G();
            }
            if(x < 0 || y < 0)
            {
                coordNotTakenCounter++;
                Log.E($"Wrong coordinates to take pixel: ({x}, {y})");
                return Color.Black;
            }
            else if (x >= currentScreen!.Width || y >= currentScreen.Height)
            {
                coordNotTakenCounter++;
                Log.E($"Wrong coordinates: ({x}, {y}). Size of current bitmap: ({currentScreen.Width}, {currentScreen.Height})");

                if (!CheckNoxState())
                {
                    Log.E($"Getscreen again");
                    G();
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
            if(coordNotTakenCounter > 30)
            {
                ScreenshotError(screenshotOnEsc, Cst.SCREENSHOT_ON_ESC_PATH);
                Log.C("Coordinated are outside of nox window. Couldn't fix nox window");
                Halt();
            }
            return currentScreen.GetPixel(x, y);

        }

        private void MoveCursorTo(int x, int y)
        {
            MouseMovement movement = new BezierMouseMovement(new PointF(previousMousePosition.x, previousMousePosition.y), new PointF(x, y));
            movement.SpeedFactor = 1f;

            foreach(var p in movement.GetPoints())
            {
                SetCursor((int)p.X, (int)p.Y);
                Wait(1);
            }

            previousMousePosition.x = x;
            previousMousePosition.y = y;
        }

        /// <summary>
        /// Set cursor position directly
        /// </summary>
        private void SetCursor(int x, int y)
        {
            if (backgroundMode)
            {
                MoveBackground(hwnd, x, y);
            }
            else
            {
                SetCursorPos(x, y);
            }
        }

        /// <summary>
        /// Move cursor to coords. If simulateMouseMovement is true => will move smoothly
        /// </summary>
        private void Move(int x, int y)
        {
            if (simulateMouseMovement)
            {
                MoveCursorTo(x, y);
            }
            else
            {
                SetCursor(x, y);
            }
        }

        /// <summary>
        /// Depends on backgroundMode
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void LC(int x, int y) => LClick(x, y);

        /// <summary>
        /// Depends on backgroundMode
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void LClick(int x, int y)
        {

            if (simulateMouseMovement)
            {
                MoveCursorTo(x, y);
            }

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
            LC(x, y);
            Wait(70);
            LC(x, y);
        }

        /// <summary>
        /// Depends on backgroundMode
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void RClick(int x, int y)
        {

            if (simulateMouseMovement)
            {
                MoveCursorTo(x, y);
            }

            if (backgroundMode)
            {
                RightClickBackground(hwnd, x, y);
            }
            else
            {
                RightClick(x, y);
            }
        }

        /// <summary>
        /// RandomClickIn
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        private void RCI(int x1, int y1, int x2, int y2) => RandomClickIn(x1, y1, x2, y2);

        private void RandomClickIn(int x1, int y1, int x2, int y2)
        {
            LC(x1 + (int)((x2 - x1) * rand.NextDouble()), y1 + (int)((y2 - y1) * rand.NextDouble()));
        }

        /// <summary>
        /// RandomMoveIn
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        private void RMI(int x1, int y1, int x2, int y2) => RandomMoveIn(x1, y1, x2, y2);
        private void RandomMoveIn(int x1, int y1, int x2, int y2)
        {
            Move(x1 + (int)((x2 - x1) * rand.NextDouble()), y1 + (int)((y2 - y1) * rand.NextDouble()));
        }

        private void RandomDblClickIn(int x1, int y1, int x2, int y2)
        {
            DblClick(x1 + (int)((x2 - x1) * rand.NextDouble()), y1 + (int)((y2 - y1) * rand.NextDouble()));
        }

        /// <summary>
        /// Depends on backgroundMode
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="delta"></param>
        private void Mouse_Wheel(int x, int y, int delta)
        {

            Move(x, y);

            if (backgroundMode)
            {
                WheelBackground(hwnd, x, y, delta);
            }
            else
            {
                ScrollWheel(delta);
            }
        }

        /// <summary>
        /// This will bring window to top, because otherwise key will not be pressed
        /// </summary>
        /// <param name="key"></param>
        private void SendKey(Keys key)
        {
            IntPtr wParam = (IntPtr)key;
            SetForegroundWindow(hwnd);
            SendMessage(hwnd, WM_KEYDOWN, wParam, 0);
            SendMessage(hwnd, WM_KEYUP, wParam, 0);
        }

        private void RandomWait(int min, int max)
        {
            int wait = rand.Next(min, max + 1);
            Log.T($"Wait {wait} ms.");
            Wait(wait);
        }


        public bool PixelIn(int x1, int y1, int x2, int y2, Color color, out (int x, int y) ret, int gap = 1)
        {
            for (int j = y1; j <= (y2 < currentScreen.Height - 1 ? y2 : currentScreen.Height - 1); j += gap)
            {
                for (int i = x1; i <= (x2 < currentScreen.Width - 1 ? x2 : currentScreen.Width - 1); i += gap)
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

        public bool PixelIn(int x1, int y1, int x2, int y2, Color color, int gap = 1)
        {

            if(currentScreen == null)
            {
                Getscreen();
            }

            for (int j = y1; j <= (y2 < currentScreen.Height - 1 ? y2 : currentScreen.Height - 1); j += gap)
            {
                for (int i = x1; i <= (x2 < currentScreen.Width - 1 ? x2 : currentScreen.Width - 1); i += gap)
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

            if (currentScreen == null)
            {
                Getscreen();
            }

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







        public int[] GenerateActivationSequence(bool includeSingleClick = false)
        {

            if (!randomizeClickSequence)
            {
                var p = new List<int>(15);
                for (int i = 0; i < 15; i++)
                {
                    if (thisDeck[i] || includeSingleClick && singleClickSlots.Contains(i))
                    {
                        p.Add(i);
                    }
                }
                return p.ToArray();
            }

            bool[,] matrix;
            if (includeSingleClick)
            {
                matrix = MatrixCopy(buildMatrix);
                foreach(var c in singleClickSlots)
                {
                    (int y, int x) = GetMatrixPosition(c);
                    matrix[y, x] = true;
                }
            }
            else
            {
                matrix = buildMatrix;
            }

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            var rnd = new Random();

            var positions = new List<(int y, int x)>(15);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (matrix[i, j])
                    {
                        positions.Add((i, j));
                    }
                }
            }

            if (positions.Count == 0)
            {
                return new int[] { };
            }

            var current = positions[rnd.Next(positions.Count)];
            var sequence = new List<(int y, int x)> { current };
            positions.Remove(current);

            while (positions.Count > 0)
            {
                int minDist = int.MaxValue;
                var closest = new List<(int y, int x)>(15);

                foreach (var pos in positions)
                {
                    int dx = Math.Abs(pos.x - current.x);
                    int dy = Math.Abs(pos.y - current.y);
                    int dist = Math.Max(dx, dy);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest.Clear();
                        closest.Add(pos);
                    }
                    else if (dist == minDist)
                    {
                        closest.Add(pos);
                    }
                }
                var next = closest[rnd.Next(closest.Count)];
                sequence.Add(next);
                positions.Remove(next);
                current = next;
            }

            int[] result = new int[sequence.Count];

            for (int i = 0; i < sequence.Count; i++)
            {
                result[i] = GetSlotIndex(sequence[i]);
            }

            return result;
        }

    }
}
