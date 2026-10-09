using gca.Classes;
using gca.Classes.SettingsScripts;
using gca.Enums;
using gca.Script;
using gca.Structs;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using static gca.Classes.Utils;

namespace gca
{
    public partial class Autobot
    {

        private bool solveCaptcha;
        private bool captchaSaveScreenshotsAlways = false;
        private bool captchaSaveFailedScreenshots = false;
        private bool screenshotCaptchaErrors = false;

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
        private bool screenshotLDPlayerLoadFail = true;
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
        private bool deleteU = false;

        private int openDungeonClickDelayMin;
        private int openDungeonClickDelayMax;

        private int matGetTimeMin;
        private int matGetTimeMax;

        private Dungeon[,] dungeonsMatrix =
        {
            { Dungeon.GreenDragon, Dungeon.BlackDragon, Dungeon.RedDragon },
            { Dungeon.Sin, Dungeon.LegendaryDragon, Dungeon.BoneDragon},
            { Dungeon.AncientDragon, Dungeon.None, Dungeon.None},
            { Dungeon.None, Dungeon.None, Dungeon.None},
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
        private bool log30DetectionsInNotificationMode;

        private bool skipNextWave = false;
        private bool skipWaves = false;

        private bool isSkip = false;
        private bool orcBandOnSkipOnly = false;
        private bool militaryFOnSkipOnly = false;
        private bool skipWithOranges = false;

        private bool replaysIfDungeonDontLoad = false;
        private bool stopAfterNKills = false;
        private int leftDungeonKills = 0;

        private bool makeReplays = false;

        private bool simulateMouseMovement = false;
        private (int x, int y) previousMousePosition;

        private bool simulateKeyBindingOnDungeonEnter = false;
        private (int x, int y) openDungeonsListPressCoords;
        private (int x, int y) openDungeonPressCoords;
        private (int x, int y) battleDungeonPressCoords;

        private bool monitorFreezing;

        private bool randomizeClickSequence = false;
        private bool pressAllHeroesOnCast = false;
        private bool pressFixedPointInHero = false;

        private bool screwUpCast = true;
        private double swapNeighbourHerosChance = 0.5;
        private double castHeroOnEndChance = 0.5;

        private (int x, int y)[] heroPressCoords = new (int x, int y)[15];

        private bool ignoreWaitsBetweenBattlesOnX3FromAd = false;

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


        private bool autobattleMode = false;
        private bool abTab = false;

        private bool infiniteAB = false;

        private int timeToBreakABMin = 600;
        private int timeToBreakABMax = 900;

        private bool tryToSkipEveryBattle = false;

        private int secondsBetweenSkipsMin = 600;
        private int secondsBetweenSkipsMax = 900;

        private int battlesWithSkipsMin = 3;
        private int battlesWithSkipsMax = 4;

        private bool setExitAfterNextBattle = false;

        private bool pwTimer = false;

        private bool healAltarUsed = false;

        private DateTime x3Timer;

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
        private int smithX, smithY;
        private Bounds smithBounds;

        private int thisPureSlot = -1;
        private int pwX, pwY;
        private Bounds pwBounds = default;

        public int thisChronoSlot = -1;
        private int chronoX, chronoY;
        private Bounds chronoBounds = default;

        private int thisOrcBandSlot = -1;
        private int orcBandX, orcBandY;
        private Bounds orcBandBounds = default;

        private int thisMilitaryFSlot = -1;
        private int militX, militY;
        private Bounds militBounds = default;

        private int cleanupIntervalMin = 7_200;
        private int cleanupIntervalMax = 14_400;

        private bool doSaveBeforeCleanup;

        private DateTime nextCleanupTime;

        private bool doRestarts = false;
        private int restartIntervalMin;
        private int restartIntervalMax;
        private DateTime nextRestartDt = DateTime.MinValue;

        private int maxBattleLength = 120_000;

        private int maxTriesToStartDungeon = 3;
        private int currentTriesToStartDungeon = 0;

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
        private int[] heroPressOrder = null!;

        private int testMouseMoveX1;
        private int testMouseMoveX2;
        private int testMouseMoveY1;
        private int testMouseMoveY2;

        private bool countCrystalsTestDarkMode;

        private bool onlineActionsTest_OpenGuildTest;
        private bool onlineActionsTest_OpenRandomProfileFromGuildTest;
        private bool onlineActionsTest_OpenGuildsTopTest;
        private bool onlineActionsTest_OpenTopTest;
        private bool onlineActionsTest_OpenTopSeasonTest;
        private bool onlineActionsTest_OpenHellSeasonMyTest;
        private bool onlineActionsTest_OpenHellSeasonTest;
        private bool onlineActionsTest_OpenWavesTopMyTest;
        private bool onlineActionsTest_OpenWavesTopTest;
        private bool onlineActionsTest_PressDeckTest;
        private bool onlineActionsTest_CraftStonesTest;
        private bool onlineActionsTest_DoSaveTest;

        private List<WaitBetweenBattlesRuntime> waitBetweenBattlesRuntimes = null!;

        Stopwatch clickerStopwatch = new Stopwatch();

        public string AppVersion { get; set; } = "";
        public string ID { get; set; } = "";
        public int TotalCaptchasSolved { get; set; } = 0;
        public string SettingsString { get; set; } = "";
        public bool? IsRepo { get; set; } = null;
        public DateTime AppStartTime { get; set; }
        public TimeSpan AppRunningTime => DateTime.Now - AppStartTime;

        public TimeSpan RunningTime => clickerStopwatch.Elapsed;
        public long RunningMs => clickerStopwatch.ElapsedMilliseconds;

        private ClickerSettings GetCurrentSettingsFromFile()
        {
            ClickerSettings settings = null!;
            try
            {
                string json = File.ReadAllText(Cst.CURRENT_SETTINGS_FILE_PATH);
                settings = JsonSerializer.Deserialize<ClickerSettings>(json)!;
            }
            catch
            {
                settings = new();
            }
            return settings;
        }

        public void Init(
            string windowName,
            IEnumerable<WaitBetweenBattlesUserControl> waitBetweenBattlesUserControls,
            BuildUserControl build)
        {
            this.windowName = windowName;
            this.waitBetweenBattlesUserControls = waitBetweenBattlesUserControls;
            this.build = build;
        }

        private void GetRenderHwnd(IntPtr hwnd)
        {
            renderHwnd = WinAPI.FindChildWindowByClass(hwnd, "RenderWindow");
        }
        public IntPtr GetLDPlayerWindow()
        {
            return WndFind(windowName);
        }

        private bool InitParameters(out string message)
        {
            ClickerSettings s = null!;
            ClickerSettings scopy = null!;

            try
            {
                s = GetCurrentSettingsFromFile();
                scopy = GetCurrentSettingsFromFile();
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
            hwnd = GetLDPlayerWindow();

            if (hwnd == IntPtr.Zero)
            {
                message += $"Didn't find window: {windowName}\n";
                return false;
            }
            else
            {
                (int x, int y, int width, int height) = GetWindowInfo(hwnd);

                backgroundMode = s.BackgroundMode;

                if (!backgroundMode)
                {
                    if (x != 0)
                    {
                        message += $"Move window {-x} pxls right\n\n";
                    }
                    if (y != 0)
                    {
                        message += $"Move window {y} pxls up\n\n";
                    }
                }
                if (Cst.WINDOW_WIDTH - width != 0)
                {
                    int expand = Cst.WINDOW_WIDTH - width;
                    message += $"Expand by {Cst.WINDOW_WIDTH - width}\n\n";
                    if(expand == -400 || expand == -1040 || expand == -2320 || expand == -6160 || expand == 154)
                    {
                        message += "Don't maximize LDPlayer and press \"Set pos\" button\n\n";
                    }
                }

                GetRenderHwnd(hwnd);

                if(renderHwnd == IntPtr.Zero)
                {
                    message += "Couldn't find LDPlayer render window\n";
                }

                WinAPI.GetWindowThreadProcessId(hwnd, out uint pid);

                if (pid == 0)
                {
                    message += "Couldn't get LDPlayer process id\n";
                }

                Process? process = Process.GetProcessById((int)pid);

                string? processPath = process.MainModule?.FileName;

                if (string.IsNullOrEmpty(processPath))
                {
                    message += "Couldn't get LDPlayer process path\n";
                }

                string directory = Path.GetDirectoryName(processPath)!;
                string consolePath = Path.Combine(directory, "ldconsole.exe");

                if (!File.Exists(consolePath))
                {
                    message += $"Couldn't find ldconsole.exe file.\nIt should be in ldplayer install path: {consolePath}\n";
                    return false;
                }

                ldConsolePath = consolePath;


                ProcessStartInfo psiGetVersion = new ProcessStartInfo
                {
                    FileName = ldConsolePath,
                    Arguments = $"",

                    UseShellExecute = false,
                    CreateNoWindow = true,

                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using Process getVersionProcess = new Process
                {
                    StartInfo = psiGetVersion
                };

                getVersionProcess.Start();

                string stdout = getVersionProcess.StandardOutput.ReadToEnd();
                string stderr = getVersionProcess.StandardError.ReadToEnd();

                getVersionProcess.WaitForExit();

                string infoOutput = stdout;

                string infoOutputFirstLine = infoOutput.Split("\n").Where(x => x.Length > 0).First();
                Log.I("First line of console output:");
                Log.I($"{infoOutputFirstLine}");

                if (!string.IsNullOrWhiteSpace(stderr))
                {
                    infoOutput += Environment.NewLine + stderr;
                }

                Match match = Regex.Match(infoOutput, @"\d+\.\d+\.\d+\.\d+");

                string version = "";

                if (match.Success)
                {
                    version = match.Value;
                }

                Log.I($"LDPlayer version: {version}");

                if (version.Split(".")[0] != "9")
                {
                    message += $"Required LDPlayer 9. Current version: {version}";
                }


                if (message.Length > 0)
                {
                    return false;
                }


                ProcessStartInfo psiGetInfo = new ProcessStartInfo
                {
                    FileName = ldConsolePath,
                    Arguments = $"list2",

                    UseShellExecute = false,
                    CreateNoWindow = true,

                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using Process infoProcess = new Process
                {
                    StartInfo = psiGetInfo
                };

                infoProcess.Start();

                stdout = infoProcess.StandardOutput.ReadToEnd();
                stderr = infoProcess.StandardError.ReadToEnd();

                infoProcess.WaitForExit();

                string output = stdout;

                if (!string.IsNullOrWhiteSpace(stderr))
                {
                    output += Environment.NewLine + stderr;
                }

                string? currentEmulatorInfo = output.Split("\n").Where(x => x.Split(",").Length == 10).FirstOrDefault(x => x.Split(",")[1] == windowName);

                if (string.IsNullOrEmpty(currentEmulatorInfo))
                {
                    message += "Couldn't get emulator info\n";
                    string windowNameLower = windowName.ToLower();
                    string? possibleWindow = output.Split("\n").Where(x => x.Split(",").Length == 10).FirstOrDefault(x => x.Split(",")[1].ToLower() == windowNameLower)?.Split(",")[1];
                    if (possibleWindow != null)
                    {
                        message += $"Didn't find window '{windowName}'.\nFound emulator: '{possibleWindow}'.\nCheck letter case in window name!\n";
                    }
                    return false;
                }

                Log.I($"LDPlayer info: {currentEmulatorInfo}");

                int resolutionWidth = 0;
                int resolutionHeight = 0;

                try
                {
                    string[] parts = currentEmulatorInfo.Split(",");
                    resolutionWidth = int.Parse(parts[7]);
                    resolutionHeight = int.Parse(parts[8]);
                }
                catch
                {
                    message += "Couldn't get emulator resolution\n";
                    return false;
                }

                if(resolutionWidth != 1600 || resolutionHeight != 900)
                {
                    message += "Change emulator resolution to 1600x900\n";
                    return false;
                }

            }

            simulateMouseMovement = s.SimulateMouseMovement;
            simulateKeyBindingOnDungeonEnter = s.SimulateKeyBindingOnDungeonEnter;

            openDungeonsListPressCoords.x = Math.Min(Math.Max(s.DungeonPressXPositions[0], 0), 100);
            openDungeonsListPressCoords.y = Math.Min(Math.Max(s.DungeonPressYPositions[0], 0), 100);
            openDungeonPressCoords.x = Math.Min(Math.Max(s.DungeonPressXPositions[1], 0), 100);
            openDungeonPressCoords.y = Math.Min(Math.Max(s.DungeonPressYPositions[1], 0), 100);
            battleDungeonPressCoords.x = Math.Min(Math.Max(s.DungeonPressXPositions[2], 0), 100);
            battleDungeonPressCoords.y = Math.Min(Math.Max(s.DungeonPressYPositions[2], 0), 100);

            WinAPI.GetCursorPos(out WinAPI.Point cursorPosition);
            previousMousePosition.x = cursorPosition.X;
            previousMousePosition.y = cursorPosition.Y;

            monitorFreezing = s.MonitorFreezing;

            maxBattleLength = s.MaxBattleLengthMs;
            if (maxBattleLength < 40_000)
            {
                message += $"{nameof(maxBattleLength)} must be 40s or more\n";
            }
            cleanupIntervalMin = s.CleanupIntervalSecMin;
            cleanupIntervalMax = s.CleanupIntervalSecMax;

            if (cleanupIntervalMin > cleanupIntervalMax)
            {
                message += $"{nameof(cleanupIntervalMin)} > {nameof(cleanupIntervalMax)}\n";
            }

            doSaveBeforeCleanup = s.DoSaveOnCleanup;

            maxRestartsForReset = s.MaxRestartsForReset;

            orcBandOnSkipOnly = s.OrcbandOnSkipOnly;
            militaryFOnSkipOnly = s.MilitaryFOnSkipOnly;

            currentTriesToStartDungeon = 0;

            mimicCollectPercent = 0;
            if (s.CollectMimic)
            {
                mimicCollectPercent = s.CollectMimicChance;
            }

            gcLoadingLimit = s.GcLoadingLimit;
            if (gcLoadingLimit < 20_000)
            {
                message += $"{nameof(gcLoadingLimit)} must be 20s or more\n";
            }
            fixedAdWait = s.FixedAdWait;

            randomizeClickSequence = s.RandomizeCastSequence;
            pressAllHeroesOnCast = s.PressAllHeroesOnCast;
            pressFixedPointInHero = s.PressFixedPointInHero;

            screwUpCast = s.ScrewUpCast;
            swapNeighbourHerosChance = Math.Max(Math.Min((double)s.SwapNeighbourHeroesChance / 1000, 100), 0);
            castHeroOnEndChance = Math.Max(Math.Min((double)s.CastHeroInEndChance / 1000, 100), 0);

            for (int i = 0; i < 15; i++)
            {
                heroPressCoords[i].x = Math.Max(0, Math.Min(100, s.HeroPressXPositions[i]));
                heroPressCoords[i].y = Math.Max(0, Math.Min(100, s.HeroPressYPositions[i]));
            }

            ignoreWaitsBetweenBattlesOnX3FromAd = s.IgnoreWaitsOnX3FromAd;

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
            deleteU = s.MatU;

            openDungeonClickDelayMin = s.OpenDungeonClickDelayMin;
            openDungeonClickDelayMax = s.OpenDungeonClickDelayMax;

            if (openDungeonClickDelayMin > openDungeonClickDelayMax)
            {
                message += $"{nameof(openDungeonClickDelayMin)} > {nameof(openDungeonClickDelayMax)}\n";
            }

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
            if (deckToPlay == 0)
            {
                message += "Wrong deck to play\n";
            }

            skipWaves = s.SkipWaves;
            isSkip = false;
            autobattleMode = s.ABMode;

            setExitAfterNextBattle = false;

            abTab = s.ABGabOrTab;

            infiniteAB = s.InfiniteAB;

            timeToBreakABMin = s.TimeToBreakABMin;
            timeToBreakABMax = s.TimeToBreakABMax;

            if (autobattleMode && !infiniteAB && timeToBreakABMin > timeToBreakABMax)
            {
                message += $"{nameof(timeToBreakABMin)} > {nameof(timeToBreakABMax)}\n";
            }

            tryToSkipEveryBattle = s.TryToSkipEveryBattle;

            secondsBetweenSkipsMin = s.TimeBetweenSkipsMin;
            secondsBetweenSkipsMax = s.TimeBetweenSkipsMax;

            if (autobattleMode && !tryToSkipEveryBattle && secondsBetweenSkipsMin > secondsBetweenSkipsMax)
            {
                message += $"{nameof(secondsBetweenSkipsMin)} > {nameof(secondsBetweenSkipsMax)}\n";
            }

            battlesWithSkipsMin = s.BattlesWithSkipsMin;
            battlesWithSkipsMax = s.BattlesWithSkipsMax;

            if (autobattleMode && !tryToSkipEveryBattle && battlesWithSkipsMin > battlesWithSkipsMax)
            {
                message += $"{nameof(battlesWithSkipsMin)} > {nameof(battlesWithSkipsMax)}\n";
            }

            makeReplays = s.MakeReplays;
            skipWithOranges = s.SkipWithOranges;

            waitForAd = 2;

            adForX3 = s.AdForSpeed;
            adForCoins = s.AdForCoins;
            adAfterSkipOnly = s.AdAfterSkipOnly;
            adDuringX3 = s.AdDuringX3;

            solveCaptcha = s.SolveCaptcha;
            solvingCaptcha = false;

            if (solveCaptcha)
            {
                try
                {
                    int ret = execute(new byte[10], 0, 0, 0, 0, false, false, out _, out int a, out double b, 1);

                    if (ret != 2 || a != 123)
                    {
                        message += "Error while calling gca_captcha_solver.dll\n";
                    }
                }
                catch (Exception e)
                {
                    string exeFolder = AppDomain.CurrentDomain.BaseDirectory;

                    string[] fileNames = { "gca_captcha_solver.dll", "opencv_world490.dll" };
                    bool foundMissing = false;
                    foreach (string name in fileNames)
                    {
                        string fullPath = Path.Combine(exeFolder, name);
                        if (!File.Exists(fullPath))
                        {
                            message += $"\"{name}\" file is missing! It must be together with gca.exe in App folder!\n";
                            foundMissing = true;
                        }
                    }

                    if (!foundMissing)
                    {
                        message += "Error while calling gca_captcha_solver.dll: " + e.Message + "\n\n";

                        message += """
                                      If you see "Couldn't resolve one of dependencies issue", then check this:

                                      1) Check if you have "Microsoft Visual C++ 2015-2022 Redistributable (64x)" installed on your computer.
                                         Press Win+R, then type "appwiz.cpl", and press Enter. It will open "Programs and components" window in control panel.
                                         There try to find "Microsoft Visual C++ 2015-2022 Redistributable (64x)". If you don't have it - download it from Microsoft site (find in internet or in instruction)
                                         After it, it should work.
                                      2) If you have it, and you still see this message, then ensure that "gca.exe", "gca_captcha_solver.dll" and "opencv_world490.dll" are in "App" folder.
                                   """;
                    }
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

            waitOnBattleButtonsMin = s.WaitOnBattleButtonsMin;
            waitOnBattleButtonsMax = s.WaitOnBattleButtonsMax;

            if (autobattleMode && waitOnBattleButtonsMin > waitOnBattleButtonsMax)
            {
                message += $"{nameof(waitOnBattleButtonsMin)} > {nameof(waitOnBattleButtonsMax)}\n";
            }

            waitBetweenBattlesRuntimes = new(s.WaitBetweenBattlesSettings.Count);

            foreach (var wbbuc in waitBetweenBattlesUserControls)
            {
                if (!wbbuc.IsChecked)
                {
                    continue;
                }
                try
                {
                    WaitBetweenBattlesRuntime wbbr = new(wbbuc.GetSetting(out string msg));
                    if(msg.Length > 0)
                    {
                        message += $"{msg}\n";
                    }
                    else
                    {
                        waitBetweenBattlesRuntimes.Add(wbbr);
                    }
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

            screenshotLDPlayerLoadFail = s.ScreenshotLDPlayerLoadFail;

            screenshotPopups = s.ScreenshotPopups;
            lastPopupScreenshot = DateTime.MinValue;

            saveScreenshotsOnError = s.SaveScreenshotsCacheOnError;

            cacheDurationSec = s.CacheDurationSeconds;
            cacheIntervalMs = s.CacheIntervalMs;
            cacheImageQuality = s.CacheImageQuality;
            if (cacheImageQuality < 10)
            {
                message += "Set cache image quality to 10 or more\n";
            }
            if (cacheDurationSec < 20)
            {
                message += "Set cache duration to 20 or more\n";
            }

            stopAfterNKills = s.StopAfterNKills;
            leftDungeonKills = s.LeftDungeonKills;

            if(dungeonFarmGlobal && stopAfterNKills && leftDungeonKills <= 0)
            {
                message += $"{nameof(leftDungeonKills)} = 0!\n";
            }

            replaysIfDungeonDontLoad = s.MakeReplaysIfDungeonDontLoad;

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
            log30DetectionsInNotificationMode = s.Log30CrystalsDetection;

            DisableIncompatibleSettings();

            if (build == null)
            {
                message += "Wrong build to play!\n";
                return false;
            }

            BuildSettings buildSettings = build.GetBuildSettings();

            for (int i = 0; i < 15; i++)
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

            if(buildSettings.PressOrder.Length != 15)
            {
                message += "Order of cast is corrupted. Update settings!\n";
                return false;
            }

            heroPressOrder = buildSettings.PressOrder.Select((pos, idx) => (pos, idx)).OrderBy(x => x.pos).Select(x => x.idx).ToArray();

            thisPureSlot = buildSettings.PwSlot;
            thisSmithSlot = buildSettings.SmithSlot;
            thisChronoSlot = buildSettings.ChronoSlot;
            thisOrcBandSlot = buildSettings.OrcBandSlot;
            thisMilitaryFSlot = buildSettings.MiliitaryFSlot;
            thisChronoSlot = buildSettings.ChronoSlot;

            if (!InitHerosPositions(out string m))
            {
                message += '\n' + m;
            }

            testMouseMoveX1 = s.TestMouseMovementX1;
            testMouseMoveX2 = s.TestMouseMovementX2;
            testMouseMoveY1 = s.TestMouseMovementY1;
            testMouseMoveY2 = s.TestMouseMovementY2;

            countCrystalsTestDarkMode = s.TestCrystalsCountDarkMode;

            onlineActionsTest_OpenGuildTest = s.OnlineActionsTest_OpenGuildTest;
            onlineActionsTest_OpenRandomProfileFromGuildTest = s.OnlineActionsTest_OpenRandomProfileFromGuildTest;
            onlineActionsTest_OpenGuildsTopTest = s.OnlineActionsTest_OpenGuildsTopTest;
            onlineActionsTest_OpenTopTest = s.OnlineActionsTest_OpenTopTest;
            onlineActionsTest_OpenTopSeasonTest = s.OnlineActionsTest_OpenTopSeasonTest;
            onlineActionsTest_OpenHellSeasonMyTest = s.OnlineActionsTest_OpenHellSeasonMyTest;
            onlineActionsTest_OpenHellSeasonTest = s.OnlineActionsTest_OpenHellSeasonTest;
            onlineActionsTest_OpenWavesTopMyTest = s.OnlineActionsTest_OpenWavesTopMyTest;
            onlineActionsTest_OpenWavesTopTest = s.OnlineActionsTest_OpenWavesTopTest;
            onlineActionsTest_PressDeckTest = s.OnlineActionsTest_PressDeck;
            onlineActionsTest_CraftStonesTest = s.OnlineActionsTest_CraftStonesTest;
            onlineActionsTest_DoSaveTest = s.OnlineActionsTest_DoSaveTest;

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = false,
            };

            if(scopy.WaitBetweenBattlesSettings.Count > 3)
            {
                scopy.WaitBetweenBattlesSettings = scopy.WaitBetweenBattlesSettings[..3];
            }

            string json = JsonSerializer.Serialize(scopy, options);
            SettingsString = json;

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
            if (thisPureSlot < -1 || thisPureSlot > 12)
            {
                message += "Pw wrong slot\n";
            }
            else if (thisPureSlot != -1 && (thisPureSlot - 1) % 3 != 0)
            {
                message += "Pw must be on center vertical\n";
            }
            else if(thisPureSlot != -1)
            {
                (pwX, pwY) = Cst.HerosBlueLinePositions[thisPureSlot];
                pwBounds = Cst.HerosBounds[thisPureSlot];
            }

            if (thisSmithSlot < -1 || thisSmithSlot > 12)
            {
                message += "Smith wrong slot\n";
            }
            else if (thisSmithSlot != -1)
            {
                (smithX, smithY) = Cst.HerosBlueLinePositions[thisSmithSlot];
                smithBounds = Cst.HerosBounds[thisSmithSlot];
            }

            if (thisChronoSlot < -1 || thisChronoSlot > 12)
            {
                message += "Chrono wrong slot\n";
            }
            else if (thisChronoSlot != -1)
            {
                (chronoX, chronoY) = Cst.HerosBlueLinePositions[thisChronoSlot];
                chronoBounds = Cst.HerosBounds[thisChronoSlot];
            }

            if (thisOrcBandSlot < -1 || thisOrcBandSlot > 12)
            {
                message += "Orc band wrong slot\n";
            }
            else if (thisOrcBandSlot != -1)
            {
                (orcBandX, orcBandY) = Cst.HerosBlueLinePositions[thisOrcBandSlot];
                orcBandBounds = Cst.HerosBounds[thisOrcBandSlot];
            }

            if (thisMilitaryFSlot < -1 || thisMilitaryFSlot > 12)
            {
                message += "Orc band wrong slot\n";
            }
            else if (thisMilitaryFSlot != -1)
            {
                (militX, militY) = Cst.HerosBlueLinePositions[thisMilitaryFSlot];
                militBounds = Cst.HerosBounds[thisMilitaryFSlot];
            }

            return message.Length == 0;
        }

    }

}
