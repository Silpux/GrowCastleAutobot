using gca_clicker.Classes;
using gca_clicker.Clicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        private bool Init(out string message)
        {
            message = "";
            backgroundMode = BackgroundModeCheckbox.IsChecked ?? false;

            hwnd = WndFind(WindowName.Text);

            if (hwnd == IntPtr.Zero)
            {
                message = "Didn't find window";
                return false;
            }


            (int x, int y, int width, int height) = GetWindowInfo(hwnd);

            if (backgroundMode)
            {
                if(Cst.WINDOW_WIDTH - width != 0)
                {
                    message += $"Expand by {Cst.WINDOW_WIDTH - width})\n\n";
                    return false;
                }
            }
            else
            {
                if(x != 0)
                {
                    message += $"Move window {-x} pxls right\n\n";
                }
                if(y != 0)
                {
                    message += $"Move window {-y} pxls up\n\n";
                }
                if (Cst.WINDOW_WIDTH - width != 0)
                {
                    message += $"Expand by {Cst.WINDOW_WIDTH - width}\n\n";
                }

                if(message.Length > 0)
                {
                    message = "Press set pos!\n\n" + message;
                    return false;
                }
            }


            solveCaptcha = SolveCaptchaCheckbox.IsChecked ?? false;


            return true;

        }

    }

}
