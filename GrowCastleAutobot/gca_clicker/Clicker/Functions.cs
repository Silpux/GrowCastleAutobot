using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            if (currentScreen != null)
            {
                if(x < 0 || x > currentScreen.Width || y <  0 || y > currentScreen.Height)
                {
                    return currentScreen.GetPixel(x, y);
                }
                Debug.WriteLine($"Wrong coordinates: ({x}, {y}). Size of current bitmap: ({currentScreen.Width}, {currentScreen.Height})");
                return Color.Black;
            }
            Debug.WriteLine($"Current bitmap is null");
            return Color.Black;
        }


        private void Lclick(int x, int y)
        {
            if (backgroundMode)
            {
                ClickBackground(hwnd, x, y);
            }
            else
            {
                Click(x, y);
            }
        }

        private void Rclick(int x, int y)
        {
            if (backgroundMode)
            {
                RClickBackground(hwnd, x, y);
            }
            else
            {
                RClick(x, y);
            }
        }




















    }
}
