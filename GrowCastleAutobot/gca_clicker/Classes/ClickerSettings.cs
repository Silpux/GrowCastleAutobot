using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Classes
{
    public class ClickerSettings
    {
        public string WindowName { get; set; }
        public int BuildToPlayIndex { get; set; }
        public bool FarmDungeon { get; set; }
        public int DungeonIndex { get; set; }
        public bool MatB { get; set; }
        public bool MatA { get; set; }
        public bool MatS { get; set; }
        public bool MatL { get; set; }
        public bool MatE { get; set; }

        public bool CastOnBossInDungeon { get; set; }
        public int CastOnBossDelay { get; set; }

        public bool MakeReplaysIfDungeonDontLoad { get; set; }

        public bool MakeReplays { get; set; }
        public bool SkipWaves { get; set; }
        public bool FiveWavesBetweenSpiks { get; set; }
        public bool SkipWithOranges { get; set; }

        public bool ABMode { get; set; }
        public bool ABGabOrTab{ get; set; }

        public bool ABWaveCanceling { get; set; }

        public bool BreakAbOn30Crystals { get; set; }

        public int TimeToBreakAB { get; set; }
        public int SkipsBetweenABSessions { get; set; }

        public bool BackgroundMode { get; set; }

        public bool SolveCaptcha { get; set; }
        public bool RestartOnCaptcha { get; set; }

        public bool UpgradeCastle { get; set; }

        public int FloorToUpgradeCastle { get; set; }

        public bool UpgradeHero { get; set; }

        public int SlotToUpgradeHero { get; set; }

        public bool AdForSpeed { get; set; }
        public bool AdForCoins { get; set; }

        public bool AdDuringX3 { get; set; }

        public bool AdAfterSkipOnly { get; set; }

        public bool HealAltar { get; set; }

        public bool DeathAltar { get; set; }

        public bool PwOnBoss { get; set; }

        public int PwOnBossDelay { get; set; }

        public bool ScreenshotItems { get; set; }

        public bool ScreenshotRunes { get; set; }

        public bool ScreenshotSolvedCaptchas { get; set; }

        public bool ScreenshotFailedCaptchas { get; set; }
        public bool ScreenshotOnEsc { get; set; }

        public bool ScreenshotLongLoad { get; set; }

        public bool ScreenshotLongWave { get; set; }
        public bool ScreenshotAfter10Esc { get; set; }
        public bool ScreenshotNoxLoadFail { get; set; }
        public bool ScreenshotNoxMainMenuLoadFail { get; set; }
        public bool ScreenshotClearAllFail { get; set; }

        public BuildSettings[] Build { get; set; }

    }
}
