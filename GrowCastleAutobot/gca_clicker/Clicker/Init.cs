using gca_clicker.Classes;
using gca_clicker.Clicker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        private bool solveCaptcha;
        private bool captchaSaveScreenshotsAlways = false;
        private bool captchaSaveFailedScreenshots = false;
        private bool screenshotCaptchaErrors = false;
        private bool restartOnCaptcha = false;

        private int deckToPlay = 0;

        private bool dungeonFarm = false;
        private bool dungeonFarmGlobal = false;
        private int dungeonNumber = 0;

        private bool screenshotRunes = false;
        private bool screenshotAfter10Esc = true;
        private bool screenshotLongWave = true;
        private bool screenshotItems = false;
        private bool screenshotIfLongGCLoad = true;
        private bool screenshotNoxLoadFail = true;
        private bool screenshotClearAllFail = true;
        private bool screenshotNoxMainMenuLoadFail = true;
        private bool screenshotOnEsc = true;


        private double mimicCollectPercent = 100;
        private bool wrongItem = false;

        private bool deleteB = false;
        private bool deleteA = false;
        private bool deleteS = false;
        private bool deleteL = false;
        private bool deleteE = false;


        private DateTime lastAddSpeed;
        private DateTime lastReplayTime;
        private DateTime lastCleanupTime;

        private TimeSpan addSpeedCheckInterval = new TimeSpan(0, 0, 1);


        private int heroClickPause = 50;

        private int fixedLoadingWait = 0;

        private bool restarted = false;

        private int maxRestartsForReset = 4;

        private bool breakABOn30Crystals = false;

        private bool skipNextWave = false;
        private bool skipWaves = false;
        private int manualsBetweenABSessions = 2;
        private bool isSkip = false;
        private bool orcBandOnSkipOnly = false;
        private int replaysForSkip = 10;
        private bool fiveWavesPauseSkip = false;
        private bool skipWithOranges = false;
        private int secondsBetweenSkips = 100;

        private bool replaysIfDungeonDontLoad = false;

        private bool makeReplays = false;


        private bool deathAltar = false;
        private bool healAltar = false;

        private bool deathAltarUsed = false;

        private bool dungeonStartCastOnBoss = false;

        private int dungeonStartCastDelay = 0;

        private int waitBeforeABOpen = 100;
        private int waitAfterABOpen = 100;
        private int waitAfterGabOpen = 100;

        private bool pwOnBoss = false;

        private bool abTab = false;
        private bool waveCanceling = false;
        private int abSkipNum = 0;

        private bool waitForCancelABButton = false;


        private bool pwTimer = false;

        private bool healAltarUsed = false;

        private int battleClickWait = 100;

        private bool autobattleMode = false;

        private DateTime x3Timer;
        private bool iHaveX3 = false;

        private bool autoUpgrade = false;
        private int upgradeHeroNum = 1;
        private bool upgradeHero = false;

        private int floorToUpgrade = 1;
        private int replaysForUpgrade = 0;

        private bool adForX3 = false;
        private bool adForCoins = false;
        private bool adAfterSkipOnly = false;
        private bool adDuringX3 = false;
        private int fixedAdWait = 0;

        private bool[] thisDeck = new bool[15];

        private int thisSmithSlot = 0;
        private int thisSmithX = 0;
        private int thisSmithY = 0;

        private int thisPureSlot = 0;
        private int thisPureX = 0;
        private int thisPureY = 0;

        public int thisChronoSlot = 0;
        public int thisChronoX = 0;
        public int thisChronoY = 0;

        private int thisOrcBandSlot = 0;
        private int thisOrcBandX = 0;
        private int thisOrcBandY = 0;

        private int thisMilitaryFSlot = 0;
        private int thisMilitaryFX = 0;
        private int thisMilitaryFY = 0;

        private int cleanupInterval = 10_800;

        private int maxBattleLength = 120_000;

        private DateTime pwBossTimer;
        private int bossPause = 0;
        private bool mimicOpened = false;
        private bool firstCrystalUpgrade = false;
        private bool solvingCaptcha = false;
        private int waitForAd = 4;

        private bool Init(out string message)
        {
            ClickerSettings s = GetClickerSettings();

            message = "";

            hwnd = WndFind(WindowName.Text);

            if (hwnd == IntPtr.Zero)
            {
                message += "Didn't find window\n";
            }
            else
            {

                (int x, int y, int width, int height) = GetWindowInfo(hwnd);

                backgroundMode = s.BackgroundMode;

                if (backgroundMode)
                {
                    if(Cst.WINDOW_WIDTH - width != 0)
                    {
                        message += $"Expand by {Cst.WINDOW_WIDTH - width}\n\n";
                    }
                }
                else
                {
                    if(x != 0)
                    {
                        message += $"Move window {-x} pxls right\n\n";
                    }
                    if(y != 0)
                    {
                        message += $"Move window {y} pxls up\n\n";
                    }
                    if (Cst.WINDOW_WIDTH - width != 0)
                    {
                        message += $"Expand by {Cst.WINDOW_WIDTH - width}\n\n";
                    }
                }
            }

            dungeonFarm = s.FarmDungeon;
            dungeonFarmGlobal = dungeonFarm;
            dungeonNumber = s.DungeonIndex + 1;

            dungeonStartCastDelay = s.CastOnBossInDungeonDelay;

            dungeonStartCastOnBoss = s.CastOnBossInDungeon;

            if (dungeonFarmGlobal && (dungeonNumber < 1 || dungeonNumber > 9))
            {
                message += "Wrong dungeon number\n";
            }

            try
            {
                x3Timer = DateTime.Parse(File.ReadAllText(Cst.TIMER_X3_FILE_PATH));
            }
            catch
            {
                x3Timer = DateTime.MinValue;
                File.WriteAllText(Cst.TIMER_X3_FILE_PATH, x3Timer.ToString("O"));
            }

            if (!File.Exists(Cst.DUNGEON_STATISTICS_PATH))
            {
                File.WriteAllText(Cst.DUNGEON_STATISTICS_PATH, Cst.DEFAULT_DUNGEON_STATISTICS);
            }

            deleteB = s.MatB;
            deleteA = s.MatA;
            deleteS = s.MatS;
            deleteL = s.MatL;
            deleteE = s.MatE;

            deckToPlay = s.BuildToPlayIndex + 1;
            if(deckToPlay == 0)
            {
                message += "Wrong deck to play\n";
            }

            skipWaves = s.SkipWaves;
            autobattleMode = s.ABMode;

            manualsBetweenABSessions = s.SkipsBetweenABSessions;

            makeReplays = s.MakeReplays;
            fiveWavesPauseSkip = s.FiveWavesBetweenSpiks;
            skipWithOranges = s.SkipWithOranges;

            adForX3 = s.AdForSpeed;
            adForCoins = s.AdForCoins;
            adAfterSkipOnly = s.AdAfterSkipOnly;
            adDuringX3 = s.AdDuringX3;


            solveCaptcha = s.SolveCaptcha;
            restartOnCaptcha = s.RestartOnCaptcha;


            if (solveCaptcha)
            {
                try
                {
                    int ret = execute(new byte[10], 0,0,0,0,false,false,out _, out int a, out double b, 1);

                    if(ret != 2 || a != 123)
                    {
                        message += "Error while calling gca_captcha_solver.dll";
                    }
                }
                catch (Exception e)
                {
                    message += "Error while calling gca_captcha_solver.dll: " + e.Message;
                }
            }


            healAltar = s.HealAltar;
            deathAltar = s.DeathAltar;

            pwOnBoss = s.PwOnBoss;
            bossPause = s.PwOnBossDelay;


            autoUpgrade = s.UpgradeCastle;
            floorToUpgrade = s.FloorToUpgradeCastle + 1;

            upgradeHero = s.UpgradeHero;
            upgradeHeroNum = s.SlotToUpgradeHero + 1;

            abTab = s.ABGabOrTab;
            secondsBetweenSkips = s.TimeToBreakAB;

            screenshotItems = s.ScreenshotItems;
            screenshotRunes = s.ScreenshotRunes;
            screenshotOnEsc = s.ScreenshotOnEsc;
            screenshotIfLongGCLoad = s.ScreenshotLongLoad;
            screenshotLongWave = s.ScreenshotLongWave;
            screenshotAfter10Esc = s.ScreenshotAfter10Esc;
            screenshotNoxLoadFail = s.ScreenshotNoxLoadFail;
            screenshotNoxMainMenuLoadFail = s.ScreenshotNoxMainMenuLoadFail;
            screenshotClearAllFail = s.ScreenshotClearAllFail;

            replaysIfDungeonDontLoad = s.MakeReplaysIfDungeonDontLoad;

            waveCanceling = s.ABWaveCanceling;
            breakABOn30Crystals = s.BreakAbOn30Crystals;

            captchaSaveScreenshotsAlways = s.ScreenshotSolvedCaptchas;
            captchaSaveFailedScreenshots = s.ScreenshotFailedCaptchas;

            DisableIncompatibleSettings();

            BuildUserControl build = BuildToPlayComboBox.SelectedIndex switch
            {
                0 => B1,
                1 => B2,
                2 => B3,
                3 => B4,
                4 => B5,
                _ => null!
            };

            if(build == null)
            {
                message += "Wrong build to play!\n";
            }

            BuildSettings buildSettings = build.GetBuildSettings();

            for(int i = 0; i < 15; i++)
            {
                thisDeck[i] = buildSettings.SlotsToPress[i];
            }

            thisPureSlot = buildSettings.PwSlot + 1;
            thisSmithSlot = buildSettings.SmithSlot + 1;
            thisChronoSlot = buildSettings.ChronoSlot + 1;
            thisOrcBandSlot = buildSettings.OrcBandSlot + 1;
            thisMilitaryFSlot = buildSettings.MiliitaryFSlot + 1;
            thisChronoSlot = buildSettings.ChronoSlot + 1;

            if(!InitHerosPositions(out string m))
            {
                message += '\n' + m;
            }

            return message.Length == 0;
        }


        public void DisableIncompatibleSettings()
        {

            if (adForCoins && !skipWaves)
            {
                adAfterSkipOnly = false;
            }

            if (dungeonFarm)
            {
                adForCoins = false;
                pwOnBoss = false;
            }

        }

        public bool InitHerosPositions(out string message)
        {
            message = "";
            thisPureX = 458;
            switch (thisPureSlot)
            {
                case 0:
                    thisPureX = 15;
                    thisPureY = 250;
                    break;
                case 2:
                    thisPureY = 92;
                    break;
                case 5:
                    thisPureY = 202;
                    break;
                case 8:
                    thisPureY = 310;
                    break;
                case 11:
                    thisPureY = 414;
                    break;
                default:
                    message += "Put pw on center\n";
                    break;
            }

            switch (thisSmithSlot)
            {
                case 0:
                    thisSmithX = 15;
                    thisSmithY = 200;
                    break;
                case 1:
                    thisSmithX = 364;
                    thisSmithY = 90;
                    break;
                case 2:
                    thisSmithX = 456;
                    thisSmithY = 90;
                    break;
                case 3:
                    thisSmithX = 547;
                    thisSmithY = 90;
                    break;
                case 4:
                    thisSmithX = 364;
                    thisSmithY = 202;
                    break;
                case 5:
                    thisSmithX = 456;
                    thisSmithY = 202;
                    break;
                case 6:
                    thisSmithX = 547;
                    thisSmithY = 202;
                    break;
                case 7:
                    thisSmithX = 364;
                    thisSmithY = 311;
                    break;
                case 8:
                    thisSmithX = 456;
                    thisSmithY = 311;
                    break;
                case 9:
                    thisSmithX = 547;
                    thisSmithY = 311;
                    break;
                case 10:
                    thisSmithX = 364;
                    thisSmithY = 415;
                    break;
                case 11:
                    thisSmithX = 456;
                    thisSmithY = 415;
                    break;
                case 12:
                    thisSmithX = 547;
                    thisSmithY = 415;
                    break;
                case 13:
                    thisSmithX = 271;
                    thisSmithY = 202;
                    break;
                default:
                    message += "Smith wrong slot\n";
                    break;
            }


            switch (thisChronoSlot)
            {
                case 0:
                    thisChronoX = 15;
                    thisChronoY = 200;
                    break;
                case 1:
                    thisChronoX = 364;
                    thisChronoY = 90;
                    break;
                case 2:
                    thisChronoX = 456;
                    thisChronoY = 90;
                    break;
                case 3:
                    thisChronoX = 547;
                    thisChronoY = 90;
                    break;
                case 4:
                    thisChronoX = 364;
                    thisChronoY = 202;
                    break;
                case 5:
                    thisChronoX = 456;
                    thisChronoY = 202;
                    break;
                case 6:
                    thisChronoX = 547;
                    thisChronoY = 202;
                    break;
                case 7:
                    thisChronoX = 364;
                    thisChronoY = 311;
                    break;
                case 8:
                    thisChronoX = 456;
                    thisChronoY = 311;
                    break;
                case 9:
                    thisChronoX = 547;
                    thisChronoY = 311;
                    break;
                case 10:
                    thisChronoX = 364;
                    thisChronoY = 415;
                    break;
                case 11:
                    thisChronoX = 456;
                    thisChronoY = 415;
                    break;
                case 12:
                    thisChronoX = 547;
                    thisChronoY = 415;
                    break;
                case 13:
                    thisChronoX = 271;
                    thisChronoY = 202;
                    break;
                default:
                    message += "Chrono wrong slot\n";
                    break;
            }


            switch (thisOrcBandSlot)
            {
                case 0:
                    thisOrcBandX = 15;
                    thisOrcBandY = 200;
                    break;
                case 1:
                    thisOrcBandX = 364;
                    thisOrcBandY = 90;
                    break;
                case 2:
                    thisOrcBandX = 456;
                    thisOrcBandY = 90;
                    break;
                case 3:
                    thisOrcBandX = 547;
                    thisOrcBandY = 90;
                    break;
                case 4:
                    thisOrcBandX = 364;
                    thisOrcBandY = 202;
                    break;
                case 5:
                    thisOrcBandX = 456;
                    thisOrcBandY = 202;
                    break;
                case 6:
                    thisOrcBandX = 547;
                    thisOrcBandY = 202;
                    break;
                case 7:
                    thisOrcBandX = 364;
                    thisOrcBandY = 311;
                    break;
                case 8:
                    thisOrcBandX = 456;
                    thisOrcBandY = 311;
                    break;
                case 9:
                    thisOrcBandX = 547;
                    thisOrcBandY = 311;
                    break;
                case 10:
                    thisOrcBandX = 364;
                    thisOrcBandY = 415;
                    break;
                case 11:
                    thisOrcBandX = 456;
                    thisOrcBandY = 415;
                    break;
                case 12:
                    thisOrcBandX = 547;
                    thisOrcBandY = 415;
                    break;
                case 13:
                    thisOrcBandX = 271;
                    thisOrcBandY = 202;
                    break;
                default:
                    message += "Orc band wrong slot\n";
                    break;
            }


            switch (thisMilitaryFSlot)
            {
                case 0:
                    thisMilitaryFX = 15;
                    thisMilitaryFY = 200;
                    break;
                case 1:
                    thisMilitaryFX = 364;
                    thisMilitaryFY = 90;
                    break;
                case 2:
                    thisMilitaryFX = 456;
                    thisMilitaryFY = 90;
                    break;
                case 3:
                    thisMilitaryFX = 547;
                    thisMilitaryFY = 90;
                    break;
                case 4:
                    thisMilitaryFX = 364;
                    thisMilitaryFY = 202;
                    break;
                case 5:
                    thisMilitaryFX = 456;
                    thisMilitaryFY = 202;
                    break;
                case 6:
                    thisMilitaryFX = 547;
                    thisMilitaryFY = 202;
                    break;
                case 7:
                    thisMilitaryFX = 364;
                    thisMilitaryFY = 311;
                    break;
                case 8:
                    thisMilitaryFX = 456;
                    thisMilitaryFY = 311;
                    break;
                case 9:
                    thisMilitaryFX = 547;
                    thisMilitaryFY = 311;
                    break;
                case 10:
                    thisMilitaryFX = 364;
                    thisMilitaryFY = 415;
                    break;
                case 11:
                    thisMilitaryFX = 456;
                    thisMilitaryFY = 415;
                    break;
                case 12:
                    thisMilitaryFX = 547;
                    thisMilitaryFY = 415;
                    break;
                case 13:
                    thisMilitaryFX = 271;
                    thisMilitaryFY = 202;
                    break;
                default:
                    message += "Military F wrong slot\n";
                    break;
            }

            return message.Length == 0;
        }

    }

}
