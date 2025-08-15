using gca_clicker.Classes;
using gca_clicker.Classes.SettingsScripts;
using gca_clicker.Clicker;
using gca_clicker.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static gca_clicker.Classes.Utils;

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

        private int currentDungeonKills = -1;

        private Dungeon dungeonToFarm = Dungeon.None;

        private bool screenshotRunes = false;
        private bool screenshotAfter10Esc = true;
        private bool screenshotLongWave = true;
        private bool screenshotItems = false;
        private bool screenshotIfLongGCLoad = true;
        private bool screenshotABErrors = true;
        private bool screenshotOnFreezing = true;
        private bool screenshotNoxLoadFail = true;
        private bool screenshotClearAllFail = true;
        private bool screenshotNoxMainMenuLoadFail = true;
        private bool screenshotOnEsc = true;

        private bool screenshotPopups = true;
        private DateTime lastPopupScreenshot;
        private TimeSpan popupScreenshotInterval = TimeSpan.FromSeconds(2);

        private bool saveScreenshotsOnError = true;

        private int cacheDurationSec;
        private int cacheIntervalMs;
        private int cacheImageQuality;


        private double mimicCollectPercent = 100;
        private bool wrongItem = false;

        private bool deleteB = false;
        private bool deleteA = false;
        private bool deleteS = false;
        private bool deleteL = false;
        private bool deleteE = false;

        private int matGetTimeMin;
        private int matGetTimeMax;

        private Dungeon[,] dungeonsMatrix =
        {
            { Dungeon.GreenDragon, Dungeon.BlackDragon, Dungeon.RedDragon },
            { Dungeon.Sin, Dungeon.LegendaryDragon, Dungeon.BoneDragon},
            { Dungeon.BeginnerDungeon, Dungeon.IntermediateDungeon, Dungeon.ExpertDungeon},
        };

        private Dictionary<Dungeon, List<Dungeon>> dungeonsNeighbours = null!;

        private bool missClickDungeons;

        private double missClickDungeonsChance;
        private bool missClickDungeonsIncludeDiagonals;


        private DateTime lastAddSpeed;
        private DateTime lastReplayTime;

        private TimeSpan addSpeedCheckInterval = TimeSpan.FromSeconds(1);

        private int gcLoadingLimit = 30_000;

        private bool restarted = false;

        private int maxRestartsForReset = 4;

        private bool breakABOn30Crystals = false;
        private bool notifyOn30Crystals = true;
        private TimeSpan notifyOn30CrystalsInterval;

        private bool playAudioOn30Crystals = true;
        private TimeSpan playAudioOn30CrystalsInterval;
        private double playAudio1On30CrystalsVolume;
        private double playAudio2On30CrystalsVolume;

        private int audio30crystalsIndex = 0;

        private DateTime last30CrystalsNotificationTime;
        private DateTime last30CrystalsAudioPlayTime;

        private bool notificationOnlyMode;

        private bool skipNextWave = false;
        private bool skipWaves = false;

        private int skipsBetweenABSessionsMin = 3;
        private int skipsBetweenABSessionsMax = 5;

        private bool isSkip = false;
        private bool orcBandOnSkipOnly = false;
        private bool militaryFOnSkipOnly = false;
        private bool skipWithOranges = false;


        private int secondsBetweenABSessionsMin = 600;
        private int secondsBetweenABSessionsMax = 900;

        private bool replaysIfDungeonDontLoad = false;

        private bool makeReplays = false;

        private bool simulateMouseMovement = false;
        private (int x, int y) previousMousePosition;

        private bool monitorFreezing;

        private bool randomizeClickSequence = false;

        private int heroClickWaitMin;
        private int heroClickWaitMax;

        private int waitBetweenCastsMin;
        private int waitBetweenCastsMax;

        private bool deathAltar = false;
        private bool healAltar = false;

        private bool deathAltarUsed = false;

        private bool dungeonStartCastOnBoss = false;

        private int dungeonStartCastDelay = 0;

        private int waitOnBattleButtonsMin;
        private int waitOnBattleButtonsMax;

        private bool pwOnBoss = false;

        private bool abTab = false;
        private bool waveCanceling = false;
        private int abSkipNum = 0;

        private bool waitForCancelABButton = false;


        private bool pwTimer = false;

        private bool healAltarUsed = false;

        private bool autobattleMode = false;

        private DateTime x3Timer;
        private bool iHaveX3 = false;

        private bool upgradeCastle = false;
        private int upgradeHeroNum = 1;
        private bool upgradeHero = false;

        private int floorToUpgrade = 1;
        private int replaysForUpgrade = 0;

        private bool adForX3 = false;
        private bool adForCoins = false;
        private bool adAfterSkipOnly = false;
        private bool adDuringX3 = false;
        private int fixedAdWait;



        private bool[] thisDeck = new bool[15];
        private bool usedSingleClickHeros = false;

        private int thisSmithSlot = -1;
        private int smithX, smithY, smithX1, smithY1, smithX2, smithY2;

        private int thisPureSlot = -1;
        private int pwX, pwY, pwX1, pwY1, pwX2, pwY2;

        public int thisChronoSlot = -1;
        private int chronoX, chronoY, chronoX1, chronoY1, chronoX2, chronoY2;

        private int thisOrcBandSlot = -1;
        private int orcBandX, orcBandY, orcBandX1, orcBandY1, orcBandX2, orcBandY2;

        private int thisMilitaryFSlot = -1;
        private int militX, militY, militX1, militY1, militX2, militY2;

        private int cleanupIntervalMin = 7_200;
        private int cleanupIntervalMax = 14_400;

        private bool doSaveBeforeCleanup;

        private DateTime nextCleanupTime;
        private bool doResetOnCleanup = false;

        private bool doRestarts = false;
        private int restartIntervalMin;
        private int restartIntervalMax;
        private DateTime nextRestartDt = DateTime.MinValue;

        private int maxBattleLength = 120_000;

        private int maxTriesToStartDungeon = 3;
        private int currentTriesToStartDungeon = 0;

        private bool speedupOnItemDrop;

        private DateTime pwBossTimer;
        private int bossPause = 0;
        private bool mimicOpened = false;
        private bool firstCrystalUpgrade = true;

        /// <summary>
        /// is in process of solving captcha, this is not setting
        /// </summary>
        private bool solvingCaptcha = false;
        private int waitForAd = 4;

        private bool[,] buildMatrix = null!;
        private List<int> singleClickSlots = new();

        private List<WaitBetweenBattlesRuntime> waitBetweenBattlesRuntimes = null!;

        Stopwatch clickerStopwatch = new Stopwatch();
        public TimeSpan RunningTime => clickerStopwatch.Elapsed;
        public long RunningMs => clickerStopwatch.ElapsedMilliseconds;

        private bool Init(out string message)
        {
            ClickerSettings s = null!;

            try
            {
                s = GetClickerSettings(throwIfError: true);
            }
            catch (Exception e)
            {
                message = e.Message;
                return false;
            }

            frameHistory.Clear();

            message = "";

            restarted = false;

            lastReplayTime = DateTime.Now;

            doRestarts = s.DoRestarts;

            restartIntervalMin = s.RestartsIntervalMin;
            restartIntervalMax = s.RestartsIntervalMax;

            if (restartIntervalMin > restartIntervalMax)
            {
                message += $"{nameof(restartIntervalMin)} > {nameof(restartIntervalMax)}\n";
            }

            lastAddSpeed = default;

            wrongItem = false;

            coordNotTakenCounter = 0;
            hwnd = WndFind(WindowName.Text);

            if (hwnd == IntPtr.Zero)
            {
                message += $"Didn't find window: {WindowName.Text}\n";
                return false;
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

                if (message.Length > 0)
                {
                    return false;
                }

                if (!s.DisableResetCleanupCheck && !ResetAndCleanupCorrect())
                {
                    message += "Reset or cleanup button is not in correct place!\n";
                }
            }

            G();

            simulateMouseMovement = s.SimulateMouseMovement;
            WinAPI.GetCursorPos(out WinAPI.Point cursorPosition);
            previousMousePosition.x = cursorPosition.X;
            previousMousePosition.y = cursorPosition.Y;

            monitorFreezing = s.MonitorFreezing;

            maxBattleLength = s.MaxBattleLengthMs;
            if(maxBattleLength < 40_000)
            {
                message += $"{nameof(maxBattleLength)} must be 40s or more\n";
            }
            cleanupIntervalMin = s.CleanupIntervalSecMin;
            cleanupIntervalMax = s.CleanupIntervalSecMax;

            if(cleanupIntervalMin > cleanupIntervalMax)
            {
                message += $"{nameof(cleanupIntervalMin)} > {nameof(cleanupIntervalMax)}\n";
            }

            doResetOnCleanup = s.DoResetOnCleanup;
            doSaveBeforeCleanup = s.DoSaveOnCleanup;

            maxRestartsForReset = s.MaxRestartsForReset;

            orcBandOnSkipOnly = s.OrcbandOnSkipOnly;
            militaryFOnSkipOnly = s.MilitaryFOnSkipOnly;

            iHaveX3 = s.IHaveX3;

            speedupOnItemDrop = s.SpeedupOnItemDrop;
            currentTriesToStartDungeon = 0;

            mimicCollectPercent = 0;
            if (s.CollectMimic)
            {
                mimicCollectPercent = s.CollectMimicChance;
            }

            gcLoadingLimit = s.GcLoadingLimit;
            if(gcLoadingLimit < 20_000)
            {
                message += $"{nameof(gcLoadingLimit)} must be 20s or more\n";
            }
            fixedAdWait = s.FixedAdWait;

            randomizeClickSequence = s.RandomizeCastSequence;

            heroClickWaitMin = s.HeroClickWaitMin;
            heroClickWaitMax = s.HeroClickWaitMax;

            if (heroClickWaitMin > heroClickWaitMax)
            {
                message += $"{nameof(heroClickWaitMin)} > {nameof(heroClickWaitMax)}\n";
            }

            waitBetweenCastsMin = s.WaitBetweenCastsMin;
            waitBetweenCastsMax = s.WaitBetweenCastsMax;

            if (waitBetweenCastsMin > waitBetweenCastsMax)
            {
                message += $"{nameof(waitBetweenCastsMin)} > {nameof(waitBetweenCastsMax)}\n";
            }

            dungeonFarm = s.FarmDungeon;
            dungeonFarmGlobal = dungeonFarm;
            currentDungeonKills = -1;

            dungeonToFarm = dungeonFarmGlobal ? (Dungeon)(1 << s.DungeonIndex) : Dungeon.None;

            dungeonStartCastDelay = s.CastOnBossInDungeonDelay;

            dungeonStartCastOnBoss = s.CastOnBossInDungeon;

            if (dungeonFarmGlobal && !dungeonToFarm.IsValidDungeon())
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

            matGetTimeMin = s.MatGetDelayMin;
            matGetTimeMax = s.MatGetDelayMax;

            if (matGetTimeMin > matGetTimeMax)
            {
                message += $"{nameof(matGetTimeMin)} > {nameof(matGetTimeMax)}\n";
            }

            missClickDungeons = s.MissclickOnDungeons;
            missClickDungeonsIncludeDiagonals = s.MissclickOnDungeonsIncludeDiagonals;

            missClickDungeonsChance = (double)s.MissclickOnDungeonsChance / 1000;

            dungeonsNeighbours = GetNeighbors(dungeonsMatrix, missClickDungeonsIncludeDiagonals);

            deckToPlay = s.BuildToPlayIndex + 1;
            if(deckToPlay == 0)
            {
                message += "Wrong deck to play\n";
            }

            skipWaves = s.SkipWaves;
            isSkip = false;
            autobattleMode = s.ABMode;

            waitForCancelABButton = false;

            skipsBetweenABSessionsMin = s.SkipsBetweenABSessionsMin;
            skipsBetweenABSessionsMax = s.SkipsBetweenABSessionsMax;

            makeReplays = s.MakeReplays;
            skipWithOranges = s.SkipWithOranges;

            waitForAd = 2;

            adForX3 = s.AdForSpeed;
            adForCoins = s.AdForCoins;
            adAfterSkipOnly = s.AdAfterSkipOnly;
            adDuringX3 = s.AdDuringX3;

            if(adForX3 && iHaveX3)
            {
                message += "You have \"I have x3\" enabled. Cannot have ad for speed together\n";
            }

            solveCaptcha = s.SolveCaptcha;
            restartOnCaptcha = s.RestartOnCaptcha;
            solvingCaptcha = false;

            if (solveCaptcha)
            {
                try
                {
                    int ret = execute(new byte[10], 0,0,0,0,false,false,out _, out int a, out double b, 1);

                    if(ret != 2 || a != 123)
                    {
                        message += "Error while calling gca_captcha_solver.dll\n";
                    }
                }
                catch (Exception e)
                {
                    message += "Error while calling gca_captcha_solver.dll: " + e.Message + "\n";
                }
            }


            healAltar = s.HealAltar;
            deathAltar = s.DeathAltar;

            healAltarUsed = false;
            deathAltarUsed = false;


            pwTimer = false;

            pwOnBoss = s.PwOnBoss;
            bossPause = s.PwOnBossDelay;

            pwBossTimer = default;

            mimicOpened = false;

            firstCrystalUpgrade = true;
            upgradeCastle = s.UpgradeCastle;
            floorToUpgrade = s.FloorToUpgradeCastle + 1;

            upgradeHero = s.UpgradeHero;
            upgradeHeroNum = s.SlotToUpgradeHero + 1;

            abSkipNum = 0;

            abTab = s.ABGabOrTab;

            secondsBetweenABSessionsMin = s.TimeToBreakABMin;
            secondsBetweenABSessionsMax = s.TimeToBreakABMax;

            if (autobattleMode && secondsBetweenABSessionsMin > secondsBetweenABSessionsMax)
            {
                message += $"{nameof(secondsBetweenABSessionsMin)} > {nameof(secondsBetweenABSessionsMax)}\n";
            }

            waitOnBattleButtonsMin = s.WaitOnBattleButtonsMin;
            waitOnBattleButtonsMax = s.WaitOnBattleButtonsMax;

            if (autobattleMode && waitOnBattleButtonsMin > waitOnBattleButtonsMax)
            {
                message += $"{nameof(waitOnBattleButtonsMin)} > {nameof(waitOnBattleButtonsMax)}\n";
            }


            waitBetweenBattlesRuntimes = new(s.WaitBetweenBattlesSettings.Count);

            foreach (var wbbuc in GetWaitBetweenBattlesUserControls())
            {
                if (!wbbuc.IsChecked)
                {
                    continue;
                }
                try
                {
                    WaitBetweenBattlesRuntime wbbr = new(wbbuc.GetSetting(true));
                    waitBetweenBattlesRuntimes.Add(wbbr);
                }
                catch (Exception e)
                {
                    message += $"{e}\n";
                }
            }


            screenshotItems = s.ScreenshotItems;
            screenshotRunes = s.ScreenshotRunes;

            captchaSaveScreenshotsAlways = s.ScreenshotSolvedCaptchas;
            captchaSaveFailedScreenshots = s.ScreenshotFailedCaptchas;
            screenshotCaptchaErrors = s.ScreenshotCaptchaErrors;

            screenshotOnEsc = s.ScreenshotOnEsc;
            screenshotIfLongGCLoad = s.ScreenshotLongLoad;
            screenshotLongWave = s.ScreenshotLongWave;
            screenshotAfter10Esc = s.ScreenshotAfter10Esc;
            screenshotABErrors = s.ScreenshotABErrors;
            screenshotOnFreezing = s.ScreenshotOnFreezing;

            screenshotNoxLoadFail = s.ScreenshotNoxLoadFail;
            screenshotNoxMainMenuLoadFail = s.ScreenshotNoxMainMenuLoadFail;
            screenshotClearAllFail = s.ScreenshotClearAllFail;

            screenshotPopups = s.ScreenshotPopups;
            lastPopupScreenshot = DateTime.MinValue;

            saveScreenshotsOnError = s.SaveScreenshotsCacheOnError;


            cacheDurationSec = s.CacheDurationSeconds;
            cacheIntervalMs = s.CacheIntervalMs;
            cacheImageQuality = s.CacheImageQuality;
            if(cacheImageQuality < 10)
            {
                message += "Set cache image quality to 10 or more\n";
            }
            if(cacheDurationSec < 20)
            {
                message += "Set cache duration to 20 or more\n";
            }

            replaysIfDungeonDontLoad = s.MakeReplaysIfDungeonDontLoad;

            waveCanceling = s.ABWaveCanceling;
            breakABOn30Crystals = s.BreakAbOn30Crystals;

            notifyOn30Crystals = s.DesktopNotificationOn30Crystals;
            notifyOn30CrystalsInterval = TimeSpan.FromSeconds(s.DesktopNotificationOn30CrystalsInterval);
            
            playAudioOn30Crystals = s.PlayAudioOn30Crystals;
            playAudioOn30CrystalsInterval = TimeSpan.FromSeconds(s.PlayAudioOn30CrystalsInterval);
            playAudio1On30CrystalsVolume = Math.Clamp(s.PlayAudio1On30CrystalsVolume / 100.0, 0, 1);
            playAudio2On30CrystalsVolume = Math.Clamp(s.PlayAudio2On30CrystalsVolume / 100.0, 0, 1);
            audio30crystalsIndex = s.Audio30CrystalsIndex;

            last30CrystalsNotificationTime = DateTime.MinValue;
            last30CrystalsAudioPlayTime = DateTime.MinValue;

            notificationOnlyMode = s.NotificationOnlyMode;

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
                return false;
            }

            BuildSettings buildSettings = build.GetBuildSettings();

            for(int i = 0; i < 15; i++)
            {
                thisDeck[i] = buildSettings.SlotsToPress[i];
            }

            usedSingleClickHeros = false;
            singleClickSlots = buildSettings.SingleClickSlots;

            buildMatrix = new bool[,]{
                {false, thisDeck[0], thisDeck[1], thisDeck[2]},
                {thisDeck[12], thisDeck[3], thisDeck[4], thisDeck[5]},
                {false, thisDeck[6], thisDeck[7], thisDeck[8]},
                {thisDeck[13], thisDeck[9], thisDeck[10], thisDeck[11]},
                {thisDeck[14], false, false, false},
            };

            thisPureSlot = buildSettings.PwSlot;
            thisSmithSlot = buildSettings.SmithSlot;
            thisChronoSlot = buildSettings.ChronoSlot;
            thisOrcBandSlot = buildSettings.OrcBandSlot;
            thisMilitaryFSlot = buildSettings.MiliitaryFSlot;
            thisChronoSlot = buildSettings.ChronoSlot;

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
            if(thisPureSlot < -1 || thisPureSlot > 12)
            {
                message += "Pw wrong slot\n";
            }
            else if (thisPureSlot != -1 && (thisPureSlot - 1) % 3 != 0)
            {
                message += "Pw must be on center vertical\n";
            }
            else
            {
                (pwX, pwY) = GetHeroBlueLineCoords(thisPureSlot);
                (pwX1, pwY1, pwX2, pwY2) = GetHeroRect(thisPureSlot);
            }

            if (thisSmithSlot < -1 || thisSmithSlot > 12)
            {
                message += "Smith wrong slot\n";
            }
            else
            {
                (smithX, smithY) = GetHeroBlueLineCoords(thisSmithSlot);
                (smithX1, smithY1, smithX2, smithY2) = GetHeroRect(thisSmithSlot);
            }

            if (thisChronoSlot < -1 || thisChronoSlot > 12)
            {
                message += "Chrono wrong slot\n";
            }
            else
            {
                (chronoX, chronoY) = GetHeroBlueLineCoords(thisChronoSlot);
                (chronoX1, chronoY1, chronoX2, chronoY2) = GetHeroRect(thisChronoSlot);
            }


            if (thisOrcBandSlot < -1 || thisOrcBandSlot > 12)
            {
                message += "Orc band wrong slot\n";
            }
            else
            {
                (orcBandX, orcBandY) = GetHeroBlueLineCoords(thisOrcBandSlot);
                (orcBandX1, orcBandY1, orcBandX2, orcBandY2) = GetHeroRect(thisOrcBandSlot);
            }


            if (thisMilitaryFSlot < -1 || thisMilitaryFSlot > 12)
            {
                message += "Orc band wrong slot\n";
            }
            else
            {
                (militX, militY) = GetHeroBlueLineCoords(thisMilitaryFSlot);
                (militX1, militY1, militX2, militY2) = GetHeroRect(thisMilitaryFSlot);
            }

            return message.Length == 0;
        }

        public bool ResetAndCleanupCorrect()
        {
            G();
            currentScreen = Colormode(7, 1477, 268, 1519, 352, currentScreen);

            // reset
            return P(1499, 333) == Col(127, 127, 127) &&
            P(1501, 330) == Col(127, 127, 127) &&
            P(1498, 330) == Col(127, 127, 127) &&
            P(1501, 333) == Col(127, 127, 127) &&
            P(1497, 324) == Col(127, 127, 127) &&
            P(1502, 324) == Col(127, 127, 127) &&
            P(1507, 329) == Col(127, 127, 127) &&
            P(1507, 334) == Col(127, 127, 127) &&
            P(1502, 339) == Col(127, 127, 127) &&
            P(1497, 339) == Col(127, 127, 127) &&
            P(1492, 334) == Col(127, 127, 127) &&
            P(1492, 329) == Col(127, 127, 127) &&
            P(1495, 336) == Cst.White &&
            P(1504, 336) == Cst.White &&
            P(1495, 327) == Cst.White &&
            P(1504, 327) == Cst.White &&

            // cleanup
            P(1500, 291) == Col(127, 127, 127) &&
            P(1490, 292) == Col(127, 127, 127) &&
            P(1498, 301) == Col(127, 127, 127);
        }

    }

}
