using gca.Classes;
using gca.Classes.Exceptions;
using gca.Enums;
using System.Windows;
using static gca.Classes.Utils;

namespace gca
{
    public partial class Autobot
    {

        public bool IsSaveGamePanelOpened(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(969, 522) == Col(58, 130, 188) &&
            P(939, 552) == Col(197, 207, 213) &&
            P(499, 348) == Col(69, 58, 48) &&
            P(545, 526) == Col(155, 230, 255);
        }

        /// <summary>
        /// Call when in gc menu
        /// </summary>
        /// <exception cref="OnlineActionsException"></exception>
        public void DoSave()
        {

            Log.L("Do save");
            if (!CheckGCMenu())
            {
                Log.T($"{nameof(DoSave)}: was not in gc menu");
                throw new OnlineActionsException($"{nameof(DoSave)}: was not in gc menu");
            }

            Wait(500);

            ClosePopup();

            CloseAdForCoins();

            Log.L("Open save panel");
            RCI(280, 773, 331, 826);

            Wait(500);

            WaitUntil(() => CheckSky() || IsSaveGamePanelOpened(false), delegate { }, 10_000, 50);

            Wait(300);

            if (!IsSaveGamePanelOpened())
            {
                Log.T($"{nameof(IsSaveGamePanelOpened)}: Save game panel was not open");
                throw new OnlineActionsException($"{nameof(IsSaveGamePanelOpened)}: Save game panel was not open");
            }
            else
            {
                Log.L("Save panel opened");
            }

            Log.L("Click save");
            RCI(931, 515, 965, 547);

            Wait(rand.Next(1500, 2500));

            Log.L("Confirm save");
            RCI(846, 685, 985, 738);

            Wait(500);

            WaitUntil(() => CheckSky(), delegate { }, 20_000, 50);

            Wait(500);

            Log.L("Closing save popup");
            ClosePopup();

            Wait(400);

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
