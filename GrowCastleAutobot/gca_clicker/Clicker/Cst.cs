using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace gca_clicker.Clicker
{
    /// <summary>
    /// Constants
    /// </summary>
    public static class Cst
    {
#if DEBUG
        public const string APP_TITLE = "GCA DEBUG";
#else
        public const string APP_TITLE = "GCA";
#endif

        public const string SCREENSHOT_PATH = "../screens/";

        public const string SCREENSHOT_RUNES_PATH = SCREENSHOT_PATH + "Runes/Rune.png";

        public const string SCREENSHOT_ITEMS_PATH = SCREENSHOT_PATH + "Items/";

        public const string SCREENSHOT_ITEMS_B_PATH = SCREENSHOT_ITEMS_PATH + "B/Item_B.png";
        public const string SCREENSHOT_ITEMS_A_PATH = SCREENSHOT_ITEMS_PATH + "A/Item_A.png";
        public const string SCREENSHOT_ITEMS_S_PATH = SCREENSHOT_ITEMS_PATH + "S/Item_S.png";
        public const string SCREENSHOT_ITEMS_L_PATH = SCREENSHOT_ITEMS_PATH + "L/Item_L.png";
        public const string SCREENSHOT_ITEMS_E_PATH = SCREENSHOT_ITEMS_PATH + "E/Item_E.png";

        public const string SCREENSHOT_ERRORS_PATH = SCREENSHOT_PATH + "Errors/";

        public const string SCREENSHOT_LONG_GC_LOAD_PATH = SCREENSHOT_ERRORS_PATH + "LongGCLoad.png";
        public const string SCREENSHOT_NOX_LOAD_FAIL_PATH = SCREENSHOT_ERRORS_PATH + "NoxLoadFail.png";
        public const string SCREENSHOT_CLEARALL_FAIL_PATH = SCREENSHOT_ERRORS_PATH + "ClearAllFail.png";
        public const string SCREENSHOT_NOX_MAIN_MENU_LOAD_FAIL_PATH = SCREENSHOT_ERRORS_PATH + "NoxMainMenuLoadFail.png";
        public const string SCREENSHOT_AB_ERROR_PATH = SCREENSHOT_ERRORS_PATH + "AB_Error.png";
        public const string SCREENSHOT_AB_ERROR2_PATH = SCREENSHOT_ERRORS_PATH + "AB_Error2.png";
        public const string SCREENSHOT_ON_FREEZE_PATH = SCREENSHOT_ERRORS_PATH + "Freezing.png";
        public const string SCREENSHOT_ON_ESC_PATH = SCREENSHOT_ERRORS_PATH + "OnEsc.png";
        public const string SCREENSHOT_AFTER_10_ESC_PATH = SCREENSHOT_ERRORS_PATH + "unknown.png";
        public const string SCREENSHOT_LONG_WAVE_PATH = SCREENSHOT_ERRORS_PATH + "LongWave.png";
        public const string SCREENSHOT_CAPTCHA_ERRORS_PATH = SCREENSHOT_PATH + "Captchas/Errors/Error.png";

        public const string SCREENSHOT_ERROR_SCREEN_CACHE_PATH = SCREENSHOT_ERRORS_PATH + "Screen_caches/Screen_cache";

        public const string SCREENSHOT_HINT_PATH = SCREENSHOT_PATH + "Hint/Hint.png";
        public const string SCREENSHOT_POPUPS_PATH = SCREENSHOT_PATH + "Popups/Popup.png";

        public const string SCREENSHOT_TEST_PATH = SCREENSHOT_PATH + "Test/";

        public const string SCREENSHOT_TEST_WINDOW_PATH = SCREENSHOT_TEST_PATH + "WindowScreenshot.png";
        public const string SCREENSHOT_TEST_SCREEN_PATH = SCREENSHOT_TEST_PATH + "CompleteScreenshot.png";

        public const string SCREENSHOT_TEST_JPG_SCREEN_PATH = SCREENSHOT_TEST_PATH + "JpgScreenshot.jpg";

        public const string DUNGEON_STATISTICS_PATH = "../dungeon_statistics.txt";
        public const string TIMER_X3_FILE_PATH = "../timerx3spd.txt";

        public const string CURRENT_SETTINGS_FILE_PATH = "clickerSettings.json";
        public const string LOG_FILE_PATH = "../gc.log";
        public const string CAPTCHA_LOG_FILE_PATH = "../captcha.log";

        public const string DEFAULT_DUNGEON_STATISTICS = "black:\r\nB: 0\r\nA: 0\r\n\r\nred:\r\n0\r\n0\r\n0\r\n\r\nsin:\r\nB: 0\r\nA: 0\r\nS: 0\r\n\r\nleg:\r\n0\r\n0\r\n0\r\n\r\nbone:\r\nA: 0\r\nS: 0\r\nE: 0\r\n";

        public const string AUDIO_30_CRYSTALS_1_PATH = "Audio1";
        public const string AUDIO_30_CRYSTALS_2_PATH = "Audio2";

        public static System.Drawing.Color SkyColor => System.Drawing.Color.FromArgb(255, 231, 237, 246);
        public static System.Drawing.Color CastleUpgradeColor => System.Drawing.Color.FromArgb(255, 77, 173, 234);

        public static System.Drawing.Color BlueLineColor => System.Drawing.Color.FromArgb(255, 84, 188, 255);

        public static System.Drawing.Color White => System.Drawing.Color.FromArgb(255, 255, 255, 255);
        public static System.Drawing.Color Black => System.Drawing.Color.FromArgb(255, 0, 0, 0);

        public const int WINDOW_WIDTH = 1520;
        public const int WINDOW_HEIGHT = 865;

        public static readonly SolidColorBrush ErrorBackgrounColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 77, 77));

#if DEBUG
        public static readonly SolidColorBrush DefaultBackground = System.Windows.Media.Brushes.Gold;
        public static readonly SolidColorBrush RunningBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 255, 50));
        public static readonly SolidColorBrush PauseRequestedBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 50));
        public static readonly SolidColorBrush PausedBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 143, 64));
        public static readonly SolidColorBrush StopRequestedBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 50, 50));
        public static readonly SolidColorBrush WaitingBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(77, 121, 255));
        public static readonly SolidColorBrush NotificationOnlyModeBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(121, 77, 255));
#else
        public static readonly SolidColorBrush DefaultBackground = System.Windows.Media.Brushes.White;
        public static readonly SolidColorBrush RunningBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 255, 128));
        public static readonly SolidColorBrush PauseRequestedBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 191, 128));
        public static readonly SolidColorBrush PausedBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 128));
        public static readonly SolidColorBrush StopRequestedBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 128, 128));
        public static readonly SolidColorBrush WaitingBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 159, 255));
        public static readonly SolidColorBrush NotificationOnlyModeBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(121, 77, 255));
#endif


    }
}
