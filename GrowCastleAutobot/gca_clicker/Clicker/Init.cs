using gca_clicker.Classes;
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

            if (backgroundMode)
            {
                hwnd = WndFind(WindowName.Text);

                if (hwnd == IntPtr.Zero)
                {
                    message = "Didn't find window";
                    return false;
                }
            }


            return true;

        }

    }

}
