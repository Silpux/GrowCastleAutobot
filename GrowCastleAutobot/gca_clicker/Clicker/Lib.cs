using gca_clicker.Clicker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {


        public bool CheckSky()
        {
            Getscreen();
            return Pxl(282, 35) == Cols.skyColor;
        }


        public bool CheckGCMenu()
        {
            Getscreen();
            return Pxl(282, 35) == Cols.castleUpgradeColor;
        }


        public bool CaptchaOnScreen()
        {
            Getscreen();
            return Pxl(403, 183) == Color.FromArgb(255, 98, 87, 73) &&
                Pxl(722, 305) == Color.FromArgb(255, 189, 165, 127) &&
                Pxl(723, 591) == Color.FromArgb(255, 189, 165, 127);
        }
















    }
}
