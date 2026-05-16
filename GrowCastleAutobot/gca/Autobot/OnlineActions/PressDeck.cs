using gca.Classes;
using gca.Classes.Exceptions;
using gca.Enums;
using System.Windows;
using static gca.Classes.Utils;

namespace gca
{
    public partial class Autobot
    {

        public bool HasDeckOnScreen()
        {
            return CheckGCMenu() && Pxl(559, 783) == Col(40, 127, 190);
        }

        public void PressDeck(int minTimes, int maxTimes)
        {

            Log.L("Press deck");
            if (!CheckGCMenu())
            {
                Log.T($"{nameof(PressDeck)}: was not in gc menu");
                throw new OnlineActionsException($"{nameof(PressDeck)}: was not in gc menu");
            }

            Wait(500);

            ClosePopup();

            CloseAdForCoins();

            if (!HasDeckOnScreen())
            {
                Log.T($"{nameof(PressDeck)}: no deck button");
                throw new OnlineActionsException($"{nameof(PressDeck)}: no deck button");
            }

            int reopenTimes = rand.Next(minTimes, maxTimes);
            int i = 0;
            do
            {
                ClosePopup();
                Log.L("Press deck");
                RCI(521, 772, 577, 832); // deck button
                Wait(rand.Next(1000, 3000));
                RCI(521, 772, 577, 832); // close deck list
                Wait(rand.Next(500, 1000));
                i++;
            } while (i < reopenTimes);

            Log.L("Finished deck opening");
            Wait(300);

        }
        public void PerformDeckActions(OnlineActions actions, int minTimes, int maxTimes)
        {
            if ((actions & OnlineActions.PressDeck) != 0)
            {
                PressDeck(minTimes, maxTimes);
            }
        }

    }
}
