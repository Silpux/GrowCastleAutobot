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
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static gca_clicker.Classes.Utils;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {



        public bool IsInTop()
        {
            Getscreen();

            return Pxl(1022, 91) == Col(218, 218, 218) &&
            Pxl(1084, 91) == Col(218, 218, 218) &&
            Pxl(1110, 92) == Col(98, 87, 73) &&
            Pxl(995, 127) == Col(98, 87, 73) &&
            Pxl(1239, 91) == Col(242, 190, 35) &&
            Pxl(1374, 109) == Col(235, 170, 23);
        }

        /// <summary>
        /// Also checks if top is open
        /// </summary>
        /// <returns></returns>
        /// <exception cref="OnlineActionsException"></exception>
        public bool IsTopGlobalOpen()
        {
            if (!IsInTop())
            {
                throw new OnlineActionsException($"{nameof(IsTopGlobalOpen)} called outside of top");
            }
            return IsInTop() && Pxl(1399, 798) == Col(234, 229, 214);
        }

        /// <summary>
        /// Also checks if top is open
        /// </summary>
        /// <returns></returns>
        /// <exception cref="OnlineActionsException"></exception>
        public bool IsTopLocalOpen()
        {
            if (!IsInTop())
            {
                throw new OnlineActionsException($"{nameof(IsTopLocalOpen)} called outside of top");
            }
            return IsInTop() && Pxl(1400, 818) == Col(255, 196, 76);
        }

        public TopSection GetCurrentTopSection()
        {
            if (!IsInTop())
            {
                return TopSection.None;
            }

            if(Pxl(912, 195) == Col(98, 87, 73))
            {
                return TopSection.SeasonWaves;
            }

            if(Pxl(912, 326) == Col(98, 87, 73))
            {
                return TopSection.WavesOverall;
            }

            if(Pxl(912, 429) == Col(98, 87, 73))
            {
                return TopSection.HellSeason;
            }

            if(Pxl(912, 559) == Col(98, 87, 73))
            {
                return TopSection.HellOverall;
            }

            return TopSection.None;
        }


        public void OpenTop()
        {

            if (!CheckGCMenu())
            {
                Log.T($"{nameof(OpenTop)} called not in gc menu");
                throw new OnlineActionsException($"{nameof(OpenTop)} called not in gc menu");
            }

            Log.I("Open top");
            RandomClickIn(156, 777, 212, 827);
            Wait(500);

            WaitUntil(() => IsInTop() || CheckSky(), Getscreen, 20_000, 50);

            if (!IsInTop())
            {
                Log.T("Couldn't open top");
                throw new OnlineActionsException("Couldn't open top");
            }
            Wait(300);
        }

        /// <summary>
        /// Open when in top
        /// </summary>
        public void OpenTopSection(TopSection section)
        {

            // when selecting any section, local top is opened

            if (!IsInTop())
            {
                Log.T($"{nameof(OpenTopSection)} called outside of top");
                throw new OnlineActionsException($"{nameof(OpenTopSection)} called outside of top");
            }

            switch (section)
            {
                case TopSection.WavesOverall:
                    RandomClickIn(858, 304, 900, 363);
                    break;
                case TopSection.HellSeason:
                    RandomClickIn(853, 416, 899, 481);
                    break;
                default:
                case TopSection.SeasonWaves:
                    RandomClickIn(861, 188, 899, 245);
                    break;
            }
            Wait(500);

            WaitUntil(() => IsInTop() || CheckSky(), Getscreen, 20_000, 50);

            if (!IsInTop())
            {
                Log.T($"Top disappeared while inside of {nameof(OpenTopSection)} opening {section}");
                throw new OnlineActionsException($"Top disappeared while inside of {nameof(OpenTopSection)} opening {section}");
            }
            Wait(300);

        }

        /// <summary>
        /// throws if not in top
        /// </summary>
        /// <exception cref="OnlineActionsException"></exception>
        public void SwitchTop()
        {
            if (!IsInTop())
            {
                Log.T($"Top is not open when calling {nameof(SwitchTop)}");
                throw new OnlineActionsException($"Top is not open when calling {nameof(SwitchTop)}");
            }
            RandomClickIn(1380, 796, 1421, 835);

            Wait(500);
            WaitUntil(() => IsInTop() || CheckSky(), Getscreen, 20_000, 50);

            if (!IsInTop())
            {
                Log.T($"Top disappeared while inside of {nameof(SwitchTop)}");
                throw new OnlineActionsException($"Top disappeared while inside of {nameof(SwitchTop)}");
            }

            Wait(300);
        }

        /// <summary>
        /// throws if not in top
        /// </summary>
        /// <exception cref="OnlineActionsException"></exception>
        public void QuitTop()
        {
            if (!IsInTop())
            {
                Log.T($"{nameof(QuitTop)} called outside of top");
                throw new OnlineActionsException($"{nameof(QuitTop)} called outside of top");
            }

            WaitUntilDeferred(CheckGCMenu, () => RClick(500, 500), 1600, 500);

            if (!CheckGCMenu())
            {
                Log.T($"{nameof(QuitTop)} couldn't quit top");
                throw new OnlineActionsException($"{nameof(QuitTop)} couldn't quit top");
            }

        }

        public void PerformTopActions(OnlineActions actions)
        {
            actions &= OnlineActions.TopActions;

            if ((actions & OnlineActions.OpenTop) == 0)
            {
                return;
            }
            OpenTop();
            Wait(rand.Next(3000, 6000));

            if ((actions & OnlineActions.OpenTopWavesOverall) != 0)
            {
                actions |= OnlineActions.OpenTopWavesMy;
            }
            if ((actions & OnlineActions.OpenTopHellSeason) != 0)
            {
                actions |= OnlineActions.OpenTopHellSeasonMy;
            }

            TopSection currentTopSection = GetCurrentTopSection();
            if(currentTopSection == TopSection.None)
            {
                throw new OnlineActionsException($"Couldn't identify current top section");
            }

            if(currentTopSection != TopSection.SeasonWaves)
            {
                OpenTopSection(TopSection.SeasonWaves);
                Wait(rand.Next(3000, 6000));
            }

            if (IsTopGlobalOpen())
            {
                actions &= ~OnlineActions.OpenTopSeason;
                SwitchTop();
                Wait(rand.Next(3000, 6000));
            }

            if ((actions & OnlineActions.OpenTopSeason) != 0)
            {
                SwitchTop();
                Wait(rand.Next(3000, 6000));
            }

            List<Action> methods = new List<Action>(6);

            if ((actions & (OnlineActions.OpenTopWavesMy)) != 0)
            {
                Action m = () =>
                {
                    OpenTopSection(TopSection.WavesOverall);
                    Wait(rand.Next(3000, 6000));
                };

                if ((actions & OnlineActions.OpenTopWavesOverall) != 0)
                {
                    m += () =>
                    {
                        SwitchTop();
                        Wait(rand.Next(3000, 6000));
                    };
                }

                methods.Add(m);
            }

            if ((actions & (OnlineActions.OpenTopHellSeasonMy)) != 0)
            {
                Action m = () =>
                {
                    OpenTopSection(TopSection.HellSeason);
                    Wait(rand.Next(3000, 6000));
                };

                if ((actions & OnlineActions.OpenTopHellSeason) != 0)
                {
                    m += () =>
                    {
                        SwitchTop();
                        Wait(rand.Next(3000, 6000));
                    };
                }

                methods.Add(m);
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

        }

    }
}
