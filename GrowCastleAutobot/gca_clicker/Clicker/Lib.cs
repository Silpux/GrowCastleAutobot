using gca_clicker.Classes;
using gca_clicker.Clicker;
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

        public void SetDefaultNoxState(nint hWnd)
        {
            WinAPI.RestoreWindow(hWnd);
            WinAPI.SetWindowPos(hWnd, hWnd, 0, 0, Cst.WINDOW_WIDTH, Cst.WINDOW_HEIGHT, WinAPI.SWP_NOZORDER);
        }

        public bool CheckNoxState()
        {
            (int x, int y, int width, int height) = GetWindowInfo(hwnd);
            if(x == -32000  && y == -32000)
            {
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


        private bool mimicOpened = false;
        private bool dungeonFarm = false;

        private bool screenshotRunes = false;

        private int dungeonNumber = 0;

        private double mimicCollectPercent = 100;
        private bool wrongItem = false;

        private bool deleteB = false;
        private bool deleteA = false;
        private bool deleteS = false;
        private bool deleteL = false;
        private bool deleteE = false;

        private bool screenshotItems = false;

        private DateTime lastAddSpeed;
        private TimeSpan addSpeedCheckInterval = new TimeSpan(0,0,1);

        public int chronoPos = 0;
        public int chronoX = 0;
        public int chronoY = 0;

        private int heroClickPause = 50;

        private int fixedLoadingWait = 0;

        private bool restarted = false;

        private DateTime lastReplayTime;

        private bool screenshotIfLongGCLoad = true;

        private bool screenshotNoxLoadFail = true;
        private bool screenshotClearAllFail = true;
        private bool screenshotNoxMainMenuLoadFail = true;
        private bool screenshotOnEsc = true;

        private DateTime lastCleanupTime;

        private int maxRestartsForReset = 2;

        private bool upgradeTower = true;
        private int floorToUpgrade = 1;

        private bool waveCanceling = false;

        private int replaysForUpgrade = 0;

        private bool breakABOn30Crystals = false;

        private bool skipNextWave = false;

        private bool skipWaves = false;

        private int manualsBetweenSkips = 2;

        public void CollectMimic()
        {
            if(!mimicOpened && !dungeonFarm)
            {
                if(Pxl(810, 93) == Cst.Black)
                {
                    Debug.WriteLine("Mimic check");
                    currentScreen = Colormode(4, currentScreen);

                    if(PixelIn(437, 794, 1339, 829, Cst.White, out (int x, int y) ret))
                    {
                        double mimic_randomizer = new Random().NextDouble() * 100;

                        if(mimic_randomizer <= mimicCollectPercent)
                        {
                            Debug.WriteLine("Collect mimic");
                            RandomClickIn(ret.x, ret.y, ret.x + 10, ret.y + 10);
                        }
                        else
                        {
                            Debug.WriteLine("Collect mimic");
                        }

                    }

                    mimicOpened = true;
                }
            }
        }


        public void CheckExitPanel()
        {


            if(Pxl(444, 481) == Col(227, 197, 144) &&
            Pxl(464, 494) == Col(167, 118, 59) &&
            Pxl(693, 491) == Col(167, 118, 59) &&
            Pxl(681, 540) == Col(120, 85, 43) &&
            Pxl(828, 489) == Col(242, 190, 35) &&
            Pxl(829, 540) == Col(235, 170, 23))
            {

                Debug.WriteLine("Close quit window");

                Lclick(571, 514);
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

                Debug.WriteLine("pause exit");

                Lclick(571, 514);
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

                Rclick(1157, 466);
                Debug.WriteLine("skip exit");
                Wait(50);
                Getscreen();

            }
        }

        public void CheckLoseABPanel()
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

                Rclick(1157, 466);
                Debug.WriteLine("ab lost window exit");
                Wait(50);
                Getscreen();
            }

        }

        public void CheckHeroPanel()
        {

            if (Pxl(768, 548) == Col(239, 72, 90) &&
            Pxl(875, 547) == Col(239, 72, 90) &&
            Pxl(742, 607) == Col(216, 51, 59) &&
            Pxl(871, 607) == Col(216, 51, 59))
            {

                Debug.WriteLine("hero quit");
                Rclick(518, 404);
                Wait(100);
                Rclick(518, 404);
                Wait(100);
                Getscreen();
            }

        }


        public void CheckRunePanel()
        {


            if (dungeonFarm)
            {

                if(PixelIn(692,435,1079,711, Col(239, 209, 104))){


                    Wait(300);
                    Getscreen();

                    if (PixelIn(692, 435, 1079, 711, Col(239, 209, 104), out (int x, int y) ret))
                    {

                        if (dungeonNumber > 6)
                        {

                            if (screenshotRunes)
                            {
                                Screenshot(currentScreen, "Runes/Rune.png");
                            }

                            Debug.WriteLine("rune collecting");

                            Lclick(ret.x, ret.y);

                            Wait(100);
                        }

                    }

                }

            }


        }



        public void CheckABExitPanel()
        {

            if (Pxl(788, 506) == Col(216, 51, 59))
            {
                Wait(200);
                Debug.WriteLine("ab quit");
                RClick(518, 404);
                Getscreen();
            }

        }



        public int CountCrystals(bool lightMode)
        {
            Color crystalWhiteColor = Cst.White;

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
            int foundmax = 0;

            //int crystalsWidth1 = 5; // 1
            int crystalsWidth2 = 12; // 9
            int crystalsWidth3 = 18; // 11
            //int crystalsWidth4 = 25; // 10
            int crystalsWidth5 = 33; // 20
            int crystals_2_width = 13;

            Getscreen();

            while((counterx > upgxmin) && foundmax == 0)
            {
                if(PixelIn(counterx, upgymin, counterx, upgymax, crystalWhiteColor))
                {
                    foundmax = counterx;
                }
                else
                {
                    counterx--;
                }
            }

            if((foundmax != 0) && (foundmax > 432))
            {
                //crystalsWidth1 = 5; // 1
                crystalsWidth2 = 15; // 9
                crystalsWidth3 = 21; // 11
                //crystalsWidth4 = 32; // 10
                crystalsWidth5 = 38; // 20
                crystals_2_width = 15;
                counterx = foundmax - 50;
                Debug.WriteLine("no oranges");
            }
            else
            {
                Debug.WriteLine("has oranges");
                counterx = foundmax - 45;
            }

            while((counterx < upgxmax) && (foundmin == 0))
            {
                if(PixelIn(counterx,upgymin,counterx, upgymax, crystalWhiteColor)){
                    foundmin = counterx;
                }
                else
                {
                    counterx = counterx + 1;
                }
            }

            int crystalsCountResult = 0;

            if((foundmax != 0) && (foundmin != 0))
            {

                if(foundmax-foundmin > crystalsWidth2)
                {
                    crystalsCountResult = 0;
                }

                if(foundmax-foundmin > crystalsWidth3)
                {
                    crystalsCountResult = 10;
                }

                if(foundmax-foundmin > crystalsWidth5)
                {

                    crystalsCountResult = 20;

                    if (lightMode)
                    {
                        counterx = foundmin;
                        foundmin = 0;
                        foundmax = 0;
                        while((counterx < upgxmax) && (foundmin == 0))
                        {
                            if(Pxl(counterx, 67) == crystalWhiteColor)
                            {
                                foundmin = counterx;
                            }
                            counterx++;

                        }
                        while((counterx < upgxmax) && (foundmax == 0))
                        {
                            if(Pxl(counterx, 67) != crystalWhiteColor)
                            {
                                foundmax = counterx;
                            }
                            else
                            {
                                counterx++;
                            }
                        }
                        if ((foundmax != 0) && (foundmin != 0) && (foundmax - foundmin < crystals_2_width))
                        {
                            crystalsCountResult = 30;
                        }
                    }
                }
            }
            return crystalsCountResult;
        }

        public void ItemDrop(ItemGrade itemGrade, int lineNumber)
        {
            if (!wrongItem)
            {
                if(lineNumber != 0)
                {
                    string line = File.ReadLines(Cst.DUNGEON_STATISTICS_PATH).Skip(lineNumber - 1).FirstOrDefault()!;
                    int itemsCount = -1;
                    try
                    {
                        itemsCount = int.Parse(Regex.Replace(line, "[^0-9]", ""));
                    }
                    catch
                    {
                        itemsCount = -1;
                    }
                    ReplaceLine(Cst.DUNGEON_STATISTICS_PATH, lineNumber, $"{itemGrade.ToString()}: {itemsCount + 1}");
                }
            }
            else
            {
                Debug.WriteLine("wrong item");

                if (PixelIn(335, 188, 1140, 700, Col(134, 163, 166))) itemGrade = ItemGrade.B;
                else if (PixelIn(335, 188, 1140, 700, Col(24, 205, 235))) itemGrade = ItemGrade.A;
                else if (PixelIn(335, 188, 1140, 700, Col(237, 14, 212))) itemGrade = ItemGrade.S;
                else if (PixelIn(335, 188, 1140, 700, Col(227, 40, 44))) itemGrade = ItemGrade.L;
                else if (PixelIn(335, 188, 1140, 700, Col(255, 216, 0))) itemGrade = ItemGrade.E;

                wrongItem = false;
            }

            (bool deleteCurrentItem, Color dustColor, string screenshotPath) = itemGrade switch
            {
                ItemGrade.B => (deleteB, Col(134, 163, 166), Cst.SCREENSHOT_ITEMS_B_PATH),
                ItemGrade.A => (deleteA, Col(24, 205, 235), Cst.SCREENSHOT_ITEMS_A_PATH),
                ItemGrade.S => (deleteS, Col(237, 14, 212), Cst.SCREENSHOT_ITEMS_S_PATH),
                ItemGrade.L => (deleteL, Col(227, 40, 44), Cst.SCREENSHOT_ITEMS_L_PATH),
                ItemGrade.E => (deleteE, Col(227, 40, 44), Cst.SCREENSHOT_ITEMS_E_PATH),
                _ => (false, Col(134, 163, 166), Cst.SCREENSHOT_ITEMS_B_PATH)
            };

            if (deleteCurrentItem)
            {
                if (PixelIn(335, 188, 1140, 700, dustColor, out (int x, int y) ret))
                {
                    RandomDblClickIn(ret.x - 30, ret.y + 10, ret.x + 30, ret.y + 60);
                }
            }
            else
            {
                Getscreen();

                if (screenshotItems)
                {
                    Screenshot(currentScreen, screenshotPath);
                }

                if (PixelIn(335, 188, 1140, 700, Col(239, 209, 104), out (int x, int y) ret))
                {
                    RandomClickIn(ret.x, ret.y, ret.x + 130, ret.y + 60);
                }

            }
        }

        public void AddSpeed()
        {
            if(DateTime.Now - lastAddSpeed > addSpeedCheckInterval)
            {

                if (PxlCount(20, 757, 121, 813, Cst.Black) > 500)
                {
                    if (CheckNoxState())
                    {

                        Debug.WriteLine("add speed");

                        RandomClickIn(79, 778, 99, 798);
                        Wait(100);

                    }

                }

                lastAddSpeed = DateTime.Now;

            }


        }

        public void ChronoClick()
        {
            if(chronoPos != 0)
            {
                if (!CheckGCMenu() && Pxl(chronoX, chronoY) == Cst.BlueLineColor)
                {

                    RandomClickIn(chronoX - 60, chronoY, chronoX, chronoY + 60);
                    Wait(heroClickPause);
                    Getscreen();
                }
            }
        }

        public void EnterGC()
        {
            Lclick(843, 446);

            if(fixedLoadingWait > 0)
            {
                Debug.WriteLine($"[EnterGC] wait fixed {fixedLoadingWait}ms.");
                Wait(fixedLoadingWait);
            }

            Debug.WriteLine($"gc click[EnterGC] wait 20s for gc open");

            if(WaitUntil(CheckGCMenu, delegate { }, 20_000, 200))
            {
                Wait(200);
                Debug.WriteLine("gc opened[EnterGC]");
                restarted = true;
                lastReplayTime = DateTime.Now;
            }
            else
            {
                if (screenshotIfLongGCLoad)
                {
                    Screenshot(currentScreen, Cst.SCREENSHOT_LONG_GC_LOAD_PATH);
                }
                Debug.WriteLine("too long loading. restarting.[EnterGC]");
            }
        }

        public void Reset()
        {
            Debug.WriteLine("Nox Reset");

            Lclick(1499, 333);
            Debug.WriteLine("reset click");

            Wait(500);
            Move(1623, 333);

            Wait(5000);
            Debug.WriteLine("wait up to 2 minutes for nox load[reset]");

            Getscreen();

            if(WaitUntil(() => Pxl(838, 150) == Cst.White, Getscreen, 120_000, 1000))
            {
                Debug.WriteLine("4s wait");
                Wait(4000);

                Debug.WriteLine("nox opened");

                EnterGC();
            }
            else
            {
                Getscreen();
                if (screenshotNoxLoadFail)
                {
                    Screenshot(currentScreen, Cst.SCREENSHOT_NOX_LOAD_FAIL_PATH);
                }
                Debug.WriteLine("nox load stuck. [reset]");
                Debug.WriteLine("window is overlapped by sth or wrong nox path. [reset]");
                Debug.WriteLine("stopped. [reset]");
                Halt();
            }

        }

        public void MakeCleanup()
        {
            bool closedGC = false;

            Debug.WriteLine("open recent");

            Lclick(1488, 833);

            Wait(300);
            Debug.WriteLine("wait for clear all button");
            
            if (WaitUntil(() => PixelIn(985, 91, 1101, 131, Cst.White), Getscreen, 3000, 30))
            {
                Debug.WriteLine("clear all button detected");
                Debug.WriteLine("close recent apps");
                
                Wait(400);

                Lclick(1062, 113);

                Debug.WriteLine("wait for nox main menu");

                if (WaitUntil(CheckNoxMainMenu, delegate { }, 5000, 100))
                {
                    Wait(700);
                    Debug.WriteLine("nox main menu opened");
                    closedGC = true;
                }
                else
                {
                    if (screenshotNoxLoadFail)
                    {
                        Screenshot(currentScreen, Cst.SCREENSHOT_NOX_LOAD_FAIL_PATH);
                    }
                    Debug.WriteLine("nox main menu loading too long. restarting[restart]");
                }

            }
            else
            {
                if (screenshotClearAllFail)
                {
                    Screenshot(currentScreen, Cst.SCREENSHOT_CLEARALL_FAIL_PATH);
                }
                Debug.WriteLine("cant see clear all button.");
            }

            if (closedGC)
            {
                Lclick(1499, 288);
                Wait(200);
                Move(1450, 288);
                Debug.WriteLine("Cleanup click. wait 7s");
                Wait(7000);
                EnterGC();
                lastCleanupTime = DateTime.Now;
            }
            else
            {
                Debug.WriteLine("Cleanup fail");
            }

        }

        public void Restart()
        {
            Debug.WriteLine("Restart");
            int restartCounter = 0;
            restarted = false;

            while(!restarted && restartCounter < maxRestartsForReset + 1){

                restartCounter++;
                Debug.WriteLine($"Restart {restartCounter}");

                if(restartCounter < maxRestartsForReset + 1)
                {
                    Lclick(1488, 833);
                    Wait(300);

                    Debug.WriteLine($"wait for clear all button");

                    if(WaitUntil(() => PixelIn(985, 91, 1101, 131, Cst.White), Getscreen, 3000, 30)){

                        Debug.WriteLine($"close recent apps");

                        Wait(400);
                        Lclick(1062, 113);

                        Debug.WriteLine($"wait for nox main menu");

                        if(WaitUntil(CheckNoxMainMenu, delegate { }, 5000, 100))
                        {
                            Wait(700);
                            Debug.WriteLine($"nox main menu opened");
                            EnterGC();
                        }
                        else
                        {
                            if (screenshotNoxMainMenuLoadFail)
                            {
                                Screenshot(currentScreen, Cst.SCREENSHOT_NOX_MAIN_MENU_LOAD_FAIL_PATH);
                            }
                            Debug.WriteLine($"nox main menu loading too long. restarting[restart]");
                        }
                    }
                    else
                    {
                        if (screenshotClearAllFail)
                        {
                            Screenshot(currentScreen, Cst.SCREENSHOT_CLEARALL_FAIL_PATH);
                        }
                        Debug.WriteLine($"cant see clear all button.");
                    }


                }
                else
                {
                    Debug.WriteLine($"{maxRestartsForReset} restarts in a row made. nox reset will be called");
                    Reset();
                }
            }
            if (!restarted)
            {
                Debug.WriteLine($"Unknown problem. couldnt load gc.");
                Halt();
            }
        }


        public void UpgradeTower()
        {

            Debug.WriteLine($"[tower upgrade] called");

            CountCrystals(true);

            if(CountCrystals(true) > 7)
            {

                Debug.WriteLine($">7 crystals. castle open [tower upgrade]");

                Lclick(440, 557); // Open castle

                Wait(200);

                switch (floorToUpgrade)
                {
                    case 1:
                        Lclick(440, 557);
                        break;
                    case 2:
                        Lclick(440, 455);
                        break;
                    case 3:
                        Lclick(440, 346);
                        break;
                    case 4:
                        Lclick(440, 233);
                        break;
                    default:
                        Debug.WriteLine($"Wrong floor to upgrade");
                        return;
                }

                Wait(400);

                Lclick(1016, 588);
                Wait(100);

                int upgradeCounter = 0;

                while((CountCrystals(false) > 7) && (upgradeCounter < 3)){
                    Lclick(1016, 588);
                    Wait(100);
                    upgradeCounter++;
                }
                Wait(200);
                Getscreen();

                if((Pxl(788,698) == Col(98, 87, 73)) && (Pxl(748,758) == Col(98, 87, 73)))
                {
                    Debug.WriteLine($"reached max tower level");
                    Lclick(788, 712);
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
                Debug.WriteLine($"no upgrading [tower upgrade]");

            }

            Getscreen();


        }

        public void GetItem()
        {

            Debug.WriteLine($"GetItem");

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
            if(WaitUntil(() =>
            {
                if(PixelIn(335, 188, 1140, 700, Col(239, 209, 104)))
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
                Debug.WriteLine($"item dropped");
                Wait(300);
                Getscreen();

                bool correctItem = false;

                switch (dungeonNumber)
                {
                    case 1:
                        if(PixelIn(397,134,1116,440, Col(218, 218, 218), out (int x, int y) ret)){
                            if(PxlCount(ret.x - 5, ret.y - 5, ret.x + 5, ret.y + 5, Cst.White) == 0)
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
                        if(PixelIn(397, 134, 1116, 440, Col(68, 255, 218))) // A
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
                    Debug.WriteLine($"Cant see GET button. rclick");
                    Rclick(518, 404);
                    Wait(100);
                    Getscreen();
                }
            }

        }




        public void CheckOnHint()
        {
            bool hintDetected = false;

            if(!CheckSky() && Pxl(19, 315) == Cst.SkyColor)
            {

                Debug.WriteLine($"hint check 1");
                Wait(200);
                Getscreen();

                if (!CheckSky() && Pxl(19, 315) == Cst.SkyColor)
                {

                    Debug.WriteLine($"hint check 2");
                    Wait(250);
                    Getscreen();

                    if (!CheckSky() && Pxl(19, 315) == Cst.SkyColor)
                    {
                        Debug.WriteLine($"hint check 3");
                        Wait(400);
                        Getscreen();

                        if (!CheckSky() && Pxl(19, 315) == Cst.SkyColor)
                        {
                            Screenshot(currentScreen, Cst.SCREENSHOT_HINT_PATH);
                            Debug.WriteLine($"unknown hint detected");
                            hintDetected = true;
                        }

                    }

                }
            }


            if (hintDetected)
            {

                Screenshot(currentScreen, Cst.SCREENSHOT_HINT_PATH);
                Debug.WriteLine($"___Hint detected___");
                Wait(3000);

                Getscreen();

                Screenshot(currentScreen, Cst.SCREENSHOT_HINT_PATH);

                Debug.WriteLine($"___RESTART___");

                Restart();

                Debug.WriteLine($"___RESTARTED___");
                Debug.WriteLine($"30 s screenshotting");

                for(int i = 0; i < 10; i++)
                {
                    Screenshot(currentScreen, Cst.SCREENSHOT_HINT_PATH);
                    Debug.WriteLine($"__Screen{i}__");
                    Wait(3000);
                    Getscreen();
                }

                Screenshot(currentScreen, Cst.SCREENSHOT_HINT_PATH);

            }

        }

        public void WaitForCancelABButton()
        {
            Debug.WriteLine($"wait for cancel ab button");
            
            if (!waveCanceling)
            {
                replaysForUpgrade = 100; // to ensure that tower or hero will be upgraded for crystals after quitting ab
            }

            Getscreen();

            if(WaitUntil(() => Pxl(788, 506) != Col(216, 51, 59), Getscreen, 10_000, 200))
            {
                Wait(200);
                bool abLostPanel = false;

                if(WaitUntil(() => Pxl(788, 506) == Col(216, 51, 59) || abLostPanel,
                () => {

                    AddSpeed();

                    if (!CheckSky())
                    {
                        CheckPausePanel();
                        CheckExitPanel();

                        if (Pxl(454, 345) == Col(75, 62, 52) &&
                        Pxl(1027, 344) == Col(75, 62, 52) &&
                        Pxl(457, 508) == Col(75, 62, 52) &&
                        Pxl(1025, 512) == Col(75, 62, 52) &&
                        Pxl(666, 572) == Col(239, 209, 104) &&
                        Pxl(812, 574) == Col(242, 190, 35) &&
                        Pxl(663, 618) == Col(235, 170, 23) &&
                        Pxl(818, 284) == Col(98, 87, 73))
                        {
                            RClick(1157, 466);
                            Debug.WriteLine($"ab lost window exit");
                            Wait(50);
                            Getscreen();
                            abLostPanel = true;
                        }
                    }
                    Getscreen();

                }, 120_000, 50))
                {
                    if (!abLostPanel)
                    {
                        Wait(50);
                        Debug.WriteLine($"cancel button detected");
                        RClick(515, 404);
                        Wait(50);
                    }
                }
                else
                {
                    Debug.WriteLine($"? ? ? ?");
                    Restart();
                }
            }
            else
            {
                Debug.WriteLine($"? o_O ?");
                Restart();
            }

        }

        public void ABWait(int secondsToWait)
        {

            TimeSpan timeToWait = TimeSpan.FromSeconds(secondsToWait);

            Debug.WriteLine($"AB {timeToWait}");

            DateTime abStart = DateTime.Now;

            bool quitWaiting = false;

            while(DateTime.Now - abStart < timeToWait)
            {
                if(!WaitUntil(() => !CheckSky() || DateTime.Now - abStart > timeToWait || quitWaiting,
                () =>
                {
                    AddSpeed();
                    if (breakABOn30Crystals)
                    {
                        if(CountCrystals(true) >= 30)
                        {
                            Debug.WriteLine($"break ab");
                            timeToWait = TimeSpan.Zero;
                            skipNextWave = true;
                        }
                    }
                }, 120_000, 500))
                {
                    Debug.WriteLine($"2min wave. restart gc");

                    if (screenshotOnEsc)
                    {
                        Screenshot(currentScreen, Cst.SCREENSHOT_AB_ERROR2_PATH);
                    }

                    quitWaiting = true;
                    timeToWait = TimeSpan.Zero;
                    Restart();
                }

                Debug.WriteLine($"switch1");

                if (!WaitUntil(() => CheckSky() || DateTime.Now - abStart > timeToWait || quitWaiting,
                () =>
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
                        Debug.WriteLine($"ab lost window exit");
                        Wait(50);
                        Getscreen();
                        quitWaiting = true;
                        timeToWait = TimeSpan.Zero;
                    }

                    CheckPausePanel();
                    CheckExitPanel();

                }, 10_000, 50))
                {

                    Debug.WriteLine($"10s closed sky");
                    Debug.WriteLine($"check if lost during ab");

                    if (screenshotOnEsc)
                    {
                        Screenshot(currentScreen, Cst.SCREENSHOT_AB_ERROR_PATH);
                    }

                    CheckLoseABPanel();

                    Wait(300);

                    Getscreen();

                    if (!CheckSky())
                    {
                        Debug.WriteLine($"unknown ab error");
                        Restart();
                    }
                    else
                    {
                        Debug.WriteLine($"lost during ab");
                    }
                    quitWaiting = true;
                    timeToWait = TimeSpan.Zero;
                }
                Debug.WriteLine($"switch2");

            }

            if (!quitWaiting)
            {
                WaitForCancelABButton();

                if (skipWaves)
                {
                    Debug.WriteLine($" {manualsBetweenSkips} battles with skips");
                }

            }

        }















    }
}
