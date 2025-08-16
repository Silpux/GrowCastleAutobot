using gca_clicker.Classes;
using gca_clicker.Clicker;
using gca_clicker.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
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
using static gca_clicker.Classes.WinAPI;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        public bool CheckSky(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(282, 35) == Cst.SkyColor;
        }

        public bool CheckGCMenu(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(1407, 159) == Cst.CastleUpgradeColor;
        }

        public bool CheckEmptyGame(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return CheckGCMenu() && P(92, 131) == Cst.SkyColor;
        }

        public bool IsInNoxMainMenu(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            currentScreen.Colormode(5, 800, 148, 1000, 151);
            return P(635, 96) != Cst.White &&
                P(843, 93) != Cst.White &&
                P(1057, 94) != Cst.White &&
                P(599, 197) != Cst.White &&
                P(847, 199) != Cst.White &&
                P(1070, 197) != Cst.White &&
                (P(806, 150) == Cst.White || P(806, 150) == Col(191, 191, 191)) &&
                (P(991, 149) == Cst.White || P(991, 149) == Col(191, 191, 191));
        }

        public bool CaptchaOnScreen(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(1153, 163) == Col(98, 87, 73) &&
            P(1156, 179) == Col(75, 62, 52) &&
            P(1155, 201) == Col(98, 87, 73) &&
            P(703, 167) == Col(223, 223, 223) &&
            P(847, 180) == Col(223, 223, 223) &&
            P(975, 178) == Col(98, 87, 73) &&
            P(807, 180) == Col(98, 87, 73);
        }

        public bool QuitBattle()
        {
            Log.I($"{nameof(QuitBattle)}");
            if (!CheckGCMenu() && !IsInTown(false))
            {
                WaitUntilDeferred(() => HasPausePanel(), () => StepBack(), 3100, 500);
                if (HasPausePanel())
                {
                    RCI(796, 480, 1039, 543);
                    Wait(500);
                    return true;
                }
                else
                {
                    Log.E($"{nameof(QuitBattle)} couldn't pause game");
                    return false;
                }
            }
            else
            {
                Log.E($"{nameof(QuitBattle)} called in wrong place");
                return false;
            }
        }

        public bool IsPopupOnScreen(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(784, 701) == Col(98, 87, 73) &&
            P(783, 715) == Col(35, 33, 30) &&
            P(780, 734) == Col(98, 87, 73) &&
            P(802, 717) == Col(98, 87, 73);
        }
        public bool IsPopupOnCurrentScreen()
        {
            return P(784, 701) == Col(98, 87, 73) &&
            P(783, 715) == Col(35, 33, 30) &&
            P(780, 734) == Col(98, 87, 73) &&
            P(802, 717) == Col(98, 87, 73);
        }

        public void ClosePopup()
        {
            if (IsPopupOnScreen())
            {
                Log.I("Close popup");
                RCI(774, 703, 793, 725);
                Wait(300);
            }
        }

        public bool IsAdForCoinsOnScreen(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(679, 781) == Col(242, 190, 35) &&
            P(688, 765) == Col(47, 37, 31) &&
            P(687, 792) == Col(47, 37, 31) &&
            P(710, 760) == Col(242, 190, 35);
        }

        public bool IsInShop(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(84, 185) == Col(98, 87, 73) &&
            P(82, 232) == Col(98, 87, 73) &&
            P(76, 255) == Col(75, 62, 52) &&
            P(1386, 222) == Col(98, 87, 73) &&
            P(1420, 138) == Col(98, 87, 73) &&
            P(1420, 154) == Col(35, 33, 30) &&
            P(1435, 154) == Col(98, 87, 73);
        }

        /// <summary>
        /// check if is in shop and quit
        /// </summary>
        /// <param name="updateScreen"></param>
        public void CheckShop(bool updateScreen = true)
        {
            if (IsInShop(updateScreen))
            {
                WaitUntilDeferred(() => CheckGCMenu(), () => StepBack(), 3100, 500);
            }
        }

        public bool IsInDungeonList(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(221, 93) == Col(218, 218, 218) &&
            P(263, 90) == Col(98, 87, 73) &&
            P(1404, 777) == Col(69, 58, 48) &&
            P(1442, 121) == Col(35, 33, 30) &&
            P(1444, 104) == Col(98, 87, 73);
        }
        public bool IsDungeonOpen(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(969, 770) == Col(51, 44, 37) &&
            P(1037, 724) == Col(239, 209, 104) &&
            P(1140, 728) == Col(242, 190, 35) &&
            P(1087, 782) == Col(235, 170, 23) &&
            P(1165, 137) == Col(35, 33, 30);
        }

        public bool IsInStartABPanel(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(483, 304) == Col(98, 87, 73) &&
            P(537, 286) == Cst.White &&
            P(614, 264) == Cst.White &&
            P(757, 286) == Cst.White &&
            P(785, 286) == Cst.White &&
            P(838, 295) == Col(98, 87, 73) &&
            P(938, 286) == Cst.White;
        }
        public void CloseAdForCoins()
        {
            if (IsAdForCoinsOnScreen())
            {
                Log.I($"Closing ad for coins");
                RCI(803, 694, 827, 720);
                Wait(500);
            }
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

            if ((x == -32000 && y == -32000) || width != Cst.WINDOW_WIDTH || height != Cst.WINDOW_HEIGHT)
            {
                if (x == -32000 && y == -32000)
                {
                    Log.X($"Was minimized");
                }
                else
                {
                    Log.X($"Had wrong size: W: {width} ({width - Cst.WINDOW_WIDTH:+0;-0;0}), H: {height} ({height - Cst.WINDOW_HEIGHT:+0;-0;0})");
                }
                Log.X($"Fix nox state");
                SetDefaultNoxState(hwnd);
                Wait(100);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check -> Wait -> action
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="actionBetweenChecks"></param>
        /// <param name="timeoutMs"></param>
        /// <param name="checkInterval"></param>
        /// <returns></returns>
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

        /// <summary>
        /// action -> wait -> check
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="actionBetweenChecks"></param>
        /// <param name="timeoutMs"></param>
        /// <param name="checkInterval"></param>
        /// <returns></returns>
        public bool WaitUntilDeferred(Func<bool> condition, Action actionBetweenChecks, int timeoutMs, int checkInterval)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                actionBetweenChecks();
                Wait(checkInterval);
                if (condition()) return true;
            }
            return false;
        }


        public void CollectMimic()
        {
            if (mimicOpened || dungeonFarm || !(P(810, 93) == Cst.Black))
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
                    RCI(ret.Item1, ret.Item2, ret.Item1 + 10, ret.Item2 + 10);
                }
                else
                {
                    Log.M("Ignore mimic");
                }
            }
            mimicOpened = true;
        }

        public bool HasExitPanel(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(444, 481) == Col(227, 197, 144) &&
            P(464, 494) == Col(167, 118, 59) &&
            P(693, 491) == Col(167, 118, 59) &&
            P(681, 540) == Col(120, 85, 43) &&
            P(828, 489) == Col(242, 190, 35) &&
            P(829, 540) == Col(235, 170, 23);
        }
        public void CheckExitPanel(bool updateScreen = true)
        {

            if (HasExitPanel(updateScreen))
            {

                Log.W("Close quit window");

                RCI(477, 487, 689, 535);
                Wait(200);
                G();
            }

        }

        public bool HasPausePanel(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }

            return P(470, 378) == Col(97, 86, 73) &&
            P(504, 483) == Col(167, 118, 59) &&
            P(690, 480) == Col(167, 118, 59) &&
            P(516, 540) == Col(120, 85, 43) &&
            P(693, 538) == Col(120, 85, 43) &&
            P(784, 481) == Col(239, 209, 104) &&
            P(1024, 536) == Col(235, 170, 23) &&
            P(869, 486) == Col(242, 190, 35);

        }
        public void CheckPausePanel(bool updateScreen = true)
        {

            if (HasPausePanel(updateScreen))
            {

                Log.W("pause exit");

                RCI(477, 487, 689, 535);
                Wait(100);
                G();
            }

        }

        public bool IsSkipPanelOnScreen(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(502, 413) == Col(239, 209, 104) &&
            P(579, 427) == Col(242, 190, 35) &&
            P(896, 411) == Col(239, 209, 104) &&
            P(982, 422) == Col(242, 190, 35) &&
            P(783, 461) == Col(235, 170, 23);
        }

        public void CheckSkipPanel(bool updateScreen = true)
        {
            if (IsSkipPanelOnScreen(updateScreen))
            {

                StepBack();
                Log.I("skip exit");
                Wait(200);
                G();

            }
        }


        public bool IsLoseABPanelOnScreen(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(526, 277) == Col(98, 87, 73) &&
            P(555, 281) == Cst.White &&
            P(717, 281) == Cst.White &&
            P(516, 372) == Col(75, 62, 52) &&
            P(965, 363) == Col(75, 62, 52) &&
            P(611, 604) == Col(98, 87, 73) &&
            P(878, 594) == Col(98, 87, 73) &&
            P(668, 573) == Col(239, 209, 104) &&
            P(802, 580) == Col(242, 190, 35) &&
            P(808, 622) == Col(235, 170, 23);
        }

        /// <summary>
        /// true if lost on AB. Will close before return
        /// </summary>
        /// <returns></returns>
        public bool CheckLoseABPanel(bool updateScreen = true)
        {

            if (IsLoseABPanelOnScreen(updateScreen))
            {

                StepBack();
                Log.X("ab lost window exit");
                Wait(100);
                G();
                return true;
            }
            return false;
        }

        public bool IsHeroPanelOnScreen(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(768, 548) == Col(239, 72, 90) &&
            P(875, 547) == Col(239, 72, 90) &&
            P(742, 607) == Col(216, 51, 59) &&
            P(871, 607) == Col(216, 51, 59);
        }
        public void CheckHeroPanel(bool updateScreen = true)
        {

            if (IsHeroPanelOnScreen(updateScreen))
            {
                Log.X("hero quit");
                WaitUntilDeferred(() => CheckGCMenu(), () => StepBack(), 600, 100);
                G();
            }

        }

        public bool IsChooseClassPanelOnScreen(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(993, 90) == Col(98, 87, 73) &&
            P(1030, 88) == Col(167, 167, 167) &&
            P(1055, 90) == Col(98, 87, 73) &&
            P(1170, 90) == Col(98, 87, 73) &&
            P(1266, 88) == Col(167, 167, 167) &&
            P(1438, 122) == Col(35, 33, 30) &&
            P(1455, 123) == Col(98, 87, 73);
        }

        public void CheckChooseClassPanel(bool updateScreen = true)
        {

            if (IsChooseClassPanelOnScreen(updateScreen))
            {
                Log.X("choose class quit");
                WaitUntilDeferred(() => CheckGCMenu(), () => StepBack(), 600, 100);
                G();
            }

        }

        public bool IsRuneOnScreen(bool updateScreen = true)
        {
            return IsItemOnScreen(updateScreen) && PxlCountEnough(429, 340, 1080, 740, Col(14, 200, 248), 100);
        }

        public bool CheckRunePanel(bool updateScreen = true)
        {
            if (!IsRuneOnScreen(updateScreen))
            {
                return false;
            }

            Wait(100);

            // check again, because there could be window animation
            if (!IsRuneOnScreen())
            {
                return false;
            }

            Log.I("rune found");
            Wait(rand.Next(matGetTimeMin, matGetTimeMax));
            G();
            if (screenshotRunes)
            {
                Screenshot(currentScreen, Cst.SCREENSHOT_RUNES_PATH);
            }

            if (PixelIn(335, 188, 1140, 700, Col(239, 209, 104), out (int x, int y) ret))
            {
                Wait(rand.Next(matGetTimeMin, matGetTimeMax));
                Log.I("Click GET");
                RCI(ret.x, ret.y, ret.x + 130, ret.y + 60);

                WaitUntil(() => CheckSky(), delegate { }, 500, 5);
            }
            else
            {
                Log.E("Couldn't find Get button when collecting rune");
                return false;
            }

            return true;


        }

        public void CheckABExitPanel()
        {

            if (P(788, 506) == Col(216, 51, 59))
            {
                Log.X("ab quit");
                StepBack();
                Wait(200);
                G();
            }

        }

        public void CheckIfInTown(bool updateScreen = true)
        {
            if (IsInTown(updateScreen))
            {
                Log.X("Currently in town. Switch back");
                SwitchTown();
            }
        }

        public ulong RangeMask(int startLeftIndex, int len)
        {
            if (len <= 0) return 0UL;
            if (len >= 64) return ulong.MaxValue;

            int end = startLeftIndex + len - 1;
            int lsbPos = 63 - end;
            ulong ones = (1UL << len) - 1UL;
            return ones << lsbPos;
        }

        public unsafe int CountCrystals(bool lightMode, bool showInLabel = false)
        {
            Log.T($"Counting crystals LM: {lightMode}");
            System.Drawing.Color crystalWhiteColor = Cst.White;
            if (!lightMode)
            {
                crystalWhiteColor = Col(89, 89, 89);
            }
            const int CRY_RECT_X1 = 288;
            const int CRY_RECT_Y1 = 40;
            const int CRY_RECT_X2 = 464;
            const int CRY_RECT_Y2 = 70;

            int counterx = CRY_RECT_X2;
            int numberLeftMostPixelX = 0;
            int numberRightmostPixelX = -999;

            int number_1_width = 12;
            int crystalsWidth2 = 12;
            int crystalsWidth3 = 18;
            int crystalsWidth4 = 33;
            int crystals_2_width = 13;
            G();

            Rectangle rect = new Rectangle(0, 0, currentScreen.Width, currentScreen.Height);
            BitmapData data = currentScreen.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                int stride = data.Stride;
                IntPtr ptr = data.Scan0;

                int targetColor = crystalWhiteColor.ToArgb();

                byte* scan0 = (byte*)ptr;
                for (int x = CRY_RECT_X2; x >= CRY_RECT_X1; x--)
                {
                    for (int y = CRY_RECT_Y1; y <= CRY_RECT_Y2; y++)
                    {
                        int pixel = *(int*)(scan0 + y * stride + x * 4);
                        if (pixel == targetColor)
                        {
                            numberRightmostPixelX = x;
                            goto FoundNumber;
                        }
                    }
                }

                Log.T("count crystals: wrong color");
                if (showInLabel)
                {
                    Dispatcher.Invoke(() =>
                    {
                        CrystalsCountLabel.Content = "";
                    });
                }
                return 0;

            FoundNumber:

                if (numberRightmostPixelX > 432)
                {
                    number_1_width = 13;
                    crystalsWidth2 = 15;
                    crystalsWidth3 = 21;
                    crystalsWidth4 = 38;
                    crystals_2_width = 15;
                    counterx = numberRightmostPixelX - 50;
                    if (showInLabel)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            CrystalsCountLabel.Content = "No oranges.";
                        });
                    }
                    Log.T("no oranges");
                }
                else
                {
                    Log.T("has oranges");
                    if (showInLabel)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            CrystalsCountLabel.Content = $"Has oranges";
                        });
                    }
                    counterx = numberRightmostPixelX - 45;
                }

                bool leftFound = false;
                ulong bits = 0;

                int left = 1;

                scan0 = (byte*)ptr;
                for (int x = counterx; x <= numberRightmostPixelX; x++)
                {
                    int y = 40;
                    while (true)
                    {
                        if (y <= 70)
                        {
                            if (*(int*)(scan0 + y * stride + x * 4) == targetColor)
                            {
                                if (!leftFound)
                                {
                                    numberLeftMostPixelX = x;
                                    leftFound = true;
                                }
                                bits |= (ulong)(1L << 63 - left++);
                                break;
                            }
                            y++;
                            continue;
                        }
                        left++;
                        break;
                    }
                }

                int bestStart = -1;
                int bestLen = 0;

                int idx = 0;
                while (idx < 64)
                {
                    int shift = 63 - idx;
                    bool isZero = (bits & (1UL << shift)) == 0;

                    if (!isZero)
                    {
                        idx++;
                        continue;
                    }

                    int start = idx;
                    int count = 0;
                    while (idx < 64 && (bits & (1UL << (63 - idx))) == 0)
                    {
                        count++;
                        idx++;
                    }

                    int end = start + count - 1;
                    bool touchesLeft = start == 0;
                    bool touchesRight = end == 63;

                    if (!touchesLeft && !touchesRight && count > bestLen)
                    {
                        bestLen = count;
                        bestStart = start;
                    }
                }

                if (bestStart != -1)
                {
                    idx = 0;
                    while (idx < 64)
                    {
                        int shift = 63 - idx;
                        if ((bits & (1UL << shift)) != 0)
                        {
                            idx++;
                            continue;
                        }

                        int start = idx;
                        int count = 0;
                        while (idx < 64 && (bits & (1UL << (63 - idx))) == 0)
                        {
                            count++; idx++;
                        }

                        int end = start + count - 1;
                        bool touchesLeft = start == 0;
                        bool touchesRight = end == 63;

                        if (!touchesLeft && !touchesRight && !(start == bestStart && count == bestLen))
                        {
                            ulong mask = RangeMask(start, count);
                            bits |= mask;
                        }
                    }
                }

                int firstNumberWidth = 0;
                bool started = false;

                int ind = 0;
                while (ind < 64 && (bits & (1UL << (63 - ind))) == 0) ind++;
                while (ind < 64 && (bits & (1UL << (63 - ind++))) != 0) firstNumberWidth++;

                int crystalsCountResult = 0;
                if (numberRightmostPixelX != 0 && numberLeftMostPixelX != 0)
                {
                    if (numberRightmostPixelX - numberLeftMostPixelX > crystalsWidth2)
                    {
                        crystalsCountResult = 0;
                    }
                    if (numberRightmostPixelX - numberLeftMostPixelX > crystalsWidth4)
                    {
                        crystalsCountResult = 20;

                        counterx = numberLeftMostPixelX;
                        numberLeftMostPixelX = 0;
                        numberRightmostPixelX = 0;
                        while(counterx <= CRY_RECT_X2)
                        {
                            int pixel = *(int*)((byte*)ptr + 67 * stride + counterx * 4); // Pxl(counterx, 67)
                            if (pixel == targetColor)
                            {
                                numberLeftMostPixelX = counterx++;
                                break;
                            }
                            counterx++;
                        }
                        while (counterx <= CRY_RECT_X2)
                        {
                            int pixel = *(int*)((byte*)ptr + 67 * stride + counterx * 4); // Pxl(counterx, 67)
                            if (pixel != targetColor)
                            {
                                numberRightmostPixelX = counterx;
                                break;
                            }
                            counterx++;
                        }
                        if (numberRightmostPixelX != 0 && numberLeftMostPixelX != 0 && numberRightmostPixelX - numberLeftMostPixelX < crystals_2_width)
                        {
                            crystalsCountResult = 30;
                        }
                    }
                    else if (numberRightmostPixelX - numberLeftMostPixelX > crystalsWidth3)
                    {
                        crystalsCountResult = 10;

                        if (firstNumberWidth > number_1_width)
                        {
                            crystalsCountResult = 20;

                            counterx = numberLeftMostPixelX;
                            numberLeftMostPixelX = 0;
                            numberRightmostPixelX = 0;
                            while(counterx <= CRY_RECT_X2)
                            {
                                int pixel = *(int*)((byte*)ptr + 67 * stride + counterx * 4); // Pxl(counterx, 67)
                                if (pixel == targetColor)
                                {
                                    numberLeftMostPixelX = counterx;
                                    break;
                                }
                                counterx++;
                            }
                            while (counterx <= CRY_RECT_X2)
                            {
                                int pixel = *(int*)((byte*)ptr + 67 * stride + counterx * 4); // Pxl(counterx, 67)
                                if (pixel != targetColor)
                                {
                                    numberRightmostPixelX = counterx;
                                    break;
                                }
                                counterx++;
                            }
                            if (numberRightmostPixelX != 0 && numberLeftMostPixelX != 0 && numberRightmostPixelX - numberLeftMostPixelX < crystals_2_width)
                            {
                                crystalsCountResult = 30;
                            }
                        }

                    }
                }
                Log.T($"Cyrstals: {crystalsCountResult}");
                return crystalsCountResult;
            }
            finally
            {
                currentScreen.UnlockBits(data);
            }
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
                    Wait(rand.Next(matGetTimeMin, matGetTimeMax));
                    RandomDblClickIn(ret.x - 30, ret.y + 10, ret.x + 30, ret.y + 60);
                    WaitUntil(() => CheckSky(), delegate { }, 500, 5);
                    G();
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

                Wait(rand.Next(matGetTimeMin, matGetTimeMax));
                G();

                if (screenshotItems)
                {
                    Screenshot(currentScreen, screenshotPath);
                }

                if (PixelIn(335, 188, 1140, 700, Col(239, 209, 104), out (int x, int y) ret))
                {
                    Log.I("Click GET");
                    RCI(ret.x, ret.y, ret.x + 130, ret.y + 60);
                    WaitUntil(() => CheckSky(), delegate { }, 500, 5);
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
                if (PxlCountEnough(20, 757, 121, 813, Cst.Black, 500))
                {
                    if (CheckNoxState())
                    {
                        Log.X("add speed");

                        RCI(79, 778, 99, 798);
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
                if (!CheckGCMenu() && P(chronoX, chronoY) == Cst.BlueLineColor)
                {
                    Log.T($"Chrono click");
                    RCI(chronoX1, chronoY1, chronoX2, chronoY2);
                    if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                    {
                        cancel = true;
                    }
                }
            }
        }

        public void EnterGC(bool restartIfNotLoading = false)
        {
            LC(843, 446);

            Log.I($"gc click[EnterGC] wait up to {gcLoadingLimit.ToString("#,#", new NumberFormatInfo() { NumberGroupSeparator = " " })} ms. for gc open");

            if (WaitUntil(() => CheckGCMenu(), delegate { }, gcLoadingLimit, 200))
            {
                Wait(200);
                UpdateRestartTime();
                Log.I("gc opened[EnterGC]");
                freezeDetectionEnabled = true;
                restarted = true;
                lastReplayTime = DateTime.Now;
            }
            else
            {
                ScreenshotError(screenshotIfLongGCLoad, Cst.SCREENSHOT_LONG_GC_LOAD_PATH);
                Log.E("too long loading");
                if (restartIfNotLoading)
                {
                    Log.E("Will call restart");
                    Restart();
                }
                Log.ST();
            }
        }

        public void Reset()
        {
            freezeDetectionEnabled = false;

            int maxTries = 10;

            for(int i = 0; i < maxTries; i++)
            {
                Log.R($"Nox Reset. try {i+1} / {maxTries}");
                LC(1499, 333);
                Log.R("reset click");
                Wait(500);
                Move(1623, 333);
                Wait(5000);
                Log.R("wait up to 5 minutes for nox load[reset]");
                G();
                if (WaitUntil(() => P(838, 150) == Cst.White && P(742, 218) != Cst.White, () => G(), 300_000, 1000))
                {
                    Log.R("7s wait");
                    Wait(7000);
                    Log.R("nox opened");
                    EnterGC(true);
                    return;
                }

                Log.E("Nox didn't load.");
            }
            Log.F($"Couldn't reset nox in {maxTries} tries. Will stop");
            G();

            ScreenshotError(screenshotNoxLoadFail, Cst.SCREENSHOT_NOX_LOAD_FAIL_PATH);

            Log.ST();

            restartRequested = false;
            Halt();
        }

        public void MakeCleanup()
        {
            Log.I("Do cleanup");
            freezeDetectionEnabled = false;

            bool closedGC = false;

            Log.I("open recent");

            LC(1488, 833);

            Wait(300);
            G();
            Log.I("wait for clear all button");

            if (WaitUntil(() => PixelIn(985, 91, 1101, 131, Cst.White), () => G(), 3000, 30))
            {
                Log.I("clear all button detected");
                Log.I("close recent apps");

                Wait(400);

                LC(1062, 113);

                Log.I("wait for nox main menu");

                if (WaitUntil(() => IsInNoxMainMenu(), delegate { }, 5000, 100))
                {
                    Wait(700);
                    Log.I("nox main menu opened");
                    closedGC = true;
                }
                else
                {
                    G();
                    ScreenshotError(screenshotNoxMainMenuLoadFail, Cst.SCREENSHOT_NOX_MAIN_MENU_LOAD_FAIL_PATH, true);
                    Log.E("nox main menu loading too long. restarting[restart]");
                    Log.ST();
                }

            }
            else
            {
                ScreenshotError(screenshotClearAllFail, Cst.SCREENSHOT_CLEARALL_FAIL_PATH, true);
                Log.E("cant see clear all button.");
                Log.ST();
            }

            if (closedGC)
            {

                if (doResetOnCleanup)
                {
                    Log.I($"Do reset instead of cleanup");
                    Wait(10_000);
                    Reset();
                }
                else
                {
                    LC(1499, 288);
                    Wait(200);
                    Move(1450, 288);
                    Log.I("Cleanup click. wait 7s");
                    Wait(7000);
                    EnterGC(true);

                }

                UpdateCleanupTime();
            }
            else
            {
                Log.E("Cleanup fail");
            }

        }

        public void UpdateRestartTime()
        {
            if (!doRestarts)
            {
                return;
            }
            nextRestartDt = DateTime.Now + GetRandomTimeSpan(restartIntervalMin * 1000, restartIntervalMax * 1000);
            Dispatcher.Invoke(() =>
            {
                NextRestartTimeLabel.Content = $"Next restart: {nextRestartDt:dd.MM.yyyy HH:mm:ss}";
            });
        }
        public void UpdateCleanupTime()
        {
            nextCleanupTime = DateTime.Now + GetRandomTimeSpan(cleanupIntervalMin * 1000, cleanupIntervalMax * 1000);
            Dispatcher.Invoke(() =>
            {
                NextCleanupTimeLabel.Content = $"Next cleanup: {nextCleanupTime:dd.MM.yyyy HH:mm:ss}";
            });
        }

        public void Restart()
        {
            Log.R("Restart");
            int restartCounter = 0;
            restarted = false;
            freezeDetectionEnabled = false;

            while (!restarted && restartCounter < maxRestartsForReset + 1)
            {

                restartCounter++;
                Log.R($"Restart {restartCounter}");

                if (restartCounter < maxRestartsForReset + 1)
                {
                    LC(1488, 833);
                    Wait(300);

                    Log.R($"wait for clear all button");

                    if (WaitUntil(() => PixelIn(985, 91, 1101, 131, Cst.White), () => G(), 3000, 30))
                    {

                        Log.R($"close recent apps");

                        Wait(400);
                        LC(1062, 113);

                        Log.R($"wait for nox main menu");

                        if (WaitUntil(() => IsInNoxMainMenu(), delegate { }, 5000, 100))
                        {
                            Wait(700);
                            Log.R($"nox main menu opened");
                            EnterGC();
                        }
                        else
                        {
                            G();
                            ScreenshotError(screenshotNoxMainMenuLoadFail, Cst.SCREENSHOT_NOX_MAIN_MENU_LOAD_FAIL_PATH, true);
                            Log.E($"nox main menu loading too long. restarting[restart]");
                            Log.ST();
                        }
                    }
                    else
                    {
                        ScreenshotError(screenshotClearAllFail, Cst.SCREENSHOT_CLEARALL_FAIL_PATH, true);
                        Log.E($"cant see clear all button.");
                        Log.ST();
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
                Log.F($"Unknown problem. couldnt load gc.");
                Halt();
            }
        }

        public void UpgradeTower()
        {
            if (!CheckGCMenu())
            {
                Log.Q($"[{nameof(UpgradeTower)}] called not in gc menu");
                return;
            }

            Log.I($"[{nameof(UpgradeTower)}] called");

            CountCrystals(true);

            if (CountCrystals(true) > 7)
            {

                Log.I($">7 crystals. castle open [{nameof(UpgradeTower)}]");

                RCI(418, 542, 459, 580); // Open castle

                Wait(200);

                switch (floorToUpgrade)
                {
                    case 1:
                        RCI(418, 542, 459, 580);
                        break;
                    case 2:
                        RCI(418, 437, 467, 479);
                        break;
                    case 3:
                        RCI(417, 322, 467, 369);
                        break;
                    case 4:
                        RCI(412, 206, 466, 257);
                        break;
                    default:
                        Log.E($"Wrong floor to upgrade");
                        return;
                }

                Wait(800);

                G();

                int cyanPxls = PxlCount(958, 586, 1126, 621, Col(0, 221, 255));

                Log.T($"Cyan pxls: {cyanPxls}");

                if (cyanPxls < 50 || cyanPxls > 150)
                {
                    Log.O($"Tower is not crystal upgradable. quit tower upgrading");
                    upgradeCastle = false;
                    Dispatcher.Invoke(() =>
                    {
                        UpgradeCastleCheckbox.Background = new SolidColorBrush(Colors.Red);
                    });
                    StepBack();
                    Wait(200);
                    StepBack();
                    Wait(200);
                    return;
                }

                RCI(956, 558, 1112, 603);
                Wait(250);

                int upgradeCounter = 0;
                int maxUpgradesInRow = 90;

                while ((CountCrystals(false) > 7) && (upgradeCounter < maxUpgradesInRow))
                {

                    cyanPxls = PxlCount(958, 586, 1126, 621, Col(0, 221, 255));
                    Log.T($"Cyan pxls: {cyanPxls}");

                    if (cyanPxls < 50 || cyanPxls > 150)
                    {
                        Log.O($"not seeing correct upgrade button. quit upgrading");
                        upgradeCastle = false;
                        Dispatcher.Invoke(() =>
                        {
                            UpgradeCastleCheckbox.Background = new SolidColorBrush(Colors.Red);
                        });
                        upgradeCounter = maxUpgradesInRow;
                    }
                    else
                    {
                        RCI(958, 554, 1108, 606);
                        Wait(250);
                        upgradeCounter++;
                    }

                }
                Wait(200);

                StepBack();
                Wait(200);
                StepBack();
                Wait(200);
                StepBack();
                Wait(200);
            }
            else
            {
                Log.I($"no upgrading [{nameof(UpgradeTower)}]");

            }

            G();

        }

        /// <summary>
        /// can be item or rune
        /// </summary>
        /// <returns></returns>
        public bool IsItemOnScreen(bool updateScreen = true)
        {
            return !CheckSky(updateScreen) && PixelIn(335, 188, 1140, 700, Col(239, 209, 104)) && PixelIn(335, 188, 1140, 700, Col(224, 165, 86));
        }

        /// <summary>
        /// call when item is on screen
        /// </summary>
        /// <returns></returns>
        public ItemGrade GetItemGrade()
        {
            if (!IsItemOnScreen(false))
            {
                return ItemGrade.NoItem;
            }
            if (PixelIn(401, 200, 1192, 703, Col(68, 255, 218))) // A
            {
                return ItemGrade.A;
            }
            else if (PixelIn(401, 200, 1192, 703, Col(244, 86, 233))) // S
            {
                return ItemGrade.S;
            }
            else if (PixelIn(401, 200, 1192, 703, Col(255, 50, 50))) // L
            {
                return ItemGrade.L;
            }
            else if (PixelIn(401, 200, 1192, 703, Col(255, 216, 0))) // E
            {
                return ItemGrade.E;
            }
            else if (PixelIn(401, 200, 1192, 703, Col(218, 218, 218), out (int x, int y) ret))
            {
                // because B item label is white, and gray pixel can appear on letter edge
                if (PxlCount(ret.x - 5, ret.y - 5, ret.x + 5, ret.y + 5, Cst.White) == 0)
                {
                    return ItemGrade.B;
                }
            }
            return ItemGrade.None;
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

            if (WaitUntil(() => IsItemOnScreen() || CheckSky(false), delegate { }, 1050, 30))
            {
                Log.I($"item dropped");
                Wait(50);
                ItemGrade currentItemGrade = GetItemGrade();

                switch (dungeonToFarm)
                {
                    case Dungeon.GreenDragon:

                        switch (currentItemGrade)
                        {
                            case ItemGrade.B:
                                ItemDrop(ItemGrade.B, 0);
                                break;
                            default:
                                wrongItem = true;
                                ItemDrop(ItemGrade.None, 0);
                                break;
                        }
                        break;

                    case Dungeon.BlackDragon:

                        switch (currentItemGrade)
                        {
                            case ItemGrade.B:
                                ItemDrop(ItemGrade.B, 2);
                                break;
                            case ItemGrade.A:
                                ItemDrop(ItemGrade.A, 3);
                                break;
                            default:
                                wrongItem = true;
                                ItemDrop(ItemGrade.None, 0);
                                break;
                        }
                        break;
                    case Dungeon.RedDragon:

                        switch (currentItemGrade)
                        {
                            case ItemGrade.B:
                                ItemDrop(ItemGrade.B, 6);
                                break;
                            case ItemGrade.A:
                                ItemDrop(ItemGrade.A, 7);
                                break;
                            case ItemGrade.S:
                                ItemDrop(ItemGrade.S, 8);
                                break;
                            default:
                                wrongItem = true;
                                ItemDrop(ItemGrade.None, 0);
                                break;
                        }
                        break;
                    case Dungeon.Sin:
                        switch (currentItemGrade)
                        {
                            case ItemGrade.B:
                                ItemDrop(ItemGrade.B, 11);
                                break;
                            case ItemGrade.A:
                                ItemDrop(ItemGrade.A, 12);
                                break;
                            case ItemGrade.S:
                                ItemDrop(ItemGrade.S, 13);
                                break;
                            default:
                                wrongItem = true;
                                ItemDrop(ItemGrade.None, 0);
                                break;
                        }
                        break;
                    case Dungeon.LegendaryDragon:
                        switch (currentItemGrade)
                        {
                            case ItemGrade.A:
                                ItemDrop(ItemGrade.A, 16);
                                break;
                            case ItemGrade.S:
                                ItemDrop(ItemGrade.S, 17);
                                break;
                            case ItemGrade.L:
                                ItemDrop(ItemGrade.L, 18);
                                break;
                            default:
                                wrongItem = true;
                                ItemDrop(ItemGrade.None, 0);
                                break;
                        }
                        break;
                    case Dungeon.BoneDragon:
                        switch (currentItemGrade)
                        {
                            case ItemGrade.A:
                                ItemDrop(ItemGrade.A, 21);
                                break;
                            case ItemGrade.S:
                                ItemDrop(ItemGrade.S, 22);
                                break;
                            case ItemGrade.E:
                                ItemDrop(ItemGrade.E, 23);
                                break;
                            default:
                                wrongItem = true;
                                ItemDrop(ItemGrade.None, 0);
                                break;
                        }
                        break;
                    default:
                        wrongItem = true;
                        ItemDrop(ItemGrade.None, 0);
                        break;

                }
            }
            else
            {
                if (CheckGCMenu())
                {
                    Log.E($"Cant see GET button. rClick");
                    StepBack();
                    Wait(100);
                    G();
                }
            }

        }

        public bool CheckOnHint()
        {

            if (CheckSky(false) || P(19, 315) != Cst.SkyColor)
            {
                return false;
            }

            Log.Q($"hint check 1");
            Wait(200);

            if (CheckSky() || P(19, 315) != Cst.SkyColor)
            {
                Log.I($"wrong");
                return false;
            }

            Log.H($"hint check 2");
            Wait(250);

            if (CheckSky() || P(19, 315) != Cst.SkyColor)
            {
                Log.I($"wrong");
                return false;
            }

            Log.H($"hint check continuous");

            if (WaitUntil(() => CheckSky() || P(19, 315) != Cst.SkyColor, delegate { }, 1510, 50))
            {
                Log.I($"wrong");
                return false;
            }

            ScreenshotError(true, Cst.SCREENSHOT_HINT_PATH, true);
            Log.F($"unknown hint detected");
            WinAPI.SetForegroundWindow(hwnd);
            screenshotCache.SaveAllToFolder(Cst.SCREENSHOT_ERROR_SCREEN_CACHE_PATH);

            ScreenshotError(true, Cst.SCREENSHOT_HINT_PATH, true);
            Log.F($"___Hint detected___");
            Wait(7000);

            G();

            ScreenshotError(true, Cst.SCREENSHOT_HINT_PATH, true);

            Log.F($"___RESTART___");

            Restart();

            Log.F($"___RESTARTED___");
            Log.F($"30 s screenshotting");

            for (int i = 0; i < 10; i++)
            {
                ScreenshotError(true, Cst.SCREENSHOT_HINT_PATH, true);
                Log.E($"__Screen{i}__");
                Wait(3000);
                G();
            }

            ScreenshotError(true, Cst.SCREENSHOT_HINT_PATH, true);
            screenshotCache.SaveAllToFolder(Cst.SCREENSHOT_ERROR_SCREEN_CACHE_PATH);

            return true;

        }

        public void WaitForCancelABButton()
        {
            Log.I($"wait for cancel ab button");

            G();

            if (WaitUntil(() => P(788, 506) != Col(216, 51, 59), () => G(), 10_000, 200))
            {
                Wait(200);
                bool abLostPanel = false;

                if (WaitUntil(() => P(788, 506) == Col(216, 51, 59) || abLostPanel,
                () => {

                    AddSpeed();

                    if (!CheckSky())
                    {
                        CheckPausePanel(false);
                        CheckExitPanel(false);

                        if (CheckLoseABPanel(false))
                        {
                            Log.W($"lost on AB [WaitForCancelABButton]");
                            abLostPanel = true;
                        }
                    }

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
                        StepBack();
                        Wait(250);
                    }
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        ABTimerLabel.Content = string.Empty;
                    });
                    Log.UC($"cancel AB button didn't appear. Will restart");
                    Restart();
                }
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    ABTimerLabel.Content = string.Empty;
                });
                Log.UC($"cancel AB button didn't disappear. Will restart");
                Restart();
            }

        }

        public void ABWait(int secondsToWait)
        {

            int waveStartTimeout = 10_000;
            int waveFinishTimeout = maxBattleLength;

            // to ensure tower is upgraded and ad is watched after ab
            waitForAd = 100;
            replaysForUpgrade = 100;

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
                    if (CheckGCMenu())
                    {
                        Log.Q("Got in gc menu while waiting on ab");
                        timeToWait = TimeSpan.Zero;
                        quitWaiting = true;
                        return;
                    }
                    Dispatcher.Invoke(() =>
                    {
                        DateTime now = DateTime.Now;
                        ABTimerLabel.Content = $"AB wait {abStart + timeToWait - DateTime.Now:hh\\:mm\\:ss}\nWait for finish {currentTimeout - now:hh\\:mm\\:ss}\nWaves passed: {wavesCounter}";
                    });
                    AddSpeed();

                    bool notificationReady = notifyOn30Crystals && DateTime.Now - last30CrystalsNotificationTime > notifyOn30CrystalsInterval;
                    bool audioCheckReady = playAudioOn30Crystals && DateTime.Now - last30CrystalsAudioPlayTime > playAudioOn30CrystalsInterval;

                    if ((breakABOn30Crystals || notificationReady || audioCheckReady) && CountCrystals(true) >= 30)
                    {
                        if (breakABOn30Crystals)
                        {
                            Log.I($"30 crystals reached. break AB mode");
                            timeToWait = TimeSpan.Zero;
                            skipNextWave = true;
                            quitOn30Crystals = true;
                        }
                        if (notificationReady)
                        {
                            ShowBalloon("", "30 crystals collected");
                            last30CrystalsNotificationTime = DateTime.Now;
                        }
                        if (audioCheckReady)
                        {
                            string file = audio30crystalsIndex == 0 ? Cst.AUDIO_30_CRYSTALS_1_PATH : Cst.AUDIO_30_CRYSTALS_2_PATH;
                            PlayAudio(file, audio30crystalsIndex == 0 ? playAudio1On30CrystalsVolume : playAudio2On30CrystalsVolume);
                            last30CrystalsAudioPlayTime = DateTime.Now;
                        }
                    }
                }, waveFinishTimeout, 100))
                {
                    Log.E($"wave is going more than {maxBattleLength.ToString("N0", new NumberFormatInfo() { NumberGroupSeparator = " " })} ms. Will restart gc");
                    Log.ST();

                    ScreenshotError(screenshotABErrors, Cst.SCREENSHOT_AB_ERROR2_PATH);

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

                bool hintDetected = false;

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
                        // screen updated in CheckLoseABPanel
                        if (CheckOnHint())
                        {
                            hintDetected = true;
                            quitWaiting = true;
                            return;
                        }
                        CheckPausePanel(false);
                        CheckExitPanel(false);
                    }

                }, waveStartTimeout, 50))
                {

                    if (hintDetected)
                    {
                        break;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        ABTimerLabel.Content = $"Long wait";
                    });

                    Log.E($"wave switching is longer than {waveStartTimeout.ToString("N0", new NumberFormatInfo() { NumberGroupSeparator = " " })} ms. Will restart gc");
                    Log.ST();

                    ScreenshotError(screenshotABErrors, Cst.SCREENSHOT_AB_ERROR_PATH);

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
            freezeDetectionEnabled = false;
            try
            {

                if (currentTriesToStartDungeon >= maxTriesToStartDungeon)
                {
                    currentTriesToStartDungeon = 0;
                    Log.E($"Cannot open dungeon. Did {maxTriesToStartDungeon} tries");

                    if (replaysIfDungeonDontLoad)
                    {
                        Log.O($"replays will be called");
                        dungeonFarm = false;
                        makeReplays = true;
                        Dispatcher.Invoke(() =>
                        {
                            FarmDungeonCheckbox.Background = new SolidColorBrush(Colors.Red);
                            ReplaysCheckbox.Background = new SolidColorBrush(Colors.Lime);
                        });

                        return;
                    }
                    else
                    {
                        Log.F($"Will stop");
                        Halt();
                    }

                }

                Dungeon dungeonToStart = dungeonToFarm;
                Log.I($"Dungeon to start: {dungeonToStart}");

                bool allowedToMissClick = false;
                if (missClickDungeons && rand.NextDouble() < missClickDungeonsChance && !solvingCaptcha)
                {
                    allowedToMissClick = true;

                    Log.M("Missclick on dungeon allowed");

                    if (currentTriesToStartDungeon > 0)
                    {
                        Log.X("Didn't open dungeon on prev battle. Missclick discarded");
                        allowedToMissClick = false;
                    }
                }

                if (allowedToMissClick)
                {
                    dungeonToStart = dungeonsNeighbours[dungeonToFarm][rand.Next(dungeonsNeighbours[dungeonToFarm].Count)];
                    Log.M($"Missclick will be done. Will open {dungeonToStart}");
                }

                G();

                Log.I($"dungeon click. wait 15s for opening");

                RCI(699, 280, 752, 323);
                DateTime openDungeonTime = DateTime.Now;

                bool notAbleToOpenDungeons = false;

                WaitUntil(() => P(561, 676) == Col(69, 58, 48) || P(858, 575) == Col(255, 185, 0) || notAbleToOpenDungeons,
                () =>
                {
                    if (CheckSky() && DateTime.Now - openDungeonTime > TimeSpan.FromSeconds(3))
                    {
                        notAbleToOpenDungeons = true;
                    }
                }, 15_000, 30);

                bool openedDungeon = !notAbleToOpenDungeons;

                if (openedDungeon)
                {
                    Log.I($"dungeon button detected. click on dungeon");

                    if (solvingCaptcha && dungeonToFarm.IsDungeon())
                    {
                        Log.I($"captcha solving. green dragon click");

                        RCI(69, 179, 410, 229);
                        Wait(150);
                        RCI(1039, 728, 1141, 770);
                        Wait(750);
                        return;
                    }
                    else
                    {
                        switch (dungeonToStart)
                        {
                            case Dungeon.GreenDragon:
                                RCI(57, 168, 371, 218);
                                break;
                            case Dungeon.BlackDragon:
                                RCI(539, 170, 903, 227);
                                break;
                            case Dungeon.RedDragon:
                                RCI(1082, 166, 1368, 212);
                                break;
                            case Dungeon.Sin:
                                RCI(57, 308, 302, 366);
                                break;
                            case Dungeon.LegendaryDragon:
                                RCI(544, 304, 891, 365);
                                break;
                            case Dungeon.BoneDragon:
                                RCI(1094, 301, 1367, 367);
                                break;
                            case Dungeon.BeginnerDungeon:
                                RCI(160, 443, 414, 483);
                                break;
                            case Dungeon.IntermediateDungeon:
                                RCI(625, 444, 879, 485);
                                break;
                            case Dungeon.ExpertDungeon:
                                RCI(1113, 438, 1361, 486);
                                break;
                        }

                        Wait(150);
                        RCI(1039, 728, 1141, 770);

                        if (solvingCaptcha)
                        {
                            Wait(400);
                        }
                        else
                        {
                            Wait(200);
                            if (!simulateMouseMovement)
                            {
                                ChronoClick(out _);
                            }
                        }

                    }
                    Wait(400);

                    if (!CheckSky())
                    {
                        Log.K($"sky not clear[dungeon]");

                        if (!WaitUntil(() => CaptchaOnScreen(), delegate { }, 310, 10))
                        {
                            Log.W($"probably inventory is full");
                            Log.W($"couldnt figth dungeon. captcha wasn't detected");

                            currentTriesToStartDungeon++;

                            WaitUntilDeferred(() => CheckGCMenu(), StepBack, 5100, 500);
                        }
                        else
                        {
                            currentTriesToStartDungeon = 0;
                        }
                    }
                    else
                    {
                        Log.I($"dungeon started");
                        currentDungeonKills++;

                        if (currentDungeonKills > 0)
                        {
                            TimeSpan runningSpan = RunningTime;

                            TimeSpan avgKillTime = runningSpan / currentDungeonKills;
                            double killsPerHour = currentDungeonKills / runningSpan.TotalMilliseconds * 3_600_000;

                            string str = $"Kills: {currentDungeonKills}. Average kill time: {avgKillTime.TotalSeconds.ToString("N2", CultureInfo.InvariantCulture)}s. Kills per hour: {killsPerHour.ToString("N2", CultureInfo.InvariantCulture)}";

                            Dispatcher.Invoke(() =>
                            {
                                DungeonKillSpeedLabel.Content = str;
                            });

                            Log.I(str);

                        }


                        currentTriesToStartDungeon = 0;

                        lastReplayTime = DateTime.Now;

                        if (deathAltar && dungeonToFarm.IsDragon())
                        {
                            Log.D($"Click altar");
                            RCI(116, 215, 172, 294);
                            HeroClickWait(ActivationWaitBreakCondition, delegate { });
                            deathAltarUsed = true;
                        }
                        else
                        {
                            if (dungeonToFarm.IsDungeon() && dungeonStartCastOnBoss)
                            {
                                if (WaitUntil(() => P(834, 94) == Col(232, 77, 77), () => G(), 10_000, 100))
                                {
                                    if (deathAltar)
                                    {
                                        RCI(116, 215, 172, 294);
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
                    Log.W($"dungeon didn't load");

                    if (replaysIfDungeonDontLoad)
                    {
                        Log.O($"replays will be called");
                        dungeonFarm = false;
                        makeReplays = true;
                        Dispatcher.Invoke(() =>
                        {
                            FarmDungeonCheckbox.Background = new SolidColorBrush(Colors.Red);
                            ReplaysCheckbox.Background = new SolidColorBrush(Colors.Lime);
                        });
                        currentTriesToStartDungeon = 0;
                    }
                    else
                    {
                        TimeSpan waitSpan = GetRandomTimeSpan(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20));
                        Log.W($"Will wait for {waitSpan}");
                        Wait((int)waitSpan.TotalMilliseconds);
                    }
                    Wait(100);
                }

            }
            finally
            {
                freezeDetectionEnabled = true;
            }
        }

        public void PerformOrcBandAndMilit()
        {
            if (thisOrcBandSlot != -1 && (!orcBandOnSkipOnly || isSkip))
            {
                Log.I($"orcband click");
                RCI(orcBandX1, orcBandY1, orcBandX2, orcBandY2);
                HeroClickWait(ActivationWaitBreakCondition, delegate { });
            }
            if (thisMilitaryFSlot != -1 && (!militaryFOnSkipOnly || isSkip))
            {
                Log.I($"militaryF click");
                RCI(militX1, militY1, militX2, militY2);
                HeroClickWait(ActivationWaitBreakCondition, delegate { });
            }
        }

        public void PutOnAB()
        {

            G();
            if (P(1291, 794) != Col(98, 87, 73) ||
            P(1217, 800) != Col(98, 87, 73) ||
            P(1257, 756) != Col(98, 87, 73))
            {
                Log.Q("Cannot enable AB: button missing");
                return;
            }

            Log.I($"ab open");
            RandomWait(waitOnBattleButtonsMin, waitOnBattleButtonsMax);

            lastReplayTime = DateTime.Now;

            RCI(1236, 773, 1282, 819);

            WaitUntil(() => IsInStartABPanel(), delegate { }, 3000, 5);

            if (!IsInStartABPanel())
            {
                Log.UC("Opened start AB panel, but it didn't appear");
            }

            RandomWait(waitOnBattleButtonsMin, waitOnBattleButtonsMax);
            if (!abTab)
            {
                RCI(488, 457, 529, 491);
                Wait(50);
                RandomWait(waitOnBattleButtonsMin, waitOnBattleButtonsMax);
            }

            RCI(656, 445, 821, 503);
            Wait(300);

            if (!CheckSky())
            {
                Log.Q("Overlap after starting AB");

                if (WaitUntil(() => CheckSky() || IsInShop(false), delegate { }, 15_000, 50))
                {
                    if (CheckSky(false))
                    {
                        Log.I("Continue");
                    }
                    else if (IsInShop(false))
                    {
                        Log.F($"Shop opened. Gold or time ended. Will stop");

                        Wait(200);

                        if (!QuitBattle())
                        {
                            Log.W($"Couldn't quit battle");
                            Restart();
                        }
                        Halt();

                    }
                }
            }

        }

        public void PerformReplay()
        {
            Wait(50);

            Log.I("replay click");
            RCI(1124, 744, 1243, 814);
            Wait(200);
            RCI(940, 734, 1052, 790);
            if (solvingCaptcha)
            {
                Wait(400);
                return;
            }
            Wait(300);
            if (!CheckSky())
            {
                Log.K("sky not clear[replays]");
                Wait(400);
                if (CaptchaOnScreen())
                {
                    Log.I("captcha detected[replays]");
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
            if (skipWaves)
            {

                if (skipNextWave)
                {
                    Log.I($"skip anyways");
                }

                if (skipWithOranges || skipNextWave || CountCrystals(true) >= 30)
                {

                    Wait(150);

                    if (!CheckSky() || CheckGCMenu(false))
                    {
                        Log.I($"battle is not open [Perform_skip]");
                    }
                    else
                    {
                        Log.I($"skip 30 click");
                        RCI(889, 411, 984, 496);
                        //RandomMoveIn(889, 411, 984, 496);
                        freezeDetectionEnabled = false;
                        skipNextWave = false;

                        isSkip = true;

                        Wait(300);

                        if (!CheckSky())
                        {
                            Wait(350);

                            if (skipWithOranges && IsInShop())
                            {
                                StepBack();
                                Log.O($"oranges are over. disable skipping with oranges");
                                skipWithOranges = false;
                                Dispatcher.Invoke(() =>
                                {
                                    SkipWithOrangesCheckbox.Background = new SolidColorBrush(Colors.Red);
                                });
                                Wait(100);
                                isSkip = false;
                            }
                        }

                        freezeDetectionEnabled = true;
                    }
                }
                else
                {
                    Log.I($"<30 crystals. rClick");
                    isSkip = false;
                    StepBack();
                    Wait(300);
                }
            }
            else
            {
                Log.I($"no skip. esc click");
                isSkip = false;
                StepBack();
                Wait(300);
            }

        }

#if UNUSED_CODE
        /// <summary>
        /// Was used when could close your place in top panel after winning battle
        /// </summary>
        public void CloseTop()
        {
            Getscreen();
            if (Pxl(260, 130) != Cst.SkyColor)
            {
                RandomClickIn(239, 95, 291, 162);
                Wait(100);
            }
        }
#endif

        public void PerformABMode()
        {

            Log.I($"Perform_AB_mode");

            if (skipWaves)
            {
                if (abSkipNum < 1)
                {
                    if (CheckSky() && !CheckGCMenu(false))
                    {
                        Log.I($"sky clear on AB start [Perform_AB_mode, skipwaves]");

                        PerformSkip();
                        PerformOrcBandAndMilit();
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
                        Log.Q($"sky not clear [Perform_AB_mode, skipwaves]");
                    }
                }
                else
                {
                    PerformSkip();
                    PerformOrcBandAndMilit();
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
                if (CheckSky() && !CheckGCMenu(false))
                {
                    Log.I($"sky clear on AB start [Perform_AB_mode, no skipwaves]");

                    PerformSkip();
                    PerformOrcBandAndMilit();
                    PutOnAB();

                    int secondsToWait = rand.Next(secondsBetweenABSessionsMin, secondsBetweenABSessionsMax + 1);
                    Log.I($"AB {secondsToWait} seconds");
                    ABWait(secondsToWait);

                    lastReplayTime = DateTime.Now;
                }
                else
                {
                    Log.Q($"sky not clear [Perform_AB_mode, no skipwaves]");
                }
            }
        }

        public void PerformWaveCanceling()
        {
            Log.I($"Do wave canceling");
            PerformSkip();
            PerformOrcBandAndMilit();
            PutOnAB();
            lastReplayTime = DateTime.Now;
            waitForCancelABButton = true;
        }

        public void PerformManualBattleStart()
        {
            Log.I($"Do manual battle start");
            lastReplayTime = DateTime.Now;
            PerformSkip();
            ChronoClick(out _);
            PerformOrcBandAndMilit();
        }

        public bool IsReplayButtonsOpened()
        {
            return P(933, 795) == Col(235, 170, 23) && P(1114, 794) == Col(235, 170, 23) && P(1293, 796) == Col(235, 170, 23);
        }

        public bool IsHellButtonsOpened()
        {
            return P(1038, 796) == Col(235, 170, 23) && P(1038, 728) == Col(242, 190, 35) && P(1320, 730) == Col(242, 190, 35) && P(1320, 796) == Col(235, 170, 23);
        }
        public void Replay()
        {
            mimicOpened = false;
            waitForAd++;
            replaysForUpgrade++;
            pwTimer = false;
            abSkipNum--;
            healAltarUsed = false;
            deathAltarUsed = false;
            usedSingleClickHeros = false;
            Log.I("Do replay");

            if (!CheckSky() || !CheckGCMenu(false))
            {
                Log.E($"[Replay] Not in main menu");
                return;
            }

            if (dungeonFarm)
            {
                PerformDungeonStart();
                return;
            }

            if (IsReplayButtonsOpened())
            {
                Log.W("close replay buttons");
                RCI(1428, 676, 1457, 708);
                Wait(300);
            }
            if (IsHellButtonsOpened())
            {
                Log.W("close hell buttons");
                RCI(1428, 676, 1457, 708);
                Wait(300);
            }

            if (makeReplays)
            {
                PerformReplay();
                return;
            }
            Log.I("battle click");
            RCI(1319, 754, 1386, 785);
            Wait(200);

            RandomWait(waitOnBattleButtonsMin, waitOnBattleButtonsMax);
            if (solvingCaptcha)
            {
                Log.I("solving captcha. wait");
                Wait(500);
                return;
            }

            if (!CheckSky())
            {
                Log.K("sky overlapped after battle click");
                Wait(750);
                if (CaptchaOnScreen())
                {
                    Log.I("captcha on screen");
                    if (solveCaptcha)
                    {
                        SolveCaptcha();
                    }
                    else
                    {
                        StopClicker();
                    }
                    Log.I("Solved captcha. continue");
                }
                else
                {
                    Log.Q("no captcha found");
                }
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
                if (CheckSky() && !CheckGCMenu(false))
                {
                    WaitForCancelABButton();
                }
                else
                {
                    Log.Q("sky not clear after ab call");
                }
            }
        }

        public bool WaitIfDragonTimer(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            if (dungeonToFarm.IsDungeon() || P(605, 137) != Col(255, 79, 79))
            {
                return false;
            }
            Wait(50);
            Log.I("dungeon farm: timer detected. waiting for timer ends");

            if (simulateMouseMovement)
            {
                RMI(12, 669, 188, 848);
            }

            bool dungeonTimerDisappear = false;

            WaitUntil(() => dungeonTimerDisappear, delegate
            {
                if (P(605, 137) == Col(255, 79, 79))
                {
                    Wait(10);
                    G();
                }
                else
                {
                    if (CheckSky(false))
                    {
                        Log.I("timer ended");

                        if (speedupOnItemDrop)
                        {
                            Log.I("click on speed");
                            RCI(50, 781, 95, 825);
                            Wait(100);
                            RCI(50, 781, 95, 825);
                            if (DateTime.Now - x3Timer < TimeSpan.FromSeconds(1200.0) || iHaveX3)
                            {
                                Wait(100);
                                RCI(50, 781, 95, 825);
                            }

                        }
                        dungeonTimerDisappear = true;
                        Log.I("wait 4s for item drop");
                        WaitUntil(() => !CheckSky(), delegate { }, 4000, 0);
                        ShowBattleLength();
                    }
                }
            }, 10000, 0);
            if (P(1403, 799) != Col(152, 180, 28) && P(1403, 799) != Col(195, 207, 209))
            {
                GetItem();
            }

            return true;
        }

        public void WaitForAdEnd(bool x3Ad)
        {
            freezeDetectionEnabled = false;
            try
            {
                if (fixedAdWait > 0)
                {
                    Log.I($"[WaitForAdEnd] wait for {(float)fixedAdWait / 1000} s.");
                    Wait(fixedAdWait);
                }

                int maxEscClicks = 120;
                int escCounter = 0;

                while (!CheckSky() && escCounter < maxEscClicks)
                {
                    StepBack();
                    escCounter++;
                    Log.I($"ESC {escCounter}");

                    bool resumeAd = false;

                    WaitUntil(() => resumeAd, () =>
                    {

                        if (CheckSky())
                        {
                            resumeAd = true;
                            Log.I($"gc detected");
                            return;
                        }

                        if (AreColorsSimilar(P(891, 586), Col(62, 130, 247)))
                        {
                            Log.I($"pause button[1] detected. click and 3s wait");
                            LC(891, 586);
                            Wait(3000);
                            resumeAd = true;
                        }
                        else if (AreColorsSimilar(P(863, 538), Col(62, 130, 247)))
                        {
                            Log.I($"pause button[2] detected. click and 3s wait");
                            LC(863, 538);
                            Wait(3000);
                            resumeAd = true;
                        }
                        else if (AreColorsSimilar(P(863, 538), Col(62, 130, 247)))
                        {
                            Log.I($"pause button[3] detected. click and 3s wait");
                            LC(1079, 591);
                            Wait(3000);
                            resumeAd = true;
                        }
                    }, 1000, 30);
                }
                freezeDetectionEnabled = true;

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
            finally
            {
                freezeDetectionEnabled = true;
            }

        }

        public void TryUpgradeTower()
        {
            if (!upgradeCastle)
            {
                return;
            }

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

        public void UpgradeHero()
        {

            if (!CheckGCMenu())
            {
                Log.Q($"[{nameof(UpgradeHero)}] called not in gc menu");
                return;
            }

            Log.I($"[{nameof(UpgradeHero)}] called");

            if (CountCrystals(true) > 7)
            {

                Log.I($">7 crystals. open hero [{nameof(UpgradeHero)}]");

                Wait(200);

                switch (upgradeHeroNum)
                {
                    case 1:
                        RCI(323, 119, 363, 157);
                        break;
                    case 2:
                        RCI(417, 115, 461, 163);
                        break;
                    case 3:
                        RCI(508, 114, 550, 162);
                        break;
                    case 4:
                        RCI(324, 227, 368, 271);
                        break;
                    case 5:
                        RCI(417, 226, 462, 276);
                        break;
                    case 6:
                        RCI(509, 226, 555, 278);
                        break;
                    case 7:
                        RCI(319, 333, 367, 385);
                        break;
                    case 8:
                        RCI(412, 334, 463, 385);
                        break;
                    case 9:
                        RCI(507, 333, 553, 387);
                        break;
                    case 10:
                        RCI(321, 437, 369, 485);
                        break;
                    case 11:
                        RCI(413, 439, 460, 483);
                        break;
                    case 12:
                        RCI(507, 432, 557, 488);
                        break;
                    case 13:
                        RCI(222, 221, 272, 271);
                        break;
                }

                Wait(800);

                G();

                int cyanPxls = PxlCount(958, 586, 1126, 621, Col(0, 221, 255));
                Log.T($"Cyan pxls: {cyanPxls}");

                if (cyanPxls < 50 || cyanPxls > 150)
                {
                    Log.O($"hero is not crystal upgradable. quit hero upgrading and disable upgrading");
                    Dispatcher.Invoke(() =>
                    {
                        UpgradeHeroForCrystalsCheckbox.Background = new SolidColorBrush(Colors.Red);
                    });
                    upgradeHero = false;
                    StepBack();
                    Wait(200);
                    StepBack();
                    Wait(200);
                }
                else
                {
                    RCI(958, 554, 1108, 606);
                    int defaultLeftToUpgrade = rand.Next(10);
                    int leftToUpgrade = defaultLeftToUpgrade;
                    Wait(250);

                    int upgradeCounter = 0;
                    int maxUpgradesInRow = 90;

                    int crystalsCount = -1;

                    while (((crystalsCount = CountCrystals(false)) > 7 || leftToUpgrade > 0) && upgradeCounter < maxUpgradesInRow)
                    {

                        leftToUpgrade--;
                        if (crystalsCount > 7)
                        {
                            leftToUpgrade = defaultLeftToUpgrade;
                        }

                        cyanPxls = PxlCount(958, 586, 1126, 621, Col(0, 221, 255));
                        Log.T($"Cyan pxls: {cyanPxls}");

                        if (cyanPxls < 50 || cyanPxls > 150)
                        {
                            Log.O($"not seeing correct upgrade button. quit upgrading.");
                            Dispatcher.Invoke(() =>
                            {
                                UpgradeHeroForCrystalsCheckbox.Background = new SolidColorBrush(Colors.Red);
                            });
                            upgradeHero = false;
                            upgradeCounter = maxUpgradesInRow;
                        }
                        else
                        {
                            RCI(958, 554, 1108, 606);
                            Wait(250);
                            upgradeCounter++;
                        }

                    }

                    Wait(200);
                    StepBack();
                    Wait(200);
                    StepBack();
                    Wait(200);

                    G();

                }
            }
            else
            {
                Log.I($"no upgrading [{nameof(UpgradeHero)}]");
            }

            G();

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
                    Log.F("Wrong hero to upgrade slot");
                    WinAPI.ForceBringWindowToFront(this);
                    System.Windows.MessageBox.Show("upgrade hero number is wrong!", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
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

            if (PixelIn(401, 200, 1192, 703, dustColor) && !CheckSky(false))
            {
                Log.I($"item[{dustColor}] found");
                GetItem();
                return true;
            }

            return false;
        }

        public void StopClicker()
        {
            Log.F($"Clicker will be stopped");
            if (restartOnCaptcha)
            {
                Restart();
                Wait(300);

                TryUpgradeTower();
                TryUpgradeHero();

            }

            Log.F($"stopped");
            Halt();

        }

        public void EscClickStart()
        {

            Log.W($"try close overlap");
            Log.ST();

            ScreenshotError(screenshotOnEsc, Cst.SCREENSHOT_ON_ESC_PATH);

            int escCounter = 0;
            bool quitCycle = false;

            while (!CheckSky() && escCounter < 10 && !quitCycle)
            {

                if (CaptchaOnScreen(false))
                {
                    Log.W($"captcha[esc]");
                    quitCycle = true;
                }

                StepBack();

                escCounter++;

                Log.W($"esc {escCounter} pressing");
                Wait(500);
            }

            if (escCounter > 9)
            {

                Log.E($"10 escapes pressed. unknown thing");
                Log.ST();

                ScreenshotError(screenshotAfter10Esc, Cst.SCREENSHOT_AFTER_10_ESC_PATH);

                Restart();
                Wait(300);

            }

        }

        public void AdForCoins()
        {

            RCI(716, 765, 784, 801);
            Log.I($"[{nameof(AdForCoins)}] called. 4s wait");
            Wait(4000);
            G();

            WaitForAdEnd(false);
        }

        public void CheckAdForX3()
        {
            if (!adForX3 || !(DateTime.Now - x3Timer > TimeSpan.FromSeconds(3610.0)))
            {
                return;
            }
            freezeDetectionEnabled = false;
            try
            {
                Log.I($"ad for x3 open[{nameof(CheckAdForX3)}]");
                RCI(311, 44, 459, 68);
                Wait(500);

                if (WaitUntil(() => IsInShop() || CheckGCMenu(false), delegate { }, 15_000, 50) && CheckGCMenu())
                {
                    Log.I("Couldn't open shop. Will check again in 10 mins");
                    x3Timer = DateTime.Now - TimeSpan.FromMinutes(50);
                    return;
                }

                Log.I("Shop opened");
                Wait(300);

                RCI(1253, 93, 1337, 114);
                G();
                Log.I($"wait for loading[{nameof(CheckAdForX3)}]");
                bool quitCycle = false;
                if (WaitUntil(() => P(78, 418) == Col(98, 87, 73) || quitCycle, () =>
                {
                    if (CheckSky() && P(158, 795) == Col(98, 87, 73))
                    {
                        quitCycle = true;
                    }
                }, 15000, 150))
                {
                    Wait(400);
                    G();
                    if (P(147, 746) == Col(98, 87, 73))
                    {
                        Log.O("connection lost. Will check again in 10 mins");
                        x3Timer = DateTime.Now - TimeSpan.FromMinutes(50);
                        WaitUntilDeferred(() => CheckGCMenu(), () => StepBack(), 3100, 500);
                        Wait(500);
                        return;
                    }
                    Log.I("opened");
                    if (P(1365, 819) == Col(97, 86, 73))
                    {
                        Log.N("x3 is active. Will be checked after 3610 sec");
                        x3Timer = DateTime.Now;
                        File.WriteAllText(Cst.TIMER_X3_FILE_PATH, x3Timer.ToString("O"));
                        WaitUntilDeferred(() => CheckGCMenu(), () => StepBack(), 3100, 500);
                    }
                    else if (PixelIn(140, 253, 592, 367, Col(82, 255, 82)))
                    {
                        Log.I($"click on ad and 2 s wait[{nameof(CheckAdForX3)}]");
                        RCI(212, 634, 446, 670);
                        Wait(2000);
                        G();
                        if (P(78, 418) == Col(98, 87, 73))
                        {
                            Log.O($"ad didn't open. disable ad for x3");
                            Dispatcher.Invoke(() =>
                            {
                                AdForSpeedCheckbox.Background = new SolidColorBrush(Colors.Red);
                            });
                            adForX3 = false;
                            WaitUntilDeferred(() => CheckGCMenu(), () => StepBack(), 3100, 500);
                            Wait(300);
                        }
                        else
                        {
                            Log.I($"ad started. 4.5 s wait[{nameof(CheckAdForX3)}]");
                            Wait(4500);
                            G();
                            WaitForAdEnd(x3Ad: true);
                        }
                    }
                    else
                    {
                        Log.O("can't see ad for x3. disable ad for x3");
                        Dispatcher.Invoke(() =>
                        {
                            AdForSpeedCheckbox.Background = new SolidColorBrush(Colors.Red);
                        });
                        adForX3 = false;
                        WaitUntilDeferred(() => CheckGCMenu(), () => StepBack(), 3100, 500);
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
                        Log.N("no internet");
                    }
                    else
                    {
                        Log.N($"too long loading. restart will be called[{nameof(CheckAdForX3)}]");
                        Restart();
                    }
                    Wait(300);
                }
            }
            finally
            {
                freezeDetectionEnabled = true;
            }
        }

        public bool CloseOverlap()
        {
            G();
            if (CheckOnHint())
            {
                return true;
            }
            if (CaptchaOnScreen(false))
            {
                Log.I("captcha");
                return true;
            }
            if (IsItemOnScreen(false))
            {
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
                CheckRunePanel(false);
            }
            if (IsDungeonOpen())
            {
                Log.Q("Close dungeon");
                WaitUntilDeferred(() => CheckGCMenu(), StepBack, 5100, 500);
                if (CheckSky())
                {
                    return true;
                }
            }
            if (IsInDungeonList())
            {
                Log.Q("Close dungeon list");
                WaitUntilDeferred(() => CheckGCMenu(), StepBack, 5100, 500);
                if (CheckSky())
                {
                    return true;
                }
            }
            if (IsInForge())
            {
                QuitForge();
                if (CheckSky())
                {
                    return true;
                }
            }
            ClosePopup();
            CheckABExitPanel();
            CheckExitPanel(false);
            CheckPausePanel(false);
            CheckSkipPanel(false);
            CheckHeroPanel(false);
            CheckChooseClassPanel(false);
            CheckShop(false);
            return false;
        }

        public void WaitForAdAndWatch()
        {
            if ((!adAfterSkipOnly || isSkip) && (waitForAd > 2) && adForCoins && (adDuringX3 || DateTime.Now - x3Timer > TimeSpan.FromSeconds(1205)))
            {
                Log.I($"waiting ad for coins button");
                Wait(400);

                if (WaitUntil(() => IsAdForCoinsOnScreen(), delegate { }, 400, 10))
                {
                    Log.I($"button detected. ad for coins calling");
                    AdForCoins();
                    waitForAd = 0;
                }
                else
                {
                    Log.N($"button was not detected. continue");
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
            return !CheckSky() || CheckGCMenu(false) || dungeonFarm && WaitIfDragonTimer(false);
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
            if (waitAmount <= 0)
            {
                return true;
            }
            if (WaitUntil(breakCondition, actionBetweenChecks, waitAmount, 10))
            {
                Log.D("Stop hero waiting on breakCondition");
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
            int waitAmount = rand.Next(waitBetweenCastsMin, waitBetweenCastsMax);
            if (waitAmount <= 0)
            {
                return true;
            }
            if (WaitUntil(breakCondition, actionBetweenChecks, waitAmount, 10))
            {
                Log.D("Stop cast waiting on breakCondition");
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
            if (deathAltar && !deathAltarUsed && P(834, 94) == Col(232, 77, 77))
            {
                RCI(116, 215, 172, 294);
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
                Log.ST();

                ScreenshotError(screenshotLongWave, Cst.SCREENSHOT_LONG_WAVE_PATH);

                Restart();
                return false;
            }
            return true;
        }

        public bool LowHp()
        {
            G();
            return P(864, 54) == Cst.Black;
        }

        public bool SmithAndHealAltar()
        {

            if ((thisSmithSlot != -1 || healAltar) && LowHp())
            {
                if (thisSmithSlot != -1 && P(smithX, smithY) == Cst.BlueLineColor && !CheckGCMenu())
                {
                    RCI(smithX1, smithY1, smithX2, smithY2);
                    Log.I("smith clicked");
                    if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                    {
                        Log.D("Cancel by smith");
                        return false;
                    }
                }
                else if (healAltar && !healAltarUsed)
                {
                    RCI(116, 215, 172, 294);
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
            Log.I($"{nameof(ActivateHeroes)}");
            if (autobattleMode || waveCanceling)
            {
                Log.W($"Got in {nameof(ActivateHeroes)} when {nameof(autobattleMode)} = {autobattleMode}, {nameof(waveCanceling)} = {waveCanceling}");
                QuitBattle();
                return;
            }

            bool quitActivating = false;

            while (CheckSky() && !CheckGCMenu(false) && !quitActivating)
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
                    if (P(lx, ly) == Cst.BlueLineColor || CoinFlip(chanceToPressRed))
                    {
                        if (CheckGCMenu())
                        {
                            Log.I("gc menu detected while activating heroes");
                            goto ActivationQuit;
                        }
                        RCI(hx1, hy1, hx2, hy2);
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


                G();
                if (thisPureSlot != -1 && pwOnBoss && P(957, 96) == Col(232, 77, 77) && !pwTimer)
                {
                    Log.I("boss hp bar detected");
                    pwBossTimer = DateTime.Now;
                    pwTimer = true;
                    G();
                }

                if (thisPureSlot != -1 && P(pwX, pwY) == Cst.BlueLineColor)
                {
                    if (pwOnBoss && !dungeonFarmGlobal)
                    {
                        if (P(809, 95) == Cst.White || (((DateTime.Now - pwBossTimer > TimeSpan.FromMilliseconds((double)bossPause * 0.7) && DateTime.Now - x3Timer <= TimeSpan.FromSeconds(1205.0)) || (DateTime.Now - pwBossTimer > TimeSpan.FromMilliseconds(bossPause) && DateTime.Now - x3Timer > TimeSpan.FromSeconds(1205.0))) && pwTimer))
                        {
                            RCI(pwX1, pwY1, pwX2, pwY2);
                            if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                            {
                                Log.D("Cancel by pw 1");
                                goto ActivationQuit;
                            }
                        }
                    }
                    else
                    {
                        RCI(pwX1, pwY1, pwX2, pwY2);
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

                if (!CheckBattleLength())
                {
                    goto ActivationQuit;
                }

                if (!CastWait(ActivationWaitBreakCondition, delegate { }))
                {
                    Log.D("Cancel by cast");
                    goto ActivationQuit;
                }

            }
        ActivationQuit:
            Log.I($"Quit {nameof(ActivateHeroes)}");

            ;

        }

        public void ActivateHeroesDun()
        {

            Log.I($"{nameof(ActivateHeroesDun)}");
            bool quitActivating = false;

            while (CheckSky() && !CheckGCMenu(false) && !quitActivating)
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
                    if (firstUse || (P(lx, ly) == Cst.BlueLineColor || CoinFlip(chanceToPressRed)) && !CheckGCMenu())
                    {
                        RCI(hx1, hy1, hx2, hy2);
                        if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                        {
                            goto ActivationQuit;
                        }
                        G();

                        if (firstUse)
                        {
                            Log.T($"Press slot {slot}");
                            if (P(lx, ly) != Cst.Black)
                            {
                                Log.T($"Didn't press hero {slot}");
                                Wait(200);
                                RCI(hx1, hy1, hx2, hy2);
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

                if (thisPureSlot != -1 && P(pwX, pwY) == Cst.BlueLineColor)
                {
                    if (P(1407, 159) != Cst.CastleUpgradeColor)
                    {
                        RCI(pwX1, pwY1, pwX2, pwY2);
                        if (!HeroClickWait(ActivationWaitBreakCondition, delegate { }))
                        {
                            Log.D("Cancel by pw");
                            goto ActivationQuit;
                        }
                        G();
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
                    goto ActivationQuit;
                }

            }
        ActivationQuit:

            Log.I($"Quit {nameof(ActivateHeroesDun)}");
            if (dungeonToFarm.IsDungeon() && CheckGCMenu())
            {
                Wait(400);
            }

        }
    }
}
