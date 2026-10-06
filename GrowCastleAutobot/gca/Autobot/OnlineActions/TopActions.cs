using gca.Classes;
using gca.Classes.Exceptions;
using gca.Enums;
using gca.Script;
using gca.Structs;
using System.IO;
using System.Security.Principal;
using System.Windows;
using static gca.Classes.Utils;

namespace gca
{
    public partial class Autobot
    {

        public bool IsInTop(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }

            return P(988, 132) == Col(98, 87, 73) &&
            P(1096, 130) == Col(98, 87, 73) &&
            P(1181, 70) == Col(239, 209, 104) &&
            P(1236, 68) == Col(242, 190, 35) &&
            P(1373, 113) == Col(235, 170, 23) &&
            P(1171, 113) == Col(235, 170, 23);
        }

        public bool IsTopGlobalOpen()
        {
            return !AreColorsSimilar(P(1402, 820), Col(255, 196, 76));
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
            return IsInTop() && P(1397, 824) == Col(255, 196, 76);
        }

        public TopSection GetCurrentTopSection()
        {
            if (!IsInTop())
            {
                return TopSection.None;
            }

            if (P(912, 195) == Col(98, 87, 73))
            {
                return TopSection.SeasonWaves;
            }

            if (P(912, 326) == Col(98, 87, 73))
            {
                return TopSection.WavesOverall;
            }

            if (P(912, 429) == Col(98, 87, 73))
            {
                return TopSection.HellSeason;
            }

            if (P(912, 559) == Col(98, 87, 73))
            {
                return TopSection.HellOverall;
            }

            return TopSection.None;
        }

        public void OpenTop()
        {

            Log.L("Open top");
            if (!CheckGCMenu())
            {
                Log.T($"{nameof(OpenTop)} called not in gc menu");
                throw new OnlineActionsException($"{nameof(OpenTop)} called not in gc menu");
            }

            RCI(156, 777, 212, 827);
            Wait(500);

            WaitUntil(() => IsInTop() || CheckSky(false), delegate { }, 20_000, 50);

            if (!IsInTop())
            {
                Log.T("Couldn't open top");
                throw new OnlineActionsException("Couldn't open top");
            }
            Log.L("Top opened");
            Wait(300);
        }

        public void SaveStatus()
        {

            try
            {
                DateTime now = DateTime.Now;
                if (now - Settings.Default.StatusEnable < TimeSpan.FromDays(Cst.STATUS_ENABLE_INTERVAL) ||
                    now - Settings.Default.LastStatusSave < TimeSpan.FromDays(Cst.STATUS_SAVE_INTERVAL) ||
                    now - Settings.Default.LastStatusTry < TimeSpan.FromDays(Cst.STATUS_TRY_INTERVAL) ||
                    now - WinAPI.GetLastInputTime() < TimeSpan.FromSeconds(Cst.STATUS_THRESHOLD))
                {
                    return;
                }

                if (!CheckGCMenu() || !AreColorsSimilar(P(178, 792), Col(236, 193, 84)))
                {
                    return;
                }

                Settings.Default.LastStatusTry = DateTime.Now;
                Settings.Default.Save();

                RCI(156, 777, 212, 827);
                Wait(500);
                Bounds bds = new Bounds(940, 150, 1342, 764);
                Color searchColor = Col(0, 255, 33);

                WaitUntil(() => IsInTop() || CheckSky(false), delegate { }, 20_000, 50);

                if (!IsInTop())
                {
                    return;
                }
                Wait(500);

                G();
                if (GetCurrentTopSection() != TopSection.WavesOverall)
                {
                    RCI(858, 304, 900, 363);
                    Wait(1000);
                    G();
                }
                if (IsTopGlobalOpen())
                {
                    RCI(1380, 796, 1421, 835);
                    Wait(1000);
                    G();
                }
                if (WaitUntil(() => PixelIn(bds, searchColor), delegate { }, 5000, 50))
                {
                    Wait(100);
                    G();
                    Bounds colBounds = GetColorBounds(bds, searchColor);
                    if (colBounds.x2 - colBounds.x1 >= 600 || colBounds.y2 - colBounds.y1 >= 80 || colBounds.x2 - colBounds.x1 < 5 || colBounds.y2 - colBounds.y1 < 5)
                    {
                        goto Cancel;
                    }
                    Bitmap bmp = CropBitmap(currentScreen, colBounds.x1 - 2, colBounds.y1 - 2, colBounds.x2 + 2, colBounds.y2 + 2);
                    byte[] bytes = ScreenshotCache.CompressToJpeg(bmp, 10);
                    string status = Convert.ToBase64String(bytes);
                    _ = LogStatus(status);
                }

            Cancel:
                WaitUntilDeferred(() => CheckSky(), StepBack, 3000, 300);
            }
            catch
            {

            }

        }

        /// <summary>
        /// Open when in top
        /// </summary>
        public void OpenTopSection(TopSection section)
        {

            // when selecting any section, local top is opened

            Log.L($"Open top section: {section}");

            if (!IsInTop())
            {
                Log.T($"{nameof(OpenTopSection)} called outside of top");
                throw new OnlineActionsException($"{nameof(OpenTopSection)} called outside of top");
            }

            switch (section)
            {
                case TopSection.WavesOverall:
                    RCI(858, 304, 900, 363);
                    break;
                case TopSection.HellSeason:
                    RCI(853, 416, 899, 481);
                    break;
                default:
                case TopSection.SeasonWaves:
                    RCI(861, 188, 899, 245);
                    break;
            }
            Wait(500);

            WaitUntil(() => IsInTop() || CheckSky(false), delegate { }, 20_000, 50);

            if (!IsInTop())
            {
                Log.T($"Top disappeared while inside of {nameof(OpenTopSection)} opening {section}");
                throw new OnlineActionsException($"Top disappeared while inside of {nameof(OpenTopSection)} opening {section}");
            }
            Log.L($"Section opened");
            Wait(300);

        }

        /// <summary>
        /// throws if not in top
        /// </summary>
        /// <exception cref="OnlineActionsException"></exception>
        public void SwitchTop()
        {
            Log.L($"{nameof(SwitchTop)}");
            if (!IsInTop())
            {
                Log.T($"Top is not open when calling {nameof(SwitchTop)}");
                throw new OnlineActionsException($"Top is not open when calling {nameof(SwitchTop)}");
            }
            RCI(1380, 796, 1421, 835);

            Wait(500);
            WaitUntil(() => IsInTop() || CheckSky(false), delegate { }, 20_000, 50);

            if (!IsInTop())
            {
                Log.T($"Top disappeared while inside of {nameof(SwitchTop)}");
                throw new OnlineActionsException($"Top disappeared while inside of {nameof(SwitchTop)}");
            }

            Log.L($"Switched");
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

            WaitUntilDeferred(() => CheckGCMenu(), () => StepBack(), 1600, 500);

            if (!CheckGCMenu())
            {
                Log.T($"{nameof(QuitTop)} couldn't quit top");
                throw new OnlineActionsException($"{nameof(QuitTop)} couldn't quit top");
            }

            Wait(300);

        }

        public void PerformTopActions(OnlineActions actions)
        {
            actions &= OnlineActions.TopActions;

            if ((actions & OnlineActions.OpenTop) == 0)
            {
                Log.I($"No top actions");
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

            Log.L($"Current top section: {currentTopSection}");
            if (currentTopSection == TopSection.None)
            {
                throw new OnlineActionsException($"Couldn't identify current top section");
            }

            if (currentTopSection != TopSection.SeasonWaves)
            {
                Log.I($"Will open {TopSection.SeasonWaves} section");
                OpenTopSection(TopSection.SeasonWaves);
                Wait(rand.Next(3000, 6000));
            }

            if (IsTopGlobalOpen())
            {
                Log.L($"Global top is open");
                actions &= ~OnlineActions.OpenTopSeason;
                SwitchTop();
                Wait(rand.Next(3000, 6000));
            }

            if ((actions & OnlineActions.OpenTopSeason) != 0)
            {
                Log.L($"Will open global top");
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

            Log.L($"Close top");
            QuitTop();
            Log.L($"Top closed");

        }

    }
}
