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
            G();
            return P(256, 461) == Col(168, 43, 42);
        }

        public bool IsInPlayerProfile()
        {
            G();
            return P(405, 643) == Col(98, 87, 73) &&
            P(843, 650) == Col(98, 87, 73) &&
            P(401, 278) == Col(75, 62, 52) &&
            P(667, 316) == Col(236, 192, 49) &&
            P(938, 316) == Col(52, 251, 61);
        }

        public void OpenGuild()
        {

            Log.I("Open guild");

            if (!CheckGCMenu())
            {
                Log.T($"{nameof(OpenGuild)} was called not in gc menu");
                throw new OnlineActionsException($"{nameof(OpenGuild)} was called not in gc menu");
            }

            RCI(1355, 411, 1422, 469);
            Wait(500);

            WaitUntil(() => IsInGuild() || CheckSky(), G, 20_000, 50);

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
            Log.I($"Open guild chat");
            if (!IsInGuild())
            {
                Log.T($"{nameof(OpenGuildChat)} called outside of guild");
                throw new OnlineActionsException($"{nameof(OpenGuildChat)} called outside of guild");
            }

            RCI(257, 553, 288, 603);
            Wait(500);

            WaitUntil(IsInGuild, G, 20_000, 50);
            Log.I($"Opened chat");
            Wait(300);

        }

        public void OpenGuildsTop()
        {
            Log.I($"Open guilds top");
            if (!IsInGuild())
            {
                Log.T($"{nameof(OpenGuildsTop)} called outside of guild");
                throw new OnlineActionsException($"{nameof(OpenGuildsTop)} called outside of guild");
            }

            RCI(257, 670, 289, 715);
            Wait(500);

            WaitUntil(IsInGuild, G, 20_000, 50);
            Log.I($"Top opened");
            Wait(300);

        }

        public void QuitGuild()
        {
            if (!IsInGuild())
            {
                Log.T($"{nameof(QuitGuild)} called outside of guild");
                throw new OnlineActionsException($"{nameof(QuitGuild)} called outside of guild");
            }

            WaitUntilDeferred(CheckGCMenu, () => RClick(500, 500), 1600, 500);

            if (!CheckGCMenu())
            {
                Log.T($"{nameof(QuitGuild)} couldn't quit top");
                throw new OnlineActionsException($"{nameof(QuitGuild)} couldn't quit top");
            }

            Wait(300);

        }

        /// <summary>
        /// Call when list of guild players is open
        /// </summary>
        public void CheckRandomProfileInGuild()
        {

            Log.I($"Open random profile in guild");

            for (int i = 0; i < 3; i++)
            {
                Mouse_Wheel(738, 584, 150);
                Wait(200);
            }
            Wait(700);

            Log.I($"Click on random player");
            RCI(364, 403, 1118, 698);

            Wait(300);

            WaitUntil(() => IsInPlayerProfile() || IsInGuild(), delegate { }, 20_000, 50);

            if (IsInPlayerProfile())
            {
                Log.I($"Player profile opened");
                Wait(rand.Next(2000, 5000));
                RClick(500, 500);
            }
            else
            {
                Log.I($"Didn't open profile");
            }
            Wait(300);

        }

        public void PerformGuildActions(OnlineActions actions)
        {
            actions &= OnlineActions.GuildActions;

            if((actions & OnlineActions.OpenGuild) == 0)
            {
                Log.I($"No guild actions");
                return;
            }

            OpenGuild();

            Log.I($"Guild opened");

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

            Log.I($"Close guild");
            QuitGuild();
            Log.I($"Guild closed");

        }

    }
}
