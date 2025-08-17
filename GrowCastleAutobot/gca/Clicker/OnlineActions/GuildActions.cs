using gca.Classes;
using gca.Classes.Exceptions;
using gca.Clicker;
using gca.Enums;
using gca.Structs;
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
using static gca.Classes.Utils;

namespace gca
{
    public partial class MainWindow : Window
    {

        public bool IsInGuild(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(256, 461) == Col(168, 43, 42);
        }

        public bool IsInPlayerProfile(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(405, 643) == Col(98, 87, 73) &&
            P(843, 650) == Col(98, 87, 73) &&
            P(401, 278) == Col(75, 62, 52) &&
            P(667, 316) == Col(236, 192, 49) &&
            P(938, 316) == Col(52, 251, 61);
        }

        public void OpenGuild()
        {

            Log.L("Open guild");

            if (!CheckGCMenu())
            {
                Log.T($"{nameof(OpenGuild)} was called not in gc menu");
                throw new OnlineActionsException($"{nameof(OpenGuild)} was called not in gc menu");
            }

            RCI(1355, 411, 1422, 469);
            Wait(500);

            WaitUntil(() => IsInGuild() || CheckSky(false), delegate { }, 20_000, 50);

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

            WaitUntilDeferred(() => CheckGCMenu(), StepBack, 1600, 500);
            Wait(100);
        }

        public void OpenGuildChat()
        {
            Log.L($"Open guild chat");
            if (!IsInGuild())
            {
                Log.T($"{nameof(OpenGuildChat)} called outside of guild");
                throw new OnlineActionsException($"{nameof(OpenGuildChat)} called outside of guild");
            }

            RCI(257, 553, 288, 603);
            Wait(500);

            WaitUntil(() => IsInGuild(), delegate { }, 20_000, 50);
            Log.L($"Opened chat");
            Wait(300);

        }

        public void OpenGuildsTop()
        {
            Log.L($"Open guilds top");
            if (!IsInGuild())
            {
                Log.T($"{nameof(OpenGuildsTop)} called outside of guild");
                throw new OnlineActionsException($"{nameof(OpenGuildsTop)} called outside of guild");
            }

            RCI(257, 670, 289, 715);
            Wait(500);

            WaitUntil(() => IsInGuild(), delegate { }, 20_000, 50);
            Log.L($"Top opened");
            Wait(300);

        }

        public void QuitGuild()
        {
            if (!IsInGuild())
            {
                Log.T($"{nameof(QuitGuild)} called outside of guild");
                throw new OnlineActionsException($"{nameof(QuitGuild)} called outside of guild");
            }

            WaitUntilDeferred(() => CheckGCMenu(), StepBack, 1600, 500);

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

            Log.L($"Open random profile in guild");

            for (int i = 0; i < 3; i++)
            {
                Mouse_Wheel(738, 584, 150);
                Wait(200);
            }
            Wait(700);

            Log.L($"Click on random player");
            RCI(364, 403, 1118, 698);

            Wait(300);

            WaitUntil(() => IsInPlayerProfile() || IsInGuild(false), delegate { }, 20_000, 50);

            if (IsInPlayerProfile())
            {
                Log.L($"Player profile opened");
                Wait(rand.Next(2000, 5000));
                RClick(500, 500);
            }
            else
            {
                Log.L($"Didn't open profile");
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

            Log.L($"Guild opened");

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

            Log.L($"Close guild");
            QuitGuild();
            Log.L($"Guild closed");

        }

    }
}
