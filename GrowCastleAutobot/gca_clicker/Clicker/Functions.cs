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
            if(currentScreen is null)
            {
                Getscreen();
            }
            if(x < 0 || y < 0)
            {
                Log.E($"Wrong coordinates to take pixel: ({x}, {y})");
                return Color.Black;
            }
            if (x >= currentScreen!.Width || y >= currentScreen.Height)
            {
                Log.E($"Wrong coordinates: ({x}, {y}). Size of current bitmap: ({currentScreen.Width}, {currentScreen.Height})");

                if (!CheckNoxState())
                {
                    Log.E($"Getscreen again");
                    Getscreen();
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
            LClick(x, y);
            Wait(70);
            LClick(x, y);
        }

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

        private void RandomClickIn(int x1, int y1, int x2, int y2)
        {
            LClick(x1 + (int)((x2 - x1) * rand.NextDouble()), y1 + (int)((y2 - y1) * rand.NextDouble()));
        }

        private void RandomMoveIn(int x1, int y1, int x2, int y2)
        {
            Move(x1 + (int)((x2 - x1) * rand.NextDouble()), y1 + (int)((y2 - y1) * rand.NextDouble()));
        }

        private void RandomDblClickIn(int x1, int y1, int x2, int y2)
        {
            DblClick(x1 + (int)((x2 - x1) * rand.NextDouble()), y1 + (int)((y2 - y1) * rand.NextDouble()));
        }

        private void RandomWait(int min, int max)
        {
            int wait = rand.Next(min, max + 1);
            Log.T($"Wait {wait} ms.");
            Wait(wait);
        }


        public bool PixelIn(int x1, int y1, int x2, int y2, Color color, out (int x, int y) ret)
        {
            for (int j = y1; j <= (y2 < currentScreen.Height - 1 ? y2 : currentScreen.Height - 1); j++)
            {
                for (int i = x1; i <= (x2 < currentScreen.Width - 1 ? x2 : currentScreen.Width - 1); i++)
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

        public bool PixelIn(int x1, int y1, int x2, int y2, Color color)
        {
            for (int j = y1; j <= (y2 < currentScreen.Height - 1 ? y2 : currentScreen.Height - 1); j++)
            {
                for (int i = x1; i <= (x2 < currentScreen.Width - 1 ? x2 : currentScreen.Width - 1); i++)
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
