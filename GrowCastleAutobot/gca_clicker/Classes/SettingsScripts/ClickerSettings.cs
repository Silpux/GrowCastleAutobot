using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Classes.SettingsScripts
{
    public class ClickerSettings
    {
        public string WindowName { get; set; } = "NoxPlayer";

        public int BuildToPlayIndex { get; set; } = 0;

        public string StartShortcut { get; set; } = "Alt+F1";
        public string StopShortcut { get; set; } = "Alt+F2";

        public bool FarmDungeon { get; set; }

        public int DungeonIndex { get; set; } = 0;
        public bool MatB { get; set; }
        public bool MatA { get; set; }
        public bool MatS { get; set; }
        public bool MatL { get; set; }
        public bool MatE { get; set; }

        public int MatGetDelayMin { get; set; } = 250;
        public int MatGetDelayMax { get; set; } = 500;

        public bool CastOnBossInDungeon { get; set; }
        public int CastOnBossInDungeonDelay { get; set; } = 500;

        public bool MakeReplaysIfDungeonDontLoad { get; set; }

        public bool MissclickOnDungeons { get; set; }
        public bool MissclickOnDungeonsIncludeDiagonals { get; set; }
        public int MissclickOnDungeonsChance { get; set; }

        public bool MakeReplays { get; set; }
        public bool SkipWaves { get; set; }
        public bool SkipWithOranges { get; set; }

        public bool ABMode { get; set; }
        public bool ABGabOrTab { get; set; }

        public bool ABWaveCanceling { get; set; }

        public bool BreakAbOn30Crystals { get; set; }


        public int TimeToBreakABMin { get; set; } = 600;
        public int TimeToBreakABMax { get; set; } = 900;
        public int SkipsBetweenABSessionsMin { get; set; } = 3;
        public int SkipsBetweenABSessionsMax { get; set; } = 5;


        public bool DesktopNotificationOn30Crystals { get; set; }
        public int DesktopNotificationOn30CrystalsInterval { get; set; } = 30;

        public bool PlayAudioOn30Crystals { get; set; }
        public int PlayAudioOn30CrystalsInterval { get; set; } = 60;
        public int PlayAudio1On30CrystalsVolume { get; set; } = 50;
        public int PlayAudio2On30CrystalsVolume { get; set; } = 50;

        public int Audio30CrystalsIndex { get; set; }

        public bool NotificationOnlyMode { get; set; }
        public bool Log30CrystalsDetection { get; set; }


        public bool BackgroundMode { get; set; }
        public bool SimulateMouseMovement { get; set; }
        public bool RandomizeCastSequence { get; set; } = true;

        public bool MonitorFreezing { get; set; }

        public int HeroClickWaitMin { get; set; } = 50;
        public int HeroClickWaitMax { get; set; } = 150;

        public int WaitBetweenCastsMin { get; set; } = 100;
        public int WaitBetweenCastsMax { get; set; } = 300;

        public int WaitOnBattleButtonsMin { get; set; } = 150;
        public int WaitOnBattleButtonsMax { get; set; } = 400;

        public bool SolveCaptcha { get; set; }
        public bool RestartOnCaptcha { get; set; }

        public bool UpgradeCastle { get; set; }

        public int FloorToUpgradeCastle { get; set; } = 0;

        public bool UpgradeHero { get; set; }

        public int SlotToUpgradeHero { get; set; } = 0;

        public bool AdForSpeed { get; set; }
        public bool AdForCoins { get; set; }

        public bool AdDuringX3 { get; set; }

        public bool AdAfterSkipOnly { get; set; }

        public bool HealAltar { get; set; }

        public bool DeathAltar { get; set; }

        public int MaxBattleLengthMs { get; set; } = 120_000;
        public int CleanupIntervalSecMin { get; set; } = 7_200;
        public int CleanupIntervalSecMax { get; set; } = 14_400;
        public bool DoResetOnCleanup { get; set; }
        public bool DoSaveOnCleanup { get; set; }
        public bool DisableResetCleanupCheck { get; set; }

        public bool DoRestarts { get; set; }
        public int RestartsIntervalMin { get; set; } = 3600;
        public int RestartsIntervalMax { get; set; } = 4800;

        public int MaxRestartsForReset { get; set; } = 4;

        public bool OrcbandOnSkipOnly { get; set; }
        public bool MilitaryFOnSkipOnly { get; set; }

        public bool IHaveX3 { get; set; }

        public bool CollectMimic { get; set; }
        public int CollectMimicChance { get; set; } = 100;

        public int GcLoadingLimit { get; set; } = 30_000;
        public int FixedAdWait { get; set; }

        public bool SpeedupOnItemDrop { get; set; } = true;


        public bool IgnoreWaitsOnABMode { get; set; }
        public List<WaitBetweenBattlesSetting> WaitBetweenBattlesSettings { get; set; } = new();


        public bool PwOnBoss { get; set; }

        public int PwOnBossDelay { get; set; } = 500;

        public bool ScreenshotItems { get; set; } = true;

        public bool ScreenshotRunes { get; set; } = true;

        public bool ScreenshotPopups { get; set; } = true;

        public bool ScreenshotSolvedCaptchas { get; set; }

        public bool ScreenshotFailedCaptchas { get; set; } = true;
        public bool ScreenshotCaptchaErrors { get; set; }
        public bool ScreenshotOnEsc { get; set; }
        public bool ScreenshotABErrors { get; set; }
        public bool ScreenshotOnFreezing { get; set; }

        public bool ScreenshotLongLoad { get; set; }

        public bool ScreenshotLongWave { get; set; }
        public bool ScreenshotAfter10Esc { get; set; }
        public bool ScreenshotNoxLoadFail { get; set; }
        public bool ScreenshotNoxMainMenuLoadFail { get; set; }
        public bool ScreenshotClearAllFail { get; set; }

        public bool SaveScreenshotsCacheOnError { get; set; }
        public int CacheDurationSeconds { get; set; } = 60;
        public int CacheIntervalMs { get; set; } = 200;
        public int CacheImageQuality { get; set; } = 20;

        public BuildSettings[] Build { get; set; } = { new(), new(), new(), new(), new() };


    }
}
