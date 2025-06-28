using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Classes
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

        public bool CastOnBossInDungeon { get; set; }
        public int CastOnBossInDungeonDelay { get; set; } = 200;

        public bool MakeReplaysIfDungeonDontLoad { get; set; }

        public bool MakeReplays { get; set; }
        public bool SkipWaves { get; set; }
        public bool FiveWavesBetweenSpiks { get; set; }
        public bool SkipWithOranges { get; set; }

        public bool ABMode { get; set; }
        public bool ABGabOrTab{ get; set; }

        public bool ABWaveCanceling { get; set; }

        public bool BreakAbOn30Crystals { get; set; }

        public int TimeToBreakAB { get; set; } = 600;
        public int SkipsBetweenABSessions { get; set; } = 3;

        public bool BackgroundMode { get; set; }

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

        public bool PwOnBoss { get; set; }

        public int PwOnBossDelay { get; set; } = 500;

        public bool ScreenshotItems { get; set; } = true;

        public bool ScreenshotRunes { get; set; } = true;

        public bool ScreenshotSolvedCaptchas { get; set; }

        public bool ScreenshotFailedCaptchas { get; set; } = true;
        public bool ScreenshotCaptchaErrors { get; set; }
        public bool ScreenshotOnEsc { get; set; }

        public bool ScreenshotLongLoad { get; set; }

        public bool ScreenshotLongWave { get; set; }
        public bool ScreenshotAfter10Esc { get; set; }
        public bool ScreenshotNoxLoadFail { get; set; }
        public bool ScreenshotNoxMainMenuLoadFail { get; set; }
        public bool ScreenshotClearAllFail { get; set; }

        public BuildSettings[] Build { get; set; } = { new(), new(), new(), new(), new() };

    }
}
