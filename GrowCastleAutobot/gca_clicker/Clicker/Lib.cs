using gca_clicker.Classes;
using gca_clicker.Clicker;
using gca_clicker.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using static gca_clicker.Classes.Utils;

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

        public bool CheckNoxMainMenu()
        {
            Getscreen();
            currentScreen = Colormode(5, 800, 148, 1000, 151, currentScreen);
            return Pxl(635, 96) != Cst.White &&
                Pxl(843, 93) != Cst.White &&
                Pxl(1057, 94) != Cst.White &&
                Pxl(599, 197) != Cst.White &&
                Pxl(847, 199) != Cst.White &&
                Pxl(1070, 197) != Cst.White &&
                (Pxl(806, 150) == Cst.White || Pxl(806, 150) == Col(191, 191, 191)) &&
                (Pxl(991, 149) == Cst.White || Pxl(991, 149) == Col(191, 191, 191));
        }

        public bool CaptchaOnScreen()
        {
            Getscreen();
            return Pxl(403, 183) == Col(98, 87, 73) &&
                Pxl(722, 305) == Col(189, 165, 127) &&
                Pxl(723, 591) == Col(189, 165, 127);

        }

        public TimeSpan GetCurrentBattleLength()
        {
            if (!solvingCaptcha)
            {
                return DateTime.Now - lastReplayTime;
            }
            return TimeSpan.Zero;
        }

        public void ShowBattleLength()
        {
            Log.I($"Battle length: {GetCurrentBattleLength():hh\\:mm\\:ss\\.fffffff}");
        }
        public void SetDefaultNoxState(nint hWnd)
        {
            WinAPI.RestoreWindow(hWnd);
            WinAPI.SetWindowPos(hWnd, hWnd, 0, 0, Cst.WINDOW_WIDTH, Cst.WINDOW_HEIGHT, WinAPI.SWP_NOZORDER);
        }

        public bool CheckNoxState()
        {
            (int x, int y, int width, int height) = GetWindowInfo(hwnd);
            if (x == -32000 && y == -32000)
            {
                Log.E($"Fix nox state");
                SetDefaultNoxState(hwnd);
                return false;
            }
            return true;
        }

        public bool WaitUntil(Func<bool> condition, Action actionBetweenChecks, int timeoutMs, int checkInterval)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                if (condition()) return true;
                Wait(checkInterval);
                actionBetweenChecks();
            }
            return false;
        }

        public void CollectMimic()
        {
            if (mimicOpened || dungeonFarm || !(Pxl(810, 93) == Cst.Black))
            {
                return;
            }
            Log.I("Mimic check");
            currentScreen = Colormode(4, currentScreen);
            if (PixelIn(437, 794, 1339, 829, Cst.White, out var ret))
            {
                double mimic_randomizer = new Random().NextDouble() * 100.0;
                if (mimic_randomizer <= mimicCollectPercent)
                {
                    Log.I("Collect mimic");
                    RandomClickIn(ret.Item1, ret.Item2, ret.Item1 + 10, ret.Item2 + 10);
                }
                else
                {
                    Log.I("Ignore mimic");
                }
            }
            mimicOpened = true;
        }

        public void CheckExitPanel()
        {

            if (Pxl(444, 481) == Col(227, 197, 144) &&
            Pxl(464, 494) == Col(167, 118, 59) &&
            Pxl(693, 491) == Col(167, 118, 59) &&
            Pxl(681, 540) == Col(120, 85, 43) &&
            Pxl(828, 489) == Col(242, 190, 35) &&
            Pxl(829, 540) == Col(235, 170, 23))
            {

                Log.W("Close quit window");

                LClick(571, 514);
                Wait(50);
                Getscreen();
            }

        }

        public void CheckPausePanel()
        {

            if (Pxl(470, 378) == Col(97, 86, 73) &&
            Pxl(504, 483) == Col(167, 118, 59) &&
            Pxl(690, 480) == Col(167, 118, 59) &&
            Pxl(516, 540) == Col(120, 85, 43) &&
            Pxl(693, 538) == Col(120, 85, 43) &&
            Pxl(784, 481) == Col(239, 209, 104) &&
            Pxl(1024, 536) == Col(235, 170, 23) &&
            Pxl(869, 486) == Col(242, 190, 35))
            {

                Log.W("pause exit");

                LClick(571, 514);
                Wait(50);
                Getscreen();
            }

        }

        public void CheckSkipPanel()
        {
            if (Pxl(502, 413) == Col(239, 209, 104) &&
            Pxl(579, 427) == Col(242, 190, 35) &&
            Pxl(896, 411) == Col(239, 209, 104) &&
            Pxl(982, 422) == Col(242, 190, 35) &&
            Pxl(783, 461) == Col(235, 170, 23))
            {

                RClick(1157, 466);
                Log.W("skip exit");
                Wait(50);
                Getscreen();

            }
        }

        /// <summary>
        /// true if lost on AB
        /// </summary>
        /// <returns></returns>
        public bool CheckLoseABPanel()
        {

            if (Pxl(526, 277) == Col(98, 87, 73) &&
            Pxl(555, 281) == Cst.White &&
            Pxl(717, 281) == Cst.White &&
            Pxl(516, 372) == Col(75, 62, 52) &&
            Pxl(965, 363) == Col(75, 62, 52) &&
            Pxl(611, 604) == Col(98, 87, 73) &&
            Pxl(878, 594) == Col(98, 87, 73) &&
            Pxl(668, 573) == Col(239, 209, 104) &&
            Pxl(802, 580) == Col(242, 190, 35) &&
            Pxl(808, 622) == Col(235, 170, 23))
            {

                RClick(1157, 466);
                Log.W("ab lost window exit");
                Wait(50);
                Getscreen();
                return true;
            }
            return false;
        }

        public void CheckHeroPanel()
        {

            if (Pxl(768, 548) == Col(239, 72, 90) &&
            Pxl(875, 547) == Col(239, 72, 90) &&
            Pxl(742, 607) == Col(216, 51, 59) &&
            Pxl(871, 607) == Col(216, 51, 59))
            {

                Log.W("hero quit");
                RClick(518, 404);
                Wait(100);
                RClick(518, 404);
                Wait(100);
                Getscreen();
            }

        }

        public void CheckRunePanel()
        {
            if (!dungeonFarm || !PixelIn(692, 435, 1079, 711, Col(239, 209, 104)))
            {
                return;
            }
            Wait(300);
            Getscreen();
            if (PixelIn(692, 435, 1079, 711, Col(239, 209, 104), out var ret) && dungeonNumber > 6)
            {
                if (screenshotRunes)
                {
                    Screenshot(currentScreen, Cst.SCREENSHOT_RUNES_PATH);
                }
                Log.I("rune collecting");
                LClick(ret.Item1, ret.Item2);
                Wait(100);
            }
        }

        public void CheckABExitPanel()
        {

            if (Pxl(788, 506) == Col(216, 51, 59))
            {
                Wait(200);
                Log.I("ab quit");
                RClick(518, 404);
                Getscreen();
            }

        }

        public int CountCrystals(bool lightMode)
        {
            Log.T($"Counting crystals LM: {lightMode}");
            System.Drawing.Color crystalWhiteColor = Cst.White;
            if (!lightMode)
            {
                crystalWhiteColor = Col(89, 89, 89);
            }
            int upgxmin = 288;
            int upgxmax = 464;
            int upgymin = 40;
            int upgymax = 70;
            int counterx = upgxmax;
            int foundmin = 0;
            int foundmax = -999;
            int crystalsWidth2 = 12;
            int crystalsWidth3 = 18;
            int crystalsWidth4 = 33;
            int crystals_2_width = 13;
            Getscreen();
            while (counterx > upgxmin && foundmax == -999)
            {
                if (PixelIn(counterx, upgymin, counterx, upgymax, crystalWhiteColor))
                {
                    foundmax = counterx;
                }
                else
                {
                    counterx--;
                }
            }
            if (foundmax > 432)
            {
                crystalsWidth2 = 15;
                crystalsWidth3 = 21;
                crystalsWidth4 = 38;
                crystals_2_width = 15;
                counterx = foundmax - 50;
                Log.T("no oranges");
            }
            else if (foundmax == -999)
            {
                Log.T("count crystals: wrong color");
                return 0;
            }
            else
            {
                Log.T("has oranges");
                counterx = foundmax - 45;
            }
            while (counterx < upgxmax && foundmin == 0)
            {
                if (PixelIn(counterx, upgymin, counterx, upgymax, crystalWhiteColor))
                {
                    foundmin = counterx;
                }
                else
                {
                    counterx++;
                }
            }
            int crystalsCountResult = 0;
            if (foundmax != 0 && foundmin != 0)
            {
                if (foundmax - foundmin > crystalsWidth2)
                {
                    crystalsCountResult = 0;
                }
                if (foundmax - foundmin > crystalsWidth3)
                {
                    crystalsCountResult = 10;
                }
                if (foundmax - foundmin > crystalsWidth4)
                {
                    crystalsCountResult = 20;
                    if (lightMode)
                    {
                        counterx = foundmin;
                        foundmin = 0;
                        foundmax = 0;
                        for (; counterx < upgxmax; counterx++)
                        {
                            if (foundmin != 0)
                            {
                                break;
                            }
                            if (Pxl(counterx, 67) == crystalWhiteColor)
                            {
                                foundmin = counterx;
                            }
                        }
                        while (counterx < upgxmax && foundmax == 0)
                        {
                            if (Pxl(counterx, 67) != crystalWhiteColor)
                            {
                                foundmax = counterx;
                            }
                            else
                            {
                                counterx++;
                            }
                        }
                        if (foundmax != 0 && foundmin != 0 && foundmax - foundmin < crystals_2_width)
                        {
                            crystalsCountResult = 30;
                        }
                    }
                }
            }
            Log.T($"Cyrstals: {crystalsCountResult}");
            return crystalsCountResult;
        }

        public void ItemDrop(ItemGrade itemGrade, int lineNumber)
        {
            if (!wrongItem)
            {
                Log.I("Correct item");

                if (lineNumber != 0)
                {
                    string line = File.ReadLines(Cst.DUNGEON_STATISTICS_PATH).Skip(lineNumber - 1).FirstOrDefault()!;
                    int itemsCount = -1;
                    try
                    {
                        itemsCount = int.Parse(Regex.Replace(line, "[^0-9]", ""));
                        ReplaceLine(Cst.DUNGEON_STATISTICS_PATH, lineNumber, $"{itemGrade.ToString()}: {itemsCount + 1}");
                    }
                    catch
                    {
                        itemsCount = -1;
                        File.WriteAllText(Cst.DUNGEON_STATISTICS_PATH, Cst.DEFAULT_DUNGEON_STATISTICS);
                    }
                }
            }
            else
            {
                Log.W("wrong item");

                if (PixelIn(335, 188, 1140, 700, Col(134, 163, 166))) itemGrade = ItemGrade.B;
                else if (PixelIn(335, 188, 1140, 700, Col(24, 205, 235))) itemGrade = ItemGrade.A;
                else if (PixelIn(335, 188, 1140, 700, Col(237, 14, 212))) itemGrade = ItemGrade.S;
                else if (PixelIn(335, 188, 1140, 700, Col(227, 40, 44))) itemGrade = ItemGrade.L;
                else if (PixelIn(335, 188, 1140, 700, Col(255, 216, 0))) itemGrade = ItemGrade.E;

                wrongItem = false;
            }

            (bool deleteCurrentItem, System.Drawing.Color dustColor, string screenshotPath) = itemGrade switch
            {
                ItemGrade.B => (deleteB, Col(134, 163, 166), Cst.SCREENSHOT_ITEMS_B_PATH),
                ItemGrade.A => (deleteA, Col(24, 205, 235), Cst.SCREENSHOT_ITEMS_A_PATH),
                ItemGrade.S => (deleteS, Col(237, 14, 212), Cst.SCREENSHOT_ITEMS_S_PATH),
                ItemGrade.L => (deleteL, Col(227, 40, 44), Cst.SCREENSHOT_ITEMS_L_PATH),
                ItemGrade.E => (deleteE, Col(227, 40, 44), Cst.SCREENSHOT_ITEMS_E_PATH),
                _ => (false, Col(134, 163, 166), Cst.SCREENSHOT_ITEMS_B_PATH)
            };

            Log.I($"Item grade: {itemGrade}. Delete: {deleteCurrentItem}");

            if (deleteCurrentItem)
            {
                Log.I("Delete item");
                if (PixelIn(335, 188, 1140, 700, dustColor, out (int x, int y) ret))
                {
                    RandomDblClickIn(ret.x - 30, ret.y + 10, ret.x + 30, ret.y + 60);
                    Wait(50);
                    Getscreen();
                    Log.I("Deleted");
                }
                else
                {
                    Log.W("Didn't see delete button");
                }
            }
            else
            {
                Log.I("Collect item");
                Getscreen();

                if (screenshotItems)
                {
                    Screenshot(currentScreen, screenshotPath);
                }

                if (PixelIn(335, 188, 1140, 700, Col(239, 209, 104), out (int x, int y) ret))
                {
                    Log.I("Click GET");
                    RandomClickIn(ret.x, ret.y, ret.x + 130, ret.y + 60);
                    Wait(100);
                    Getscreen();
                }
                else
                {
                    Log.W("Didn't see GET button");
                }

            }
        }

        public void AddSpeed()
        {
            if (DateTime.Now - lastAddSpeed > addSpeedCheckInterval)
            {

                if (PxlCount(20, 757, 121, 813, Cst.Black) > 500)
                {
                    if (CheckNoxState())
                    {

                        Log.W("add speed");

                        RandomClickIn(79, 778, 99, 798);
                        Wait(100);

                    }

                }

                lastAddSpeed = DateTime.Now;

            }

        }

        public void ChronoClick(out bool cancel)
        {
            cancel = false;
            if (thisChronoSlot != -1)
            {
                if (!CheckGCMenu() && Pxl(chronoX, chronoY) == Cst.BlueLineColor)
                {
                    Log.T($"Chrono click");
                    RandomClickIn(chronoX1, chronoY1, chronoX2, chronoY2);
                    if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                    {
                        cancel = true;
                    }
                    Getscreen();
                }
            }
        }

        public void EnterGC()
        {
            LClick(843, 446);

            if (fixedLoadingWait > 0)
            {
                Log.W($"[EnterGC] wait fixed {fixedLoadingWait} ms.");
                Wait(fixedLoadingWait);
            }

            Log.I($"gc click[EnterGC] wait 20s for gc open");

            if (WaitUntil(CheckGCMenu, delegate { }, 20_000, 200))
            {
                Wait(200);
                Log.I("gc opened[EnterGC]");
                restarted = true;
                lastReplayTime = DateTime.Now;
            }
            else
            {
                if (screenshotIfLongGCLoad)
                {
                    Screenshot(currentScreen, Cst.SCREENSHOT_LONG_GC_LOAD_PATH);
                }
                Log.E("too long loading. restarting.[EnterGC]");
            }
        }

        public void Reset()
        {
            Log.E("Nox Reset");
            LClick(1499, 333);
            Log.E("reset click");
            Wait(500);
            Move(1623, 333);
            Wait(5000);
            Log.E("wait up to 2 minutes for nox load[reset]");
            Getscreen();
            if (WaitUntil(() => Pxl(838, 150) == Cst.White, Getscreen, 120000, 1000))
            {
                Log.I("4s wait");
                Wait(4000);
                Log.I("nox opened");
                EnterGC();
                return;
            }
            Getscreen();
            if (screenshotNoxLoadFail)
            {
                Screenshot(currentScreen, Cst.SCREENSHOT_NOX_LOAD_FAIL_PATH);
            }
            Log.C("nox load stuck. [reset]");
            Log.C("window is overlapped by sth or wrong nox path. [reset]");
            Log.C("stopped. [reset]");
            Halt();
        }

        public void MakeCleanup()
        {
            Log.I("Do cleanup");
            bool closedGC = false;

            Log.I("open recent");

            LClick(1488, 833);

            Wait(300);
            Log.I("wait for clear all button");

            if (WaitUntil(() => PixelIn(985, 91, 1101, 131, Cst.White), Getscreen, 3000, 30))
            {
                Log.I("clear all button detected");
                Log.I("close recent apps");

                Wait(400);

                LClick(1062, 113);

                Log.I("wait for nox main menu");

                if (WaitUntil(CheckNoxMainMenu, delegate { }, 5000, 100))
                {
                    Wait(700);
                    Log.I("nox main menu opened");
                    closedGC = true;
                }
                else
                {
                    if (screenshotNoxLoadFail)
                    {
                        Screenshot(currentScreen, Cst.SCREENSHOT_NOX_LOAD_FAIL_PATH);
                    }
                    Log.E("nox main menu loading too long. restarting[restart]");
                }

            }
            else
            {
                if (screenshotClearAllFail)
                {
                    Screenshot(currentScreen, Cst.SCREENSHOT_CLEARALL_FAIL_PATH);
                }
                Log.E("cant see clear all button.");
            }

            if (closedGC)
            {
                LClick(1499, 288);
                Wait(200);
                Move(1450, 288);
                Log.I("Cleanup click. wait 7s");
                Wait(7000);
                EnterGC();
                lastCleanupTime = DateTime.Now;
            }
            else
            {
                Log.E("Cleanup fail");
            }

        }

        public void Restart()
        {
            Log.I("Restart");
            int restartCounter = 0;
            restarted = false;

            while (!restarted && restartCounter < maxRestartsForReset + 1)
            {

                restartCounter++;
                Log.I($"Restart {restartCounter}");

                if (restartCounter < maxRestartsForReset + 1)
                {
                    LClick(1488, 833);
                    Wait(300);

                    Log.I($"wait for clear all button");

                    if (WaitUntil(() => PixelIn(985, 91, 1101, 131, Cst.White), Getscreen, 3000, 30))
                    {

                        Log.I($"close recent apps");

                        Wait(400);
                        LClick(1062, 113);

                        Log.I($"wait for nox main menu");

                        if (WaitUntil(CheckNoxMainMenu, delegate { }, 5000, 100))
                        {
                            Wait(700);
                            Log.I($"nox main menu opened");
                            EnterGC();
                        }
                        else
                        {
                            if (screenshotNoxMainMenuLoadFail)
                            {
                                Screenshot(currentScreen, Cst.SCREENSHOT_NOX_MAIN_MENU_LOAD_FAIL_PATH);
                            }
                            Log.E($"nox main menu loading too long. restarting[restart]");
                        }
                    }
                    else
                    {
                        if (screenshotClearAllFail)
                        {
                            Screenshot(currentScreen, Cst.SCREENSHOT_CLEARALL_FAIL_PATH);
                        }
                        Log.E($"cant see clear all button.");
                    }

                }
                else
                {
                    Log.E($"{maxRestartsForReset} restarts in a row made. nox reset will be called");
                    Reset();
                }
            }
            if (!restarted)
            {
                Log.C($"Unknown problem. couldnt load gc.");
                Halt();
            }
        }

        public void UpgradeTower()
        {
            Log.I($"[tower upgrade] called");

            CountCrystals(true);

            if (CountCrystals(true) > 7)
            {

                Log.I($">7 crystals. castle open [tower upgrade]");

                LClick(440, 557); // Open castle

                Wait(200);

                switch (floorToUpgrade)
                {
                    case 1:
                        LClick(440, 557);
                        break;
                    case 2:
                        LClick(440, 455);
                        break;
                    case 3:
                        LClick(440, 346);
                        break;
                    case 4:
                        LClick(440, 233);
                        break;
                    default:
                        Log.E($"Wrong floor to upgrade");
                        return;
                }

                Wait(800);

                Getscreen();

                int cyanPxls = PxlCount(958, 586, 1126, 621, Col(0, 221, 255));

                Log.T($"Cyan pxls: {cyanPxls}");

                if (cyanPxls < 50 || cyanPxls > 150)
                {
                    Log.W($"Tower is not crystal upgradable. quit tower upgrading");
                    upgradeCastle = false;
                    Dispatcher.Invoke(() =>
                    {
                        UpgradeCastleCheckbox.Background = new SolidColorBrush(Colors.Red);
                    });
                    RClick(1157, 466);
                    Wait(200);
                    RClick(1157, 466);
                    Wait(200);
                    return;
                }

                RandomClickIn(956, 558, 1112, 603);
                Wait(300);

                int upgradeCounter = 0;
                int maxUpgradesInRow = 90;

                while ((CountCrystals(false) > 7) && (upgradeCounter < maxUpgradesInRow))
                {

                    cyanPxls = PxlCount(958, 586, 1126, 621, Col(0, 221, 255));
                    Log.T($"Cyan pxls: {cyanPxls}");

                    if (cyanPxls < 50 || cyanPxls > 150)
                    {
                        Log.W($"not seeing correct upgrade button. quit upgrading");
                        upgradeCastle = false;
                        Dispatcher.Invoke(() =>
                        {
                            UpgradeCastleCheckbox.Background = new SolidColorBrush(Colors.Red);
                        });
                        upgradeCounter = maxUpgradesInRow;
                    }
                    else
                    {
                        RandomClickIn(958, 554, 1108, 606);
                        Wait(300);
                        upgradeCounter++;
                    }

                }
                Wait(200);
                Getscreen();

                if ((Pxl(788, 698) == Col(98, 87, 73)) && (Pxl(748, 758) == Col(98, 87, 73)))
                {
                    Log.E($"reached max tower level");
                    LClick(788, 712);
                    Wait(300);
                }

                RClick(1157, 466);
                Wait(200);
                RClick(1157, 466);
                Wait(200);
                RClick(1157, 466);
                Wait(200);
            }
            else
            {
                Log.W($"no upgrading [tower upgrade]");

            }

            Getscreen();

        }

        public void GetItem()
        {

            Log.I($"GetItem");

            // Col(134, 163, 166)    b stone
            // Col(24, 205, 235)    a stone
            // Col(237, 14, 212)    s stone
            // Col(227, 40, 44)     l stone

            // Col(218, 218, 218) b word color
            // Col(68, 255, 218)  a word color
            // Col(244, 86, 233)  s word color
            // Col(255, 50, 50)   l word color
            // Col(255, 216, 0)   e word color

            bool noItem = false;
            if (WaitUntil(() =>
            {
                if (PixelIn(335, 188, 1140, 700, Col(239, 209, 104)))
                {
                    return true;
                }
                if (CheckGCMenu())
                {
                    noItem = true;
                    return true;
                }
                return false;
            }, Getscreen, 1050, 30))
            {
                Log.I($"item dropped");
                Wait(50);
                Getscreen();

                switch (dungeonNumber)
                {
                    case 1:

                        bool correctItem = false;
                        if (PixelIn(397, 134, 1116, 440, Col(218, 218, 218), out (int x, int y) ret))
                        {
                            if (PxlCount(ret.x - 5, ret.y - 5, ret.x + 5, ret.y + 5, Cst.White) == 0)
                            {
                                ItemDrop(ItemGrade.B, 0);
                                correctItem = true;
                            }
                        }
                        if (!correctItem)
                        {
                            wrongItem = true;
                            ItemDrop(ItemGrade.None, 0);
                        }
                        break;

                    case 2:
                        if (PixelIn(397, 134, 1116, 440, Col(68, 255, 218))) // A
                        {
                            ItemDrop(ItemGrade.A, 3);
                        }
                        else if (PixelIn(397, 134, 1116, 440, Col(218, 218, 218))) // B
                        {
                            ItemDrop(ItemGrade.B, 2);
                        }
                        else
                        {
                            wrongItem = true;
                            ItemDrop(ItemGrade.None, 0);
                        }
                        break;
                    case 3:

                        if (PixelIn(397, 134, 1116, 440, Col(244, 86, 233))) // S
                        {
                            ItemDrop(ItemGrade.S, 8);
                        }
                        else if (PixelIn(397, 134, 1116, 440, Col(68, 255, 218))) // A
                        {
                            ItemDrop(ItemGrade.A, 7);
                        }
                        else if (PixelIn(397, 134, 1116, 440, Col(218, 218, 218))) // B
                        {
                            ItemDrop(ItemGrade.B, 6);
                        }
                        else
                        {
                            wrongItem = true;
                            ItemDrop(ItemGrade.None, 0);
                        }
                        break;
                    case 4:
                        if (PixelIn(397, 134, 1116, 440, Col(244, 86, 233))) // S
                        {
                            ItemDrop(ItemGrade.S, 13);
                        }
                        else if (PixelIn(397, 134, 1116, 440, Col(68, 255, 218))) // A
                        {
                            ItemDrop(ItemGrade.A, 12);
                        }
                        else if (PixelIn(397, 134, 1116, 440, Col(218, 218, 218))) // B
                        {
                            ItemDrop(ItemGrade.B, 11);
                        }
                        else
                        {
                            wrongItem = true;
                            ItemDrop(ItemGrade.None, 0);
                        }
                        break;
                    case 5:
                        if (PixelIn(397, 134, 1116, 440, Col(68, 255, 218))) // A
                        {
                            ItemDrop(ItemGrade.A, 16);
                        }
                        else if (PixelIn(397, 134, 1116, 440, Col(244, 86, 233))) // S
                        {
                            ItemDrop(ItemGrade.S, 17);
                        }
                        else if (PixelIn(397, 134, 1116, 440, Col(255, 50, 50))) // L
                        {
                            ItemDrop(ItemGrade.L, 18);
                        }
                        else
                        {
                            wrongItem = true;
                            ItemDrop(ItemGrade.None, 0);
                        }
                        break;
                    case 6:
                        if (PixelIn(397, 134, 1116, 440, Col(68, 255, 218))) // A
                        {
                            ItemDrop(ItemGrade.A, 21);
                        }
                        else if (PixelIn(397, 134, 1116, 440, Col(244, 86, 233))) // S
                        {
                            ItemDrop(ItemGrade.S, 22);
                        }
                        else if (PixelIn(397, 134, 1116, 440, Col(255, 216, 0))) // E
                        {
                            ItemDrop(ItemGrade.E, 23);
                        }
                        else
                        {
                            wrongItem = true;
                            ItemDrop(ItemGrade.None, 0);
                        }
                        break;
                }
            }

            else
            {
                if (CheckGCMenu())
                {
                    Log.E($"Cant see GET button. rClick");
                    RClick(518, 404);
                    Wait(100);
                    Getscreen();
                }
            }

        }

        public void CheckOnHint()
        {
            bool hintDetected = false;

            if (!CheckSky() && Pxl(19, 315) == Cst.SkyColor)
            {

                Log.W($"hint check 1");
                Wait(200);
                Getscreen();

                if (!CheckSky() && Pxl(19, 315) == Cst.SkyColor)
                {

                    Log.E($"hint check 2");
                    Wait(250);
                    Getscreen();

                    if (!CheckSky() && Pxl(19, 315) == Cst.SkyColor)
                    {
                        Log.E($"hint check 3");
                        Wait(400);
                        Getscreen();

                        if (!CheckSky() && Pxl(19, 315) == Cst.SkyColor)
                        {
                            Screenshot(currentScreen, Cst.SCREENSHOT_HINT_PATH);
                            Log.E($"unknown hint detected");
                            hintDetected = true;
                        }

                    }

                }
            }

            if (hintDetected)
            {

                Screenshot(currentScreen, Cst.SCREENSHOT_HINT_PATH);
                Log.E($"___Hint detected___");
                Wait(3000);

                Getscreen();

                Screenshot(currentScreen, Cst.SCREENSHOT_HINT_PATH);

                Log.E($"___RESTART___");

                Restart();

                Log.E($"___RESTARTED___");
                Log.E($"30 s screenshotting");

                for (int i = 0; i < 10; i++)
                {
                    Screenshot(currentScreen, Cst.SCREENSHOT_HINT_PATH);
                    Log.E($"__Screen{i}__");
                    Wait(3000);
                    Getscreen();
                }

                Screenshot(currentScreen, Cst.SCREENSHOT_HINT_PATH);

            }

        }

        public void WaitForCancelABButton()
        {
            Log.I($"wait for cancel ab button");

            if (!waveCanceling)
            {
                replaysForUpgrade = 100; // to ensure that tower or hero will be upgraded for crystals after quitting ab
            }

            Getscreen();

            if (WaitUntil(() => Pxl(788, 506) != Col(216, 51, 59), Getscreen, 10_000, 200))
            {
                Wait(200);
                bool abLostPanel = false;

                if (WaitUntil(() => Pxl(788, 506) == Col(216, 51, 59) || abLostPanel,
                () => {

                    AddSpeed();

                    if (!CheckSky())
                    {
                        CheckPausePanel();
                        CheckExitPanel();

                        if (CheckLoseABPanel())
                        {
                            Log.E($"lost on AB [WaitForCancelABButton]");
                            abLostPanel = true;
                        }
                    }
                    Getscreen();

                }, 120_000, 50))
                {
                    Dispatcher.Invoke(() =>
                    {
                        ABTimerLabel.Content = string.Empty;
                    });
                    if (!abLostPanel)
                    {
                        Wait(50);
                        Log.I($"cancel button detected");
                        RClick(515, 404);
                        Wait(50);
                    }
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        ABTimerLabel.Content = string.Empty;
                    });
                    Log.E($"? ? ? ?");
                    Restart();
                }
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    ABTimerLabel.Content = string.Empty;
                });
                Log.E($"? o_O ?");
                Restart();
            }

        }

        public void ABWait(int secondsToWait)
        {

            int waveStartTimeout = 10_000;
            int waveFinishTimeout = 120_000;

            TimeSpan timeToWait = TimeSpan.FromSeconds(secondsToWait);

            Log.I($"AB wait mode for {timeToWait}");

            DateTime abStart = DateTime.Now;

            bool quitWaiting = false;
            int wavesCounter = 0;

            DateTime startTime;
            DateTime finishTime;

            bool quitOn30Crystals = false;

            while (DateTime.Now - abStart < timeToWait)
            {
                startTime = DateTime.Now;


                DateTime currentTimeout = DateTime.Now + TimeSpan.FromMilliseconds(waveFinishTimeout);

                if (!WaitUntil(() => !CheckSky() || DateTime.Now - abStart > timeToWait || quitWaiting,
                () =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        DateTime now = DateTime.Now;
                        ABTimerLabel.Content = $"AB wait {abStart + timeToWait - DateTime.Now:hh\\:mm\\:ss}\nWait for finish {currentTimeout - now:hh\\:mm\\:ss}\nWaves passed: {wavesCounter}";
                    });
                    AddSpeed();
                    if (breakABOn30Crystals)
                    {
                        if (CountCrystals(true) >= 30)
                        {
                            Log.I($"30 crystals reached. break AB mode");
                            timeToWait = TimeSpan.Zero;
                            skipNextWave = true;
                            quitOn30Crystals = true;
                        }
                    }
                }, waveFinishTimeout, 500))
                {
                    Log.E($"2min wave. restart gc");

                    if (screenshotOnEsc)
                    {
                        Screenshot(currentScreen, Cst.SCREENSHOT_AB_ERROR2_PATH);
                    }

                    quitWaiting = true;
                    timeToWait = TimeSpan.Zero;
                    Restart();
                }

                if (DateTime.Now - abStart < timeToWait && timeToWait != TimeSpan.Zero)
                {
                    Log.I($"Wave finished");
                }

                wavesCounter++;

                currentTimeout = DateTime.Now + TimeSpan.FromMilliseconds(waveStartTimeout);

                if (!WaitUntil(() => CheckSky() || DateTime.Now - abStart > timeToWait || quitWaiting,
                () =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        DateTime now = DateTime.Now;
                        ABTimerLabel.Content = $"AB wait {abStart + timeToWait - now:hh\\:mm\\:ss}\nWait for wave {currentTimeout - now:hh\\:mm\\:ss}\nWaves passed: {wavesCounter}";
                    });
                    if (CheckLoseABPanel())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ABTimerLabel.Content = $"Lost";
                        });
                        Log.E($"Lost on AB");
                        quitWaiting = true;
                        timeToWait = TimeSpan.Zero;
                    }
                    else
                    {
                        CheckPausePanel();
                        CheckExitPanel();
                    }

                }, waveStartTimeout, 50))
                {

                    Dispatcher.Invoke(() =>
                    {
                        ABTimerLabel.Content = $"Long wait";
                    });

                    Log.E($"10s closed sky");

                    if (screenshotOnEsc)
                    {
                        Screenshot(currentScreen, Cst.SCREENSHOT_AB_ERROR_PATH);
                    }

                    Restart();

                    quitWaiting = true;
                    timeToWait = TimeSpan.Zero;
                }

                finishTime = DateTime.Now;
                if (DateTime.Now - abStart < timeToWait && timeToWait != TimeSpan.Zero)
                {
                    Log.I($"Wave started. Previous wave duration: {finishTime - startTime:hh\\:mm\\:ss\\.fffffff}");
                }

            }

            if (!quitWaiting)
            {
                Dispatcher.Invoke(() =>
                {
                    if (quitOn30Crystals)
                    {
                        ABTimerLabel.Content = $"30 crystals collected\nWait for cancel AB button";
                    }
                    else
                    {
                        ABTimerLabel.Content = $"Wait for cancel AB button";
                    }
                });
                WaitForCancelABButton();
            }
        }

        public void PerformDungeonStart()
        {

            Getscreen();

            Log.I($"dungeon click. wait 15s for opening");

            RandomClickIn(699, 280, 752, 323);
            DateTime openDungeonTime = DateTime.Now;

            bool notAbleToOpenDungeons = false;

            bool openedDungeon = false;

            WaitUntil(() => Pxl(561, 676) == Col(69, 58, 48) || Pxl(858, 575) == Col(255, 185, 0) || notAbleToOpenDungeons,
            () =>
            {
                if (CheckSky() && DateTime.Now - openDungeonTime > TimeSpan.FromSeconds(3))
                {
                    notAbleToOpenDungeons = true;
                }
            }, 15_000, 30);

            openedDungeon = !notAbleToOpenDungeons;

            if (openedDungeon)
            {
                Log.I($"dungeon button detected. click on dungeon");

                if (solvingCaptcha && dungeonNumber > 6)
                {
                    Log.W($"captcha solving. green dragon click");

                    RandomClickIn(69, 179, 410, 229);
                    Wait(150);
                    RandomClickIn(1039, 728, 1141, 770);
                }
                else
                {
                    switch (dungeonNumber)
                    {
                        case 1:
                            RandomClickIn(57, 168, 371, 218);
                            break;
                        case 2:
                            RandomClickIn(539, 170, 903, 227);
                            break;
                        case 3:
                            RandomClickIn(1082, 166, 1368, 212);
                            break;
                        case 4:
                            RandomClickIn(57, 308, 302, 366);
                            break;
                        case 5:
                            RandomClickIn(544, 304, 891, 365);
                            break;
                        case 6:
                            RandomClickIn(1094, 301, 1367, 367);
                            break;
                        case 7:
                            RandomClickIn(160, 443, 414, 483);
                            break;
                        case 8:
                            RandomClickIn(625, 444, 879, 485);
                            break;
                        case 9:
                            RandomClickIn(1113, 438, 1361, 486);
                            break;
                    }

                    Wait(150);
                    RandomClickIn(1039, 728, 1141, 770);

                    if (solvingCaptcha)
                    {
                        Wait(400);
                    }
                    else
                    {
                        Wait(200);
                        Getscreen();
                        if (!simulateMouseMovement)
                        {
                            ChronoClick(out _);
                        }
                    }

                }
                Wait(400);

                if (!CheckSky())
                {
                    Log.E($"sky not clear[dungeon]");

                    Wait(300);
                    Getscreen();

                    if (Pxl(1077, 734) != Col(120, 85, 43))
                    {
                        Log.E($"probably inventory is full");
                        Log.E($"couldnt figth dungeon. captcha wasn't detected");

                        if (replaysIfDungeonDontLoad)
                        {
                            Log.I($"replays will be called");
                            dungeonFarm = false;
                            makeReplays = true;

                            Dispatcher.Invoke(() =>
                            {
                                FarmDungeonCheckbox.Background = new SolidColorBrush(Colors.Red);
                                ReplaysCheckbox.Background = new SolidColorBrush(Colors.Lime);
                            });
                        }

                        // close current dungeon
                        LClick(1165, 134);
                        Wait(100);

                        // close dungeons
                        LClick(1442, 122);
                        Wait(100);
                    }
                    else
                    {
                        Log.W($"captcha detected[dungeon]");
                    }
                }
                else
                {
                    Log.I($"dungeon started");

                    lastReplayTime = DateTime.Now;

                    if (deathAltar && dungeonNumber < 7)
                    {
                        Log.D($"Click altar");
                        RandomClickIn(116, 215, 172, 294);
                        HeroClickWait(ActivationWaitBreakCondition, delegate { });
                        deathAltarUsed = true;
                    }
                    else
                    {
                        if (dungeonNumber > 6 && dungeonStartCastOnBoss)
                        {
                            if (WaitUntil(() => Pxl(834, 94) == Col(232, 77, 77), Getscreen, 10_000, 100))
                            {
                                if (deathAltar)
                                {
                                    RandomClickIn(116, 215, 172, 294);
                                    HeroClickWait(ActivationWaitBreakCondition, delegate { });
                                    deathAltarUsed = true;
                                }
                                Wait(dungeonStartCastDelay);
                            }

                        }
                    }
                }
            }
            else
            {
                Log.E($"dungeon didnt load");

                if (replaysIfDungeonDontLoad)
                {
                    Log.E($"replays will be called");
                    dungeonFarm = false;
                    makeReplays = true;
                    Dispatcher.Invoke(() =>
                    {
                        FarmDungeonCheckbox.Background = new SolidColorBrush(Colors.Red);
                        ReplaysCheckbox.Background = new SolidColorBrush(Colors.Lime);
                    });
                }
                Wait(100);
            }

        }

        public void PerformOrcBandAndMilit()
        {
            if (thisOrcBandSlot != -1 && (!orcBandOnSkipOnly || isSkip))
            {
                Log.I($"orcband click");
                RandomClickIn(orcBandX1, orcBandY1, orcBandX2, orcBandY2);
                HeroClickWait(ActivationWaitBreakCondition, delegate { });
            }
            if (thisMilitaryFSlot != -1 && (!militaryFOnSkipOnly || isSkip))
            {
                Log.I($"militaryF click");
                RandomClickIn(militX1, militY1, militX2, militY2);
                HeroClickWait(ActivationWaitBreakCondition, delegate { });
            }
        }

        public void PutOnAB()
        {
            Log.I($"ab open");
            int currentWait = rand.Next(waitOnABButtonsMin, waitOnABButtonsMax + 1);
            Wait(currentWait);

            lastReplayTime = DateTime.Now;

            RandomClickIn(1236, 773, 1282, 819);

            currentWait = rand.Next(waitOnABButtonsMin, waitOnABButtonsMax + 1);
            Wait(currentWait);

            if (!abTab)
            {
                RandomClickIn(488, 457, 529, 491);
                currentWait = rand.Next(waitOnABButtonsMin, waitOnABButtonsMax + 1);
                Wait(currentWait);
            }

            RandomClickIn(656, 445, 821, 503);
            Wait(300);

        }

        public void PerformReplay()
        {
            Wait(50);

            Log.I("replay click");
            RandomClickIn(1124, 744, 1243, 814);
            Wait(200);
            RandomClickIn(940, 734, 1052, 790);
            if (solvingCaptcha)
            {
                Wait(400);
                return;
            }
            Wait(300);
            if (!CheckSky())
            {
                Log.E("sky not clear[replays]");
                Wait(400);
                if (CaptchaOnScreen())
                {
                    Log.W("captcha detected[replays]");
                }
            }
            else
            {
                if (!simulateMouseMovement)
                {
                    ChronoClick(out _);
                }
                lastReplayTime = DateTime.Now;
                Log.I("sky clear[replays]");
            }
        }

        public void PerformSkip()
        {
            if ((replaysForSkip > 5 || !fiveWavesPauseSkip || skipNextWave) && skipWaves)
            {

                if (skipNextWave)
                {
                    Log.I($"skip anyways");
                }
                else
                {
                    Wait(300); // skip panel can overlap crystal count when it goes down
                }

                if (skipWithOranges || skipNextWave || CountCrystals(true) >= 30)
                {

                    Wait(150);

                    if (!CheckSky() || CheckGCMenu())
                    {
                        Log.I($"battle is not open [Perform_skip]");
                    }
                    else
                    {
                        Log.I($"skip 30 click");
                        RandomClickIn(889, 411, 984, 496);
                        //RandomMoveIn(889, 411, 984, 496);
                        skipNextWave = false;

                        isSkip = true;
                        replaysForSkip = 0;

                        Wait(100);

                        if (!CheckSky())
                        {
                            Wait(350);
                            Getscreen();

                            if (skipWithOranges &&
                            Pxl(83, 182) == Col(98, 87, 73) &&
                            Pxl(1390, 195) == Col(98, 87, 73) &&
                            Pxl(97, 424) == Col(78, 64, 50) &&
                            Pxl(652, 420) == Col(78, 64, 50) &&
                            Pxl(926, 421) == Col(78, 64, 50))
                            {
                                RClick(1157, 466);
                                Wait(100);
                                RClick(1157, 466);
                                Log.E($"oranges are over");
                                skipWithOranges = false;
                                Dispatcher.Invoke(() =>
                                {
                                    SkipWithOrangesCheckbox.Background = new SolidColorBrush(Colors.Red);
                                });
                                Wait(100);
                                isSkip = false;
                            }
                        }
                    }
                }
                else
                {
                    isSkip = false;
                    Log.I($"<30 crystals. rClick");
                    RClick(1157, 466);
                    Wait(100);
                }
            }
            else
            {
                isSkip = false;
                Log.I($"no skip. esc click");
                RClick(1157, 466);
                Wait(100);
            }

        }

        public void CloseTop()
        {
            Getscreen();
            if (Pxl(260, 130) != Cst.SkyColor)
            {
                RandomClickIn(239, 95, 291, 162);
                Wait(100);
            }
        }

        public void PerformABMode()
        {

            Log.I($"Perform_AB_mode");

            if (skipWaves)
            {
                if (abSkipNum < 1)
                {
                    Wait(400);
                    if (CheckSky() && !CheckGCMenu())
                    {
                        Log.I($"sky clear on AB start [Perform_AB_mode, skipwaves]");

                        CloseTop();

                        PerformSkip();
                        PutOnAB();

                        int secondsToWait = rand.Next(secondsBetweenABSessionsMin, secondsBetweenABSessionsMax + 1);

                        Log.I($"AB {secondsToWait} seconds");

                        ABWait(secondsToWait);

                        lastReplayTime = DateTime.Now;

                        abSkipNum = rand.Next(skipsBetweenABSessionsMin, skipsBetweenABSessionsMax + 1) + 1;
                        Log.I($"{abSkipNum - 1} battles with skips");
                    }
                    else
                    {
                        Log.E($"sky not clear [Perform_AB_mode, skipwaves]");
                    }
                }
                else
                {
                    PerformSkip();
                    CloseTop();
                    PutOnAB();
                    waitForCancelABButton = true;
                    Dispatcher.Invoke(() =>
                    {
                        ABTimerLabel.Content = $"{abSkipNum} battles with skips left\nWait for cancel ab button";
                    });
                }
            }
            else
            {
                Wait(200);
                if (CheckSky() && !CheckGCMenu())
                {
                    Log.I($"sky clear on AB start [Perform_AB_mode, no skipwaves]");

                    CloseTop();
                    PutOnAB();

                    int secondsToWait = rand.Next(secondsBetweenABSessionsMin, secondsBetweenABSessionsMax + 1);
                    Log.I($"AB {secondsToWait} seconds");
                    ABWait(secondsToWait);

                    lastReplayTime = DateTime.Now;
                }
                else
                {
                    Log.E($"sky not clear [Perform_AB_mode, no skipwaves]");
                }
            }
        }

        public void PerformWaveCanceling()
        {
            Log.I($"Do wave canceling");
            PerformSkip();
            CloseTop();
            PutOnAB();
            lastReplayTime = DateTime.Now;
            waitForCancelABButton = true;
        }

        public void PerformManualBattleStart()
        {
            Log.I($"Do manual battle start");
            lastReplayTime = DateTime.Now;
            PerformSkip();
            CloseTop();
            ChronoClick(out _);
            PerformOrcBandAndMilit();
        }

        public void Replay()
        {
            mimicOpened = false;
            waitForAd++;
            replaysForUpgrade++;
            replaysForSkip++;
            pwTimer = false;
            abSkipNum--;
            healAltarUsed = false;
            deathAltarUsed = false;
            usedSingleClickHeros = false;
            Log.I("Do replay");

            if (!CheckSky() || !CheckGCMenu())
            {
                Log.E($"[Replay] Not in main menu");
                return;
            }

            if (dungeonFarm)
            {
                PerformDungeonStart();
                return;
            }
            if (Pxl(1038, 796) == Col(235, 170, 23) && Pxl(1038, 728) == Col(242, 190, 35) && Pxl(1320, 730) == Col(242, 190, 35) && Pxl(1320, 796) == Col(235, 170, 23))
            {
                Log.W("close replay buttons");
                LClick(1442, 672);
                Wait(300);
            }
            if (Pxl(933, 795) == Col(235, 170, 23) && Pxl(1114, 794) == Col(235, 170, 23) && Pxl(1293, 796) == Col(235, 170, 23))
            {
                Log.W("close hell buttons");
                LClick(1442, 672);
                Wait(300);
            }
            if (makeReplays)
            {
                PerformReplay();
                return;
            }
            Log.I("battle click");
            RandomClickIn(1319, 754, 1386, 785);
            Wait(battleClickWait);
            if (solvingCaptcha)
            {
                Log.W("solving captcha. wait");
                Wait(500);
                return;
            }
            waitForCancelABButton = false;
            if (autobattleMode)
            {
                PerformABMode();
            }
            else if (waveCanceling)
            {
                PerformWaveCanceling();
            }
            else
            {
                PerformManualBattleStart();
                return;
            }
            if (waitForCancelABButton)
            {
                if (CheckSky() && !CheckGCMenu())
                {
                    WaitForCancelABButton();
                }
                else
                {
                    Log.E("sky not clear after ab call");
                }
            }
        }

        public bool WaitIfDragonTimer()
        {
            Getscreen();
            if (dungeonNumber >= 7 || !(Pxl(605, 137) == Col(255, 79, 79)))
            {
                return false;
            }
            Wait(50);
            Log.I("dungeon farm: timer detected. waiting for timer ends");

            if (simulateMouseMovement)
            {
                RandomMoveIn(12, 669, 188, 848);
            }

            bool dungeonTimerDisappear = false;

            WaitUntil(() => dungeonTimerDisappear, delegate
            {
                if (Pxl(605, 137) == Col(255, 79, 79))
                {
                    Wait(30);
                    Getscreen();
                }
                else
                {
                    Wait(10);
                    Getscreen();
                    if (CheckSky())
                    {
                        Log.I("timer ended. click on speed");
                        RandomClickIn(50, 781, 95, 825);
                        Wait(100);
                        RandomClickIn(50, 781, 95, 825);
                        if (DateTime.Now - x3Timer < TimeSpan.FromSeconds(1200.0) || iHaveX3)
                        {
                            Wait(100);
                            RandomClickIn(50, 781, 95, 825);
                        }
                        dungeonTimerDisappear = true;
                        Log.I("wait 4s for item drop");
                        WaitUntil(() => !CheckSky(), Getscreen, 4000, 50);
                        ShowBattleLength();
                    }
                }
            }, 10000, 0);
            if (Pxl(1403, 799) != Col(152, 180, 28) && Pxl(1403, 799) != Col(195, 207, 209))
            {
                GetItem();
            }

            return true;
        }

        public void WaitForAdEnd(bool x3Ad)
        {
            if (fixedAdWait > 0)
            {
                Log.I($"[WaitForAdEnd] wait for {(float)fixedAdWait / 1000} s.");
                Wait(fixedAdWait);
            }

            int maxEscClicks = 90;
            int escCounter = 0;

            while (!CheckSky() && escCounter < maxEscClicks)
            {
                RClick(1157, 466);
                escCounter++;
                Log.I($"ESC {escCounter}");

                bool resumeAd = false;

                if (WaitUntil(() => resumeAd, () =>
                {
                    Getscreen();

                    if (CheckSky())
                    {
                        resumeAd = true;
                        Log.I($"gc detected");
                        return;
                    }

                    if (AreColorsSimilar(Pxl(891, 586), Col(62, 130, 247)))
                    {
                        Log.I($"pause button[1] detected. click and 3s wait");
                        LClick(891, 586);
                        Wait(3000);
                        resumeAd = true;
                    }
                    else if (AreColorsSimilar(Pxl(863, 538), Col(62, 130, 247)))
                    {
                        Log.I($"pause button[2] detected. click and 3s wait");
                        LClick(863, 538);
                        Wait(3000);
                        resumeAd = true;
                    }
                    else if (AreColorsSimilar(Pxl(863, 538), Col(62, 130, 247)))
                    {
                        Log.I($"pause button[3] detected. click and 3s wait");
                        LClick(1079, 591);
                        Wait(3000);
                        resumeAd = true;
                    }
                }, 1000, 30))
                {

                }
            }

            if (!x3Ad)
            {
                if (escCounter >= maxEscClicks)
                {
                    Log.E($"{maxEscClicks} esc clicked. restart will be called");
                    Restart();
                }
                else
                {
                    Wait(500);
                    Log.I($"continue");
                }
            }
            else
            {
                if (escCounter >= maxEscClicks)
                {
                    Log.E($"{maxEscClicks} esc clicked. restart will be called[ad for x3]");
                    Restart();
                    Wait(300);
                }
                else
                {
                    Log.I($"writing time to timerx3spd. continue[ad for x3]");
                    x3Timer = DateTime.Now;
                    File.WriteAllText(Cst.TIMER_X3_FILE_PATH, x3Timer.ToString("O"));
                }
            }

        }

        public void TryUpgradeTower()
        {
            if (upgradeCastle)
            {
                if (firstCrystalUpgrade)
                {
                    firstCrystalUpgrade = false;
                    Log.I($"first tower upgrade");
                    replaysForUpgrade = 0;
                    UpgradeTower();
                }
                else if (replaysForUpgrade > 9)
                {
                    replaysForUpgrade = 0;
                    Log.I($"tower upgrade calling");
                    UpgradeTower();
                }
            }
        }

        public void UpgradeHero()
        {

            Log.I($"[hero upgrade] called");

            if (CountCrystals(true) > 7)
            {

                Log.I($">7 crystals. open hero [hero upgrade]");

                Wait(200);

                switch (upgradeHeroNum)
                {
                    case 1:
                        RandomClickIn(323, 119, 363, 157);
                        break;
                    case 2:
                        RandomClickIn(417, 115, 461, 163);
                        break;
                    case 3:
                        RandomClickIn(508, 114, 550, 162);
                        break;
                    case 4:
                        RandomClickIn(324, 227, 368, 271);
                        break;
                    case 5:
                        RandomClickIn(417, 226, 462, 276);
                        break;
                    case 6:
                        RandomClickIn(509, 226, 555, 278);
                        break;
                    case 7:
                        RandomClickIn(319, 333, 367, 385);
                        break;
                    case 8:
                        RandomClickIn(412, 334, 463, 385);
                        break;
                    case 9:
                        RandomClickIn(507, 333, 553, 387);
                        break;
                    case 10:
                        RandomClickIn(321, 437, 369, 485);
                        break;
                    case 11:
                        RandomClickIn(413, 439, 460, 483);
                        break;
                    case 12:
                        RandomClickIn(507, 432, 557, 488);
                        break;
                    case 13:
                        RandomClickIn(222, 221, 272, 271);
                        break;
                }

                Wait(800);

                Getscreen();

                int cyanPxls = PxlCount(958, 586, 1126, 621, Col(0, 221, 255));
                Log.T($"Cyan pxls: {cyanPxls}");

                if (cyanPxls < 50 || cyanPxls > 150)
                {
                    Log.W($"hero is not crystal upgradable. quit hero upgrading and disable upgrading");
                    Dispatcher.Invoke(() =>
                    {
                        UpgradeHeroForCrystalsCheckbox.Background = new SolidColorBrush(Colors.Red);
                    });
                    upgradeHero = false;
                    RClick(1157, 466);
                    Wait(200);
                    RClick(1157, 466);
                    Wait(200);
                }
                else
                {
                    RandomClickIn(958, 554, 1108, 606);
                    Wait(300);

                    int upgradeCounter = 0;
                    int maxUpgradesInRow = 90;

                    while (CountCrystals(false) > 7 && upgradeCounter < maxUpgradesInRow)
                    {
                        cyanPxls = PxlCount(958, 586, 1126, 621, Col(0, 221, 255));
                        Log.T($"Cyan pxls: {cyanPxls}");

                        if (cyanPxls < 50 || cyanPxls > 150)
                        {
                            Log.W($"not seeing correct upgrade button. quit upgrading.");
                            Dispatcher.Invoke(() =>
                            {
                                UpgradeHeroForCrystalsCheckbox.Background = new SolidColorBrush(Colors.Red);
                            });
                            upgradeHero = false;
                            upgradeCounter = maxUpgradesInRow;
                        }
                        else
                        {
                            RandomClickIn(958, 554, 1108, 606);
                            Wait(300);
                            upgradeCounter++;
                        }

                    }

                    Wait(200);
                    RClick(1157, 466);
                    Wait(200);
                    RClick(1157, 466);
                    Wait(200);

                    Getscreen();

                }
            }
            else
            {
                Log.I($"no upgrading [hero upgrade]");
            }

            Getscreen();

        }

        public void TryUpgradeHero()
        {
            if (!upgradeHero)
            {
                return;
            }
            if (firstCrystalUpgrade)
            {
                firstCrystalUpgrade = false;
                Log.I("first hero upgrade");
                replaysForUpgrade = 0;
                UpgradeHero();
            }
            else if (replaysForUpgrade > 9)
            {
                replaysForUpgrade = 0;
                Log.I("hero upgrade");
                if ((upgradeHeroNum < 1) | (upgradeHeroNum > 13))
                {
                    Log.C("Wrong hero to upgrade slot");
                    WinAPI.ForceBringWindowToFront(this);
                    MessageBox.Show("upgrade hero number is wrong!", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    Halt();
                }
                UpgradeHero();
            }
        }

        public bool CheckItemOnScreen(System.Drawing.Color dustColor)
        {
            // Col(134, 163, 166)    b stone
            // Col(24, 205, 235)    a stone
            // Col(237, 14, 212)    s stone
            // Col(227, 40, 44)     l stone

            if (PixelIn(401, 268, 1192, 703, dustColor))
            {
                if (Pxl(1403, 799) != Col(152, 180, 28) && Pxl(1403, 799) != Col(195, 207, 209))
                {
                    Log.I($"item[{dustColor}] found");
                    GetItem();
                    return true;
                }
            }

            return false;
        }

        public void StopClicker()
        {
            Log.C($"Stop requested");
            if (restartOnCaptcha)
            {
                Restart();
                Wait(300);

                if (upgradeCastle)
                {
                    UpgradeTower();
                }

            }

            Log.C($"stopped");
            Halt();

        }

        public void EscClickStart()
        {

            Log.W($"overlap. esc press");

            if (screenshotOnEsc)
            {
                Screenshot(currentScreen, Cst.SCREENSHOT_ON_ESC_PATH);
            }

            Getscreen();

            int escCounter = 0;
            bool quitCycle = false;

            while (!CheckSky() && escCounter < 10 && !quitCycle)
            {

                if (CaptchaOnScreen())
                {
                    Log.W($"captcha[esc]");
                    quitCycle = true;
                }

                RClick(1157, 466);

                escCounter++;

                Log.W($"esc {escCounter} pressing");
                Wait(500);
            }

            if (escCounter > 9)
            {

                Log.E($"10 escapes pressed. unknown thing");

                if (screenshotAfter10Esc)
                {
                    Screenshot(currentScreen, Cst.SCREENSHOT_AFTER_10_ESC_PATH);
                }

                Restart();
                Wait(300);

            }

        }

        public void AdForCoins()
        {

            RandomClickIn(716, 765, 784, 801);
            Log.I($"[ad for coins] ad for coins clicked. 4s wait");
            Wait(4000);
            Getscreen();

            WaitForAdEnd(false);
        }

        public void CheckAdForX3()
        {
            if (!adForX3 || !(DateTime.Now - x3Timer > TimeSpan.FromSeconds(3610.0)))
            {
                return;
            }
            Log.I("ad for x3 open[adforx3]");
            RandomClickIn(311, 44, 459, 68);
            Wait(500);
            RandomClickIn(1253, 93, 1337, 114);
            Wait(250);
            Getscreen();
            Log.I("wait for loading[adforx3]");
            bool quitCycle = false;
            if (WaitUntil(() => Pxl(78, 418) == Col(98, 87, 73) || quitCycle, () =>
            {
                if (CheckSky() && Pxl(158, 795) == Col(98, 87, 73))
                {
                    quitCycle = true;
                }
            }, 15000, 150))
            {
                Wait(400);
                Getscreen();
                if (Pxl(147, 746) == Col(98, 87, 73))
                {
                    Log.E("connection lost (?)");
                    Dispatcher.Invoke(() =>
                    {
                        AdForSpeedCheckbox.Background = new SolidColorBrush(Colors.Red);
                    });
                    adForX3 = false;
                    LClick(1442, 137);
                    Wait(500);
                    return;
                }
                Log.I("opened");
                if (Pxl(1365, 819) == Col(97, 86, 73))
                {
                    Log.E("x3 is active (?). will be checked after 3610 sec");
                    x3Timer = DateTime.Now;
                    File.WriteAllText(Cst.TIMER_X3_FILE_PATH, x3Timer.ToString("O"));
                    LClick(1442, 137);
                }
                else if (PixelIn(140, 253, 592, 367, Col(82, 255, 82)))
                {
                    Log.I("click on ad and 2 s wait[adforx3]");
                    RandomClickIn(212, 634, 446, 670);
                    Wait(2000);
                    Getscreen();
                    if (Pxl(78, 418) == Col(98, 87, 73))
                    {
                        Log.E("ad didnt open. closing[adforx3]");
                        Dispatcher.Invoke(() =>
                        {
                            AdForSpeedCheckbox.Background = new SolidColorBrush(Colors.Red);
                        });
                        adForX3 = false;
                        LClick(1442, 137);
                        Wait(300);
                    }
                    else
                    {
                        Log.I("ad started. 4.5 s wait[adforx3]");
                        Wait(4500);
                        Getscreen();
                        WaitForAdEnd(x3Ad: true);
                    }
                }
                else
                {
                    Log.E("cant see ad for x3 (???)");
                    Dispatcher.Invoke(() =>
                    {
                        AdForSpeedCheckbox.Background = new SolidColorBrush(Colors.Red);
                    });
                    adForX3 = false;
                    LClick(1442, 137);
                    Wait(300);
                }
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    AdForSpeedCheckbox.Background = new SolidColorBrush(Colors.Red);
                });
                adForX3 = false;
                if (quitCycle)
                {
                    Log.E("no internet (?)");
                }
                else
                {
                    Log.E("too long loading. restart will be called[adforx3]");
                    Restart();
                }
                Wait(300);
            }
        }

        public bool CloseOverlap()
        {
            Getscreen();
            CheckOnHint();
            if (CaptchaOnScreen())
            {
                Log.W("captcha");
                return true;
            }
            if (CheckItemOnScreen(Col(134, 163, 166)))
            {
                return true;
            }
            if (CheckItemOnScreen(Col(24, 205, 235)))
            {
                return true;
            }
            if (CheckItemOnScreen(Col(237, 14, 212)))
            {
                return true;
            }
            if (CheckItemOnScreen(Col(227, 40, 44)))
            {
                return true;
            }
            CheckABExitPanel();
            CheckExitPanel();
            CheckPausePanel();
            CheckSkipPanel();
            CheckHeroPanel();
            CheckRunePanel();
            return false;
        }

        public void WaitForAdAndWatch()
        {
            if ((!adAfterSkipOnly || isSkip) && (waitForAd > 4) && adForCoins && (adDuringX3 || DateTime.Now - x3Timer > TimeSpan.FromSeconds(1205)))
            {
                Log.I($"waiting ad for coins button[0]");
                Wait(400);
                Getscreen();

                if (Pxl(714, 806) == Col(235, 170, 23))
                {
                    Log.I($"button detected. ad for coins calling[0]");
                    AdForCoins();
                    waitForAd = 0;
                }
                else
                {
                    Log.E($"button wasnt detected. continue[0]");
                }
            }
        }

        public bool CoinFlip(double trueChance = 0.5)
        {
            return rand.NextDouble() < trueChance;
        }

        public (int x, int y) GetHeroBlueLineCoords(int slot)
        {
            return slot switch
            {
                0 => (360, 88),
                1 => (456, 92),
                2 => (547, 91),
                3 => (364, 202),
                4 => (455, 202),
                5 => (549, 201),
                6 => (362, 311),
                7 => (455, 310),
                8 => (547, 311),
                9 => (362, 414),
                10 => (456, 414),
                11 => (548, 415),
                12 => (271, 203),
                13 => (183, 452),
                14 => (182, 587),
                _ => (0, 0),
            };
        }

        public (int x1, int y1, int x2, int y2) GetHeroRect(int slot)
        {
            return slot switch
            {
                0 => (322, 110, 363, 165),
                1 => (418, 110, 455, 165),
                2 => (498, 110, 546, 165),
                3 => (322, 203, 363, 276),
                4 => (418, 203, 455, 276),
                5 => (498, 203, 546, 276),
                6 => (322, 311, 363, 387),
                7 => (418, 311, 455, 387),
                8 => (498, 311, 546, 387),
                9 => (322, 414, 363, 492),
                10 => (418, 414, 455, 492),
                11 => (498, 414, 546, 492),
                12 => (218, 197, 267, 266),
                13 => (96, 476, 235, 546),
                14 => (90, 597, 232, 667),
                _ => (0, 0, 0, 0)
            };
        }

        public bool ActivationWaitBreakCondition()
        {
            return !CheckSky() || CheckGCMenu() || dungeonFarm && WaitIfDragonTimer();
        }

        /// <summary>
        /// true if waited, false if breakCondition
        /// </summary>
        /// <param name="breakCondition"></param>
        /// <param name="actionBetweenChecks"></param>
        /// <returns></returns>
        public bool HeroClickWait(Func<bool> breakCondition, Action actionBetweenChecks)
        {
            int waitAmount = rand.Next(heroClickWaitMin, heroClickWaitMax);
            if(waitAmount <= 0)
            {
                return true;
            }
            if (WaitUntil(breakCondition, actionBetweenChecks, waitAmount, 10))
            {
                Log.D("Stop hero waiting on breakConsition");
                return false;
            }
            return true;
        }

        /// <summary>
        /// true if waited, false if breakCondition
        /// </summary>
        /// <param name="breakCondition"></param>
        /// <param name="actionBetweenChecks"></param>
        /// <returns></returns>
        public bool CastWait(Func<bool> breakCondition, Action actionBetweenChecks)
        {
            Log.T("Cast click wait");
            int waitAmount = rand.Next(waitBetweenCastsMin, waitBetweenCastsMax);
            if (waitAmount <= 0)
            {
                return true;
            }
            if (WaitUntil(breakCondition, actionBetweenChecks, waitAmount, 10))
            {
                Log.E("Stop cast waiting on breakConsition");
                return false;
            }
            return true;
        }

        /// <summary>
        /// true if waited, false if breakCondition
        /// </summary>
        /// <param name="breakCondition"></param>
        /// <param name="actionBetweenChecks"></param>
        /// <param name="waitAmount"></param>
        /// <returns></returns>
        public bool WaitConditional(Func<bool> breakCondition, Action actionBetweenChecks, int waitAmount)
        {
            if (WaitUntil(breakCondition, actionBetweenChecks, waitAmount, 10))
            {
                Log.D("Stop on wait conditional");
                return false;
            }
            return true;
        }

        public void DeathAltar(out bool cancel)
        {
            cancel = false;
            if (deathAltar && !deathAltarUsed && Pxl(834, 94) == Col(232, 77, 77))
            {
                RandomClickIn(116, 215, 172, 294);
                if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                {
                    cancel = true;
                }
                deathAltarUsed = true;
            }
        }

        /// <summary>
        /// true if ok. Otherwise false + made restart before return
        /// </summary>
        /// <returns></returns>
        public bool CheckBattleLength()
        {
            TimeSpan currentBattleLength = GetCurrentBattleLength();
            if (currentBattleLength > TimeSpan.FromMilliseconds(maxBattleLength))
            {

                Log.E($"battle length: {currentBattleLength}. restart will be called");

                if (screenshotLongWave)
                {
                    Screenshot(currentScreen, Cst.SCREENSHOT_LONG_WAVE_PATH);
                }

                Restart();
                return false;
            }
            return true;
        }

        public bool LowHp()
        {
            Getscreen();
            return Pxl(864, 54) == Cst.Black;
        }

        public bool SmithAndHealAltar()
        {

            if ((thisSmithSlot != -1 || healAltar) && LowHp())
            {
                if (thisSmithSlot != -1 && Pxl(smithX, smithY) == Cst.BlueLineColor && Pxl(1407, 159) != Cst.CastleUpgradeColor)
                {
                    RandomClickIn(smithX1, smithY1, smithX2, smithY2);
                    Log.I("smith clicked");
                    if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                    {
                        Log.D("Cancel by smith");
                        return false;
                    }
                }
                else if (healAltar && !healAltarUsed)
                {
                    RandomClickIn(116, 215, 172, 294);
                    Log.I("altar clicked");
                    healAltarUsed = true;
                    if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                    {
                        Log.D("Cancel by heal altar");
                        return false;
                    }
                }
            }
            return true;
        }

        public void ActivateHeroes()
        {
            bool quitActivating = false;

            while (CheckSky() && !CheckGCMenu() && !quitActivating)
            {
                AddSpeed();
                CollectMimic();
                CheckSkipPanel();
                ChronoClick(out bool cancel);
                if (cancel)
                {
                    Log.D("Cancel by chrono 1");
                    goto ActivationQuit;
                }

                C();

                int[] castPattern = GenerateActivationSequence(!usedSingleClickHeros);
                usedSingleClickHeros = true;
                double chanceToPressRed = 0.01;

                foreach (int slot in castPattern)
                {
                    (int lx, int ly) = GetHeroBlueLineCoords(slot);
                    (int hx1, int hy1, int hx2, int hy2) = GetHeroRect(slot);
                    if (Pxl(lx, ly) == Cst.BlueLineColor || CoinFlip(chanceToPressRed))
                    {
                        RandomClickIn(hx1, hy1, hx2, hy2);
                        if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                        {
                            Log.D($"Cancel by clickable {slot}");
                            goto ActivationQuit;
                        }
                    }
                    if (!SmithAndHealAltar())
                    {
                        goto ActivationQuit;
                    }
                }


                Getscreen();
                if (thisPureSlot != -1 && pwOnBoss && Pxl(957, 96) == Col(232, 77, 77) && !pwTimer)
                {
                    Log.I("boss hp bar detected");
                    pwBossTimer = DateTime.Now;
                    pwTimer = true;
                }

                if (thisPureSlot != -1 && Pxl(pwX, pwY) == Cst.BlueLineColor)
                {
                    if (pwOnBoss && !dungeonFarmGlobal)
                    {
                        if (Pxl(809, 95) == Cst.White || (((DateTime.Now - pwBossTimer > TimeSpan.FromMilliseconds((double)bossPause * 0.7) && DateTime.Now - x3Timer <= TimeSpan.FromSeconds(1205.0)) || (DateTime.Now - pwBossTimer > TimeSpan.FromMilliseconds(bossPause) && DateTime.Now - x3Timer > TimeSpan.FromSeconds(1205.0))) && pwTimer))
                        {
                            RandomClickIn(pwX1, pwY1, pwX2, pwY2);
                            if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                            {
                                Log.D("Cancel by pw 1");
                                goto ActivationQuit;
                            }
                        }
                    }
                    else
                    {
                        RandomClickIn(pwX1, pwY1, pwX2, pwY2);
                        if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                        {
                            Log.D("Cancel by pw 2");
                            goto ActivationQuit;
                        }
                    }
                }

                ChronoClick(out cancel);
                if (cancel)
                {
                    Log.D("Cancel by chrono 2");
                    goto ActivationQuit;
                }

                if (!CastWait(ActivationWaitBreakCondition, delegate { }))
                {
                    Log.D("Cancel by cast");
                    goto ActivationQuit;
                }

            }
        ActivationQuit:

            ;

        }

        public void ActivateHeroesDun()
        {

            bool quitActivating = false;

            while (CheckSky() && !CheckGCMenu() && !quitActivating)
            {

                AddSpeed();
                ChronoClick(out bool cancel);
                if (cancel)
                {
                    Log.D("Cancel by chrono 1");
                    goto ActivationQuit;
                }

                C();


                if (!SmithAndHealAltar())
                {
                    goto ActivationQuit;
                }

                int[] castPattern = GenerateActivationSequence(!usedSingleClickHeros);
                usedSingleClickHeros = true;
                double chanceToPressRed = 0.01;


                foreach (int slot in castPattern)
                {
                    (int lx, int ly) = GetHeroBlueLineCoords(slot);
                    (int hx1, int hy1, int hx2, int hy2) = GetHeroRect(slot);
                    bool firstUse = singleClickSlots.Contains(slot);
                    if (firstUse || (Pxl(lx, ly) == Cst.BlueLineColor || CoinFlip(chanceToPressRed)) && (Pxl(1407, 159) != Cst.CastleUpgradeColor))
                    {
                        RandomClickIn(hx1, hy1, hx2, hy2);
                        if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                        {
                            goto ActivationQuit;
                        }
                        Getscreen();

                        if (firstUse)
                        {
                            Log.T($"Press slot {slot}");
                            if (Pxl(lx, ly) != Cst.Black)
                            {
                                Log.T($"Didn't press hero {slot}");
                                Wait(200);
                                RandomClickIn(hx1, hy1, hx2, hy2);
                                if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                                {
                                    goto ActivationQuit;
                                }
                            }
                        }
                        if (ActivationWaitBreakCondition())
                        {
                            goto ActivationQuit;
                        }
                    }
                    if (!SmithAndHealAltar())
                    {
                        goto ActivationQuit;
                    }
                }

                if (thisPureSlot != -1 && Pxl(pwX, pwY) == Cst.BlueLineColor)
                {
                    if (Pxl(1407, 159) != Cst.CastleUpgradeColor)
                    {
                        RandomClickIn(pwX1, pwY1, pwX2, pwY2);
                        if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                        {
                            Log.D("Cancel by pw");
                            goto ActivationQuit;
                        }
                        Getscreen();
                    }
                }

                ChronoClick(out cancel);
                if (cancel)
                {
                    Log.D("Cancel by chrono 2");
                    goto ActivationQuit;
                }
                DeathAltar(out cancel);
                if (cancel)
                {
                    Log.D("Cancel by DeathAltar");
                    goto ActivationQuit;
                }

                if (!CastWait(ActivationWaitBreakCondition, delegate { }))
                {
                    goto ActivationQuit;
                }

                if (!CheckBattleLength() || WaitIfDragonTimer())
                {
                    break;
                }

            }
        ActivationQuit:

            if (dungeonNumber > 6 && CheckGCMenu())
            {
                Wait(200);
            }

        }
    }
}
