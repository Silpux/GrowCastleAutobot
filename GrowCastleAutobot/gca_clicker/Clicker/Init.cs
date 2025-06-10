using gca_clicker.Classes;
using gca_clicker.Clicker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int manualsBetweenSkips = 2;
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

        private bool thisDeck1 = false;
        private bool thisDeck2 = false;
        private bool thisDeck3 = false;
        private bool thisDeck4 = false;
        private bool thisDeck5 = false;
        private bool thisDeck6 = false;
        private bool thisDeck7 = false;
        private bool thisDeck8 = false;
        private bool thisDeck9 = false;
        private bool thisDeck10 = false;
        private bool thisDeck11 = false;
        private bool thisDeck12 = false;
        private bool thisDeck13 = false;
        private bool thisDeck14 = false;
        private bool thisDeck15 = false;


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
            message = "";
            backgroundMode = BackgroundModeCheckbox.IsChecked ?? false;

            hwnd = WndFind(WindowName.Text);

            if (hwnd == IntPtr.Zero)
            {
                message = "Didn't find window";
                return false;
            }


            (int x, int y, int width, int height) = GetWindowInfo(hwnd);

            if (backgroundMode)
            {
                if(Cst.WINDOW_WIDTH - width != 0)
                {
                    message += $"Expand by {Cst.WINDOW_WIDTH - width}\n\n";
                    return false;
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

                if(message.Length > 0)
                {
                    message = "Press set pos!\n\n" + message;
                    return false;
                }
            }

            dungeonFarm = FarmDungeonCheckbox.IsChecked ?? false;
            dungeonFarmGlobal = dungeonFarm;
            dungeonNumber = DungeonComboBox.SelectedIndex + 1;

            if (dungeonFarmGlobal && (dungeonNumber < 1 || dungeonNumber > 9))
            {
                message += "Wrong dungeon number\n";
                return false;
            }

            x3Timer = DateTime.Parse(File.ReadAllText(Cst.TIMER_X3_FILE_PATH));

            deleteB = MatBCheckbox.IsChecked ?? false;
            deleteA = MatACheckbox.IsChecked ?? false;
            deleteS = MatSCheckbox.IsChecked ?? false;
            deleteL = MatLCheckbox.IsChecked ?? false;
            deleteE = MatECheckbox.IsChecked ?? false;

            deckToPlay = BuildToPlayComboBox.SelectedIndex + 1;
            if(deckToPlay == 0)
            {
                message += "Wrong deck to play\n";
                return false;
            }

            skipWaves = SkipWavesCheckbox.IsChecked ?? false;
            autobattleMode = ABModeCheckbox.IsChecked ?? false;

            if(skipWaves && autobattleMode)
            {
                try
                {
                    manualsBetweenSkips = int.Parse(SkipsBetweenABSessionsTextBox.Text);
                }
                catch
                {
                    message += "Skips between ab sessions number is wrong\n";
                    return false;
                }

            }

            makeReplays = ReplaysCheckbox.IsChecked ?? false;
            fiveWavesPauseSkip = FiveWavesBetweenSkipsCheckbox.IsChecked ?? false;
            skipWithOranges = SkipWithOrangesCheckbox.IsChecked ?? false;

            adForX3 = AdForSpeedCheckbox.IsChecked ?? false;
            adForCoins = AdForCoinsCheckbox.IsChecked ?? false;
            adAfterSkipOnly = AdAfterSkipOnlyCheckbox.IsChecked ?? false;
            adDuringX3 = AdDuringx3Checkbox.IsChecked ?? false;


            solveCaptcha = SolveCaptchaCheckbox.IsChecked ?? false;
            restartOnCaptcha = RestartOnCaptchaCheckbox.IsChecked ?? false;

            healAltar = HealAltarCheckbox.IsChecked ?? false;
            deathAltar = DeathAltarCheckbox.IsChecked ?? false;

            pwOnBoss = PwOnBossCheckbox.IsChecked ?? false;
            if (pwOnBoss)
            {
                try
                {
                    bossPause = int.Parse(PwOnBossDelayTextBox.Text);
                }
                catch
                {
                    message += "Pw Delay number is wrong\n";
                    return false;
                }
            }

            autoUpgrade = UpgradeCastleCheckbox.IsChecked ?? false;
            if (autoUpgrade)
            {
                floorToUpgrade = FloorComboBox.SelectedIndex + 1;
                if(floorToUpgrade < 1 || floorToUpgrade > 4)
                {
                    message += "Wrong floor to upgrade\n";
                    return false;
                }
            }


            if (GabRadioButton.IsChecked ?? false) abTab = false;
            else if(TabRadioButton.IsChecked ?? false) abTab=true;
            else if (autobattleMode)
            {
                message += "Select gab or tab\n";
                return false;
            }

            if (autobattleMode)
            {
                try
                {
                    secondsBetweenSkips = int.Parse(TimeToBreakABTextBox.Text);
                }
                catch
                {
                    message += "Time to break AB is wrong\n";
                    return false;
                }
            }

            screenshotItems = ScreenshotItemsCheckbox.IsChecked ?? false;
            screenshotRunes = ScreenshotRunesCheckbox.IsChecked ?? false;
            screenshotOnEsc = ScreenshotOnEscCheckbox.IsChecked ?? false;
            screenshotIfLongGCLoad = ScreenshotLongLoadCheckbox.IsChecked ?? false;
            screenshotLongWave = ScreenshotLongWaveCheckbox.IsChecked ?? false;
            screenshotAfter10Esc = ScreenshotAfter10EscCheckbox.IsChecked ?? false;
            screenshotNoxLoadFail = ScreenshotNoxLoadFailCheckbox.IsChecked ?? false;
            screenshotNoxMainMenuLoadFail = ScreenshotNoxMainMenuLoadFailCheckbox.IsChecked ?? false;
            screenshotClearAllFail = ScreenshotNoxClearAllFailCheckbox.IsChecked ?? false;

            dungeonStartCastOnBoss = CastOnBossCheckbox.IsChecked ?? false;

            if (dungeonFarmGlobal)
            {
                try
                {
                    dungeonStartCastDelay = int.Parse(CastOnBossDelayTextBox.Text);
                }
                catch
                {
                    message += "Cast on boss delay number is wrong\n";
                    return false;
                }
            }

            replaysIfDungeonDontLoad = MakeReplaysIfDungeonDoesntLoadCheckBox.IsChecked ?? false;

            waveCanceling = ABWaveCancelingCheckbox.IsChecked ?? false;
            breakABOn30Crystals = BreakABOn30CrystalsCheckbox.IsChecked ?? false;

            upgradeHero = UpgradeHeroForCrystalsCheckbox.IsChecked ?? false;

            captchaSaveScreenshotsAlways = ScreenshotSolvedCaptchasCheckbox.IsChecked ?? false;
            captchaSaveFailedScreenshots = ScreenshotFailedCaptchasCheckbox.IsChecked ?? false;

            thisDeck1 = true;
            thisDeck2 = false;
            thisDeck3 = false;
            thisDeck4 = true;
            thisDeck5 = true;
            thisDeck6 = true;
            thisDeck7 = true;
            thisDeck8 = false;
            thisDeck9 = true;
            thisDeck10 = false;
            thisDeck11 = true;
            thisDeck12 = true;
            thisDeck13 = false;
            thisDeck14 = true;
            thisDeck15 = true;

            thisSmithSlot = 2;
            thisChronoSlot = 10;
            thisPureSlot = 8;



            if(!InitHerosPositions(out string m))
            {
                message += '\n' + m;
                return false;
            }
























                return true;

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
                    thisDeck2 = false;
                    thisPureY = 92;
                    break;
                case 5:
                    thisDeck5 = false;
                    thisPureY = 202;
                    break;
                case 8:
                    thisDeck8 = false;
                    thisPureY = 310;
                    break;
                case 11:
                    thisDeck11 = false;
                    thisPureY = 414;
                    break;
                default:
                    message = "Put pw on center";
                    return false;
            }

            switch (thisSmithSlot)
            {
                case 0:
                    thisSmithX = 15;
                    thisSmithY = 200;
                    break;
                case 1:
                    thisDeck1 = false;
                    thisSmithX = 364;
                    thisSmithY = 90;
                    break;
                case 2:
                    thisDeck2 = false;
                    thisSmithX = 456;
                    thisSmithY = 90;
                    break;
                case 3:
                    thisDeck3 = false;
                    thisSmithX = 547;
                    thisSmithY = 90;
                    break;
                case 4:
                    thisDeck4 = false;
                    thisSmithX = 364;
                    thisSmithY = 202;
                    break;
                case 5:
                    thisDeck5 = false;
                    thisSmithX = 456;
                    thisSmithY = 202;
                    break;
                case 6:
                    thisDeck6 = false;
                    thisSmithX = 547;
                    thisSmithY = 202;
                    break;
                case 7:
                    thisDeck7 = false;
                    thisSmithX = 364;
                    thisSmithY = 311;
                    break;
                case 8:
                    thisDeck8 = false;
                    thisSmithX = 456;
                    thisSmithY = 311;
                    break;
                case 9:
                    thisDeck9 = false;
                    thisSmithX = 547;
                    thisSmithY = 311;
                    break;
                case 10:
                    thisDeck10 = false;
                    thisSmithX = 364;
                    thisSmithY = 415;
                    break;
                case 11:
                    thisDeck11 = false;
                    thisSmithX = 456;
                    thisSmithY = 415;
                    break;
                case 12:
                    thisDeck12 = false;
                    thisSmithX = 547;
                    thisSmithY = 415;
                    break;
                case 13:
                    thisDeck13 = false;
                    thisSmithX = 271;
                    thisSmithY = 202;
                    break;
                default:
                    message = "Smith wrong slot";
                    return false;
            }


            switch (thisChronoSlot)
            {
                case 0:
                    thisChronoX = 15;
                    thisChronoY = 200;
                    break;
                case 1:
                    thisDeck1 = false;
                    thisChronoX = 364;
                    thisChronoY = 90;
                    break;
                case 2:
                    thisDeck2 = false;
                    thisChronoX = 456;
                    thisChronoY = 90;
                    break;
                case 3:
                    thisDeck3 = false;
                    thisChronoX = 547;
                    thisChronoY = 90;
                    break;
                case 4:
                    thisDeck4 = false;
                    thisChronoX = 364;
                    thisChronoY = 202;
                    break;
                case 5:
                    thisDeck5 = false;
                    thisChronoX = 456;
                    thisChronoY = 202;
                    break;
                case 6:
                    thisDeck6 = false;
                    thisChronoX = 547;
                    thisChronoY = 202;
                    break;
                case 7:
                    thisDeck7 = false;
                    thisChronoX = 364;
                    thisChronoY = 311;
                    break;
                case 8:
                    thisDeck8 = false;
                    thisChronoX = 456;
                    thisChronoY = 311;
                    break;
                case 9:
                    thisDeck9 = false;
                    thisChronoX = 547;
                    thisChronoY = 311;
                    break;
                case 10:
                    thisDeck10 = false;
                    thisChronoX = 364;
                    thisChronoY = 415;
                    break;
                case 11:
                    thisDeck11 = false;
                    thisChronoX = 456;
                    thisChronoY = 415;
                    break;
                case 12:
                    thisDeck12 = false;
                    thisChronoX = 547;
                    thisChronoY = 415;
                    break;
                case 13:
                    thisDeck13 = false;
                    thisChronoX = 271;
                    thisChronoY = 202;
                    break;
                default:
                    message = "Chrono wrong slot";
                    return false;
            }


            switch (thisOrcBandSlot)
            {
                case 0:
                    thisOrcBandX = 15;
                    thisOrcBandY = 200;
                    break;
                case 1:
                    thisDeck1 = false;
                    thisOrcBandX = 364;
                    thisOrcBandY = 90;
                    break;
                case 2:
                    thisDeck2 = false;
                    thisOrcBandX = 456;
                    thisOrcBandY = 90;
                    break;
                case 3:
                    thisDeck3 = false;
                    thisOrcBandX = 547;
                    thisOrcBandY = 90;
                    break;
                case 4:
                    thisDeck4 = false;
                    thisOrcBandX = 364;
                    thisOrcBandY = 202;
                    break;
                case 5:
                    thisDeck5 = false;
                    thisOrcBandX = 456;
                    thisOrcBandY = 202;
                    break;
                case 6:
                    thisDeck6 = false;
                    thisOrcBandX = 547;
                    thisOrcBandY = 202;
                    break;
                case 7:
                    thisDeck7 = false;
                    thisOrcBandX = 364;
                    thisOrcBandY = 311;
                    break;
                case 8:
                    thisDeck8 = false;
                    thisOrcBandX = 456;
                    thisOrcBandY = 311;
                    break;
                case 9:
                    thisDeck9 = false;
                    thisOrcBandX = 547;
                    thisOrcBandY = 311;
                    break;
                case 10:
                    thisDeck10 = false;
                    thisOrcBandX = 364;
                    thisOrcBandY = 415;
                    break;
                case 11:
                    thisDeck11 = false;
                    thisOrcBandX = 456;
                    thisOrcBandY = 415;
                    break;
                case 12:
                    thisDeck12 = false;
                    thisOrcBandX = 547;
                    thisOrcBandY = 415;
                    break;
                case 13:
                    thisDeck13 = false;
                    thisOrcBandX = 271;
                    thisOrcBandY = 202;
                    break;
                default:
                    message = "Orc band wrong slot";
                    return false;
            }


            switch (thisMilitaryFSlot)
            {
                case 0:
                    thisMilitaryFX = 15;
                    thisMilitaryFY = 200;
                    break;
                case 1:
                    thisDeck1 = false;
                    thisMilitaryFX = 364;
                    thisMilitaryFY = 90;
                    break;
                case 2:
                    thisDeck2 = false;
                    thisMilitaryFX = 456;
                    thisMilitaryFY = 90;
                    break;
                case 3:
                    thisDeck3 = false;
                    thisMilitaryFX = 547;
                    thisMilitaryFY = 90;
                    break;
                case 4:
                    thisDeck4 = false;
                    thisMilitaryFX = 364;
                    thisMilitaryFY = 202;
                    break;
                case 5:
                    thisDeck5 = false;
                    thisMilitaryFX = 456;
                    thisMilitaryFY = 202;
                    break;
                case 6:
                    thisDeck6 = false;
                    thisMilitaryFX = 547;
                    thisMilitaryFY = 202;
                    break;
                case 7:
                    thisDeck7 = false;
                    thisMilitaryFX = 364;
                    thisMilitaryFY = 311;
                    break;
                case 8:
                    thisDeck8 = false;
                    thisMilitaryFX = 456;
                    thisMilitaryFY = 311;
                    break;
                case 9:
                    thisDeck9 = false;
                    thisMilitaryFX = 547;
                    thisMilitaryFY = 311;
                    break;
                case 10:
                    thisDeck10 = false;
                    thisMilitaryFX = 364;
                    thisMilitaryFY = 415;
                    break;
                case 11:
                    thisDeck11 = false;
                    thisMilitaryFX = 456;
                    thisMilitaryFY = 415;
                    break;
                case 12:
                    thisDeck12 = false;
                    thisMilitaryFX = 547;
                    thisMilitaryFY = 415;
                    break;
                case 13:
                    thisDeck13 = false;
                    thisMilitaryFX = 271;
                    thisMilitaryFY = 202;
                    break;
                default:
                    message = "Military F wrong slot";
                    return false;
            }

            return true;
        }

    }

}
