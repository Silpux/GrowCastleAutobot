using gca_clicker.Classes;
using gca_clicker.Clicker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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

                if(PixelIn(692,435,1079,711, Col(239, 209, 104), out _)){


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
                if(PixelIn(counterx, upgymin, counterx, upgymax, crystalWhiteColor, out _))
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
                if(PixelIn(counterx,upgymin,counterx, upgymax, crystalWhiteColor, out _)){
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
                    RemoveLine(Cst.DUNGEON_STATISTICS_PATH, lineNumber);
                    InsertLine(Cst.DUNGEON_STATISTICS_PATH, lineNumber, $"{itemGrade.ToString()}: {itemsCount + 1}");

                }

            }
            else
            {

                Debug.WriteLine("wrong item");

                if (PixelIn(335, 188, 1140, 700, Col(134, 163, 166), out _)) itemGrade = ItemGrade.B;
                else if (PixelIn(335, 188, 1140, 700, Col(24, 205, 235), out _)) itemGrade = ItemGrade.A;
                else if (PixelIn(335, 188, 1140, 700, Col(237, 14, 212), out _)) itemGrade = ItemGrade.S;
                else if (PixelIn(335, 188, 1140, 700, Col(227, 40, 44), out _)) itemGrade = ItemGrade.L;
                else if (PixelIn(335, 188, 1140, 700, Col(255, 216, 0), out _)) itemGrade = ItemGrade.E;

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




    }
}
