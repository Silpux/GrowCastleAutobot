using gca.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace gca
{
    public partial class Autobot
    {

        public const bool CAPTCHA_TEST_MODE = false;

        [DllImport("gca_captcha_solver.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int execute(byte[] data, int width, int height, int channels, int count, bool saveScreenshots, bool failMode, out int trackedNumber, out int ans, out double ratio0_1, int testVal);

        private ScreenshotCache screenshotCache = new();

        private Random rand = new Random();

        private int coordNotTakenCounter;

        public event Action<string>? OnInitFailed;

        public event Action<bool>? OnStarted;
        public event Action? OnPauseRequested;
        public event Action? OnPaused;
        public event Action? OnStopRequested;
        public event Action? OnStopped;
        public event Action<string, string, ToolTipIcon>? OnShowBalloon;

        public event Action<string, double>? OnPlayAudio;

        public event Action? OnSwitchFromReplaysToDungeons;
        public event Action? OnSwitchFromDungeonsToReplays;

        public event Action? OnScriptError;

        public event Action<string>? OnShowCrystalsCountResultLabel;

        public event Action<DateTime>? OnShowNextRestartLabel;
        public event Action<DateTime>? OnShowNextCleanupLabel;

        public event Action? OnDisableTowerUpgrade;
        public event Action? OnDisableHeroUpgrade;

        public event Action<string>? OnABLabelUpdate;

        public event Action<string>? OnDungeonKillSpeedUpdate;

        public event Action? OnDisableSkipWithOranges;

        public event Action? OnDisableAdForSpeed;

        public event Action<SolidColorBrush>? OnChangeBackground;

        public event Action<string>? OnInfoLabelUpdate;

        public event Action<string>? OnCrystalsCountTestLabelUpdate;

        public event Action<string>? OnRestartTestLabelUpdate;
        public event Action<string>? OnUpgradeTestLabelUpdate;

        public event Action<string>? OnOnlineActionsTestLabelUpdate;
        public event Action<string>? OnCaptchaTestLabelUpdate;
        public event Action<string>? OnGameStatusLabelUpdate;


        private void ShowBalloon(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            OnShowBalloon?.Invoke(title, message, icon);
        }
    }
}
