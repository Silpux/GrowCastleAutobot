using gca_clicker.Classes;
using gca_clicker.Classes.Exceptions;
using gca_clicker.Clicker;
using gca_clicker.Enums;
using gca_clicker.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static gca_clicker.Classes.Utils;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        public bool IsSaveGamePanelOpened()
        {
            Getscreen();
            return Pxl(969, 522) == Col(58, 130, 188) &&
            Pxl(939, 552) == Col(197, 207, 213) &&
            Pxl(499, 348) == Col(69, 58, 48) &&
            Pxl(545, 526) == Col(155, 230, 255);
        }

        /// <summary>
        /// Call when in gc menu
        /// </summary>
        /// <exception cref="OnlineActionsException"></exception>
        public void DoSave()
        {
            if (!CheckGCMenu())
            {
                Log.T($"{nameof(DoSave)}: was not in gc menu");
                throw new OnlineActionsException($"{nameof(DoSave)}: was not in gc menu");
            }

            RandomClickIn(280, 773, 331, 826);

            Wait(500);

            WaitUntil(() => CheckSky() || IsSaveGamePanelOpened(), delegate { }, 10_000, 50);

            Wait(300);

            if (!IsSaveGamePanelOpened())
            {
                Log.T($"{nameof(IsSaveGamePanelOpened)}: Save game panel was not open");
                throw new OnlineActionsException($"{nameof(IsSaveGamePanelOpened)}: Save game panel was not open");
            }

            RandomClickIn(931, 515, 965, 547);

            Wait(rand.Next(1500, 2500));

            RandomClickIn(846, 685, 985, 738);

            Wait(500);

            WaitUntil(CheckSky, delegate { }, 20_000, 50);

            Wait(200);

            ClosePopup();

            Wait(200);

        }

        public void PerformSaveActions(OnlineActions actions)
        {
            if ((actions & OnlineActions.DoSave) != 0)
            {
                DoSave();
            }
        }
    }
}
