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
            return Pxl(282, 35) == Cst.SkyColor;
        }


        public bool CheckGCMenu()
        {
            Getscreen();
            return Pxl(1407, 159) == Cst.CastleUpgradeColor;
        }


        public bool CaptchaOnScreen()
        {
            Getscreen();
            return Pxl(403, 183) == Col(98, 87, 73) &&
                Pxl(722, 305) == Col(189, 165, 127) &&
                Pxl(723, 591) == Col(189, 165, 127);

        }
















    }
}
