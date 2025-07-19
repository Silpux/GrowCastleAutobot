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
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static gca_clicker.Classes.Utils;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        public bool IsInGuild()
        {
            Getscreen();
            return Pxl(256, 461) == Col(168, 43, 42);
        }

        public bool IsInPlayerProfile()
        {
            Getscreen();
            return Pxl(405, 643) == Col(98, 87, 73) &&
            Pxl(1079, 639) == Col(98, 87, 73) &&
            Pxl(401, 278) == Col(75, 62, 52) &&
            Pxl(667, 316) == Col(236, 192, 49) &&
            Pxl(938, 316) == Col(52, 251, 61);
        }

        public void OpenGuild()
        {

            Log.I("Open guild");

            if (!CheckGCMenu())
            {
                Log.T($"{nameof(OpenGuild)} was called not in gc menu");
                throw new OnlineActionsException($"{nameof(OpenGuild)} was called not in gc menu");
            }

            RandomClickIn(1355, 411, 1422, 469);
            Wait(500);

            WaitUntil(() => IsInGuild() || CheckSky(), Getscreen, 20_000, 50);

            // list of your guild members is always opened, even if another section was open when closing

            if (!IsInGuild())
            {
                Log.T("Couldn't open guild");
                throw new OnlineActionsException("Couldn't open guild");
            }
            Wait(300);

        }

        public void CloseGuild()
        {
            if (!IsInGuild())
            {
                Log.T($"Called {nameof(CloseGuild)}, but guild wasn't open");
                throw new OnlineActionsException($"Called {nameof(CloseGuild)}, but guild wasn't open");
            }

            WaitUntilDeferred(CheckGCMenu, () => RClick(500, 500), 1600, 500);
            Wait(100);
        }

        public void OpenGuildChat()
        {
            if (!IsInGuild())
            {
                Log.T($"{nameof(OpenGuildChat)} called outside of guild");
                throw new OnlineActionsException($"{nameof(OpenGuildChat)} called outside of guild");
            }

            RandomClickIn(257, 553, 288, 603);
            Wait(500);

            WaitUntil(IsInGuild, Getscreen, 20_000, 50);
            Wait(300);

        }

        public void OpenGuildsTop()
        {
            if (!IsInGuild())
            {
                Log.T($"{nameof(OpenGuildsTop)} called outside of guild");
                throw new OnlineActionsException($"{nameof(OpenGuildsTop)} called outside of guild");
            }

            RandomClickIn(257, 670, 289, 715);
            Wait(500);

            WaitUntil(IsInGuild, Getscreen, 20_000, 50);
            Wait(300);

        }

        public void ExitGuild()
        {
            if (!IsInGuild())
            {
                Log.T($"{nameof(ExitGuild)} called outside of guild");
                throw new OnlineActionsException($"{nameof(ExitGuild)} called outside of guild");
            }

            WaitUntilDeferred(CheckGCMenu, () => RClick(500, 500), 1600, 500);

            if (!CheckGCMenu())
            {
                Log.T($"{nameof(ExitGuild)} couldn't quit top");
                throw new OnlineActionsException($"{nameof(ExitGuild)} couldn't quit top");
            }

        }

        /// <summary>
        /// Call when list of guild players is open
        /// </summary>
        public void CheckRandomProfileInGuild()
        {

            for (int i = 0; i < 3; i++)
            {
                Mouse_Wheel(738, 584, 150);
                Wait(200);
            }
            Wait(700);

            RandomClickIn(364, 403, 1118, 698);

            Wait(300);

            WaitUntil(() => IsInPlayerProfile() || IsInGuild(), delegate { }, 20_000, 50);

            if (IsInPlayerProfile())
            {
                Wait(rand.Next(2000, 5000));
                RClick(500, 500);
            }
            Wait(300);

        }

        public void PerformGuildActions(OnlineActions actions)
        {
            actions &= OnlineActions.GuildActions;

            if((actions & OnlineActions.OpenGuild) == 0)
            {
                return;
            }

            OpenGuild();
            Wait(rand.Next(3000, 6000));

            if ((actions & OnlineActions.OpenRandomProfileFromMyGuild) != 0)
            {
                CheckRandomProfileInGuild();
                Wait(rand.Next(3000, 6000));
            }

            List<Action> methods = new List<Action>(2);

            if ((actions & OnlineActions.OpenGuildChat) != 0)
            {
                methods.Add(() =>
                {
                    OpenGuildChat();
                    Wait(rand.Next(3000, 6000));
                });
            }

            if ((actions & OnlineActions.OpenGuildsTop) != 0)
            {
                methods.Add(() =>
                {
                    OpenGuildsTop();
                    Wait(rand.Next(3000, 6000));
                });
            }

            int n = methods.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                Action t = methods[k];
                methods[k] = methods[n];
                methods[n] = t;
            }

            foreach (var method in methods)
            {
                method();
            }

            ExitGuild();

        }

    }
}
