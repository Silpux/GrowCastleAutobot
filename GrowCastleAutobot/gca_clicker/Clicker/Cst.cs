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

        public const string APP_TITLE = "GCA";

        public const string SCREENSHOT_RUNES_PATH = "../screens/Runes/Rune.png";

        public const string SCREENSHOT_ITEMS_B_PATH = "../screens/Items/B/Item_B.png";
        public const string SCREENSHOT_ITEMS_A_PATH = "../screens/Items/A/Item_A.png";
        public const string SCREENSHOT_ITEMS_S_PATH = "../screens/Items/S/Item_S.png";
        public const string SCREENSHOT_ITEMS_L_PATH = "../screens/Items/L/Item_L.png";
        public const string SCREENSHOT_ITEMS_E_PATH = "../screens/Items/E/Item_E.png";

        public const string SCREENSHOT_LONG_GC_LOAD_PATH = "../screens/Errors/LongGCLoad.png";
        public const string SCREENSHOT_NOX_LOAD_FAIL_PATH = "../screens/Errors/NoxLoadFail.png";
        public const string SCREENSHOT_CLEARALL_FAIL_PATH = "../screens/Errors/ClearAllFail.png";
        public const string SCREENSHOT_NOX_MAIN_MENU_LOAD_FAIL_PATH = "../screens/Errors/NoxMainMenuLoadFail.png";
        public const string SCREENSHOT_AB_ERROR_PATH = "../screens/Errors/AB_Error.png";
        public const string SCREENSHOT_AB_ERROR2_PATH = "../screens/Errors/AB_Error2.png";
        public const string SCREENSHOT_ON_ESC_PATH = "../screens/Errors/OnEsc.png";
        public const string SCREENSHOT_AFTER_10_ESC_PATH = "../screens/Errors/unknown.png";
        public const string SCREENSHOT_LONG_WAVE_PATH = "../screens/Errors/LongWave.png";
        public const string SCREENSHOT_CAPTCHA_ERRORS_PATH = "../screens/Captchas/Errors/Error.png";

        public const string SCREENSHOT_HINT_PATH = "../screens/Hint/Hint.png";

        public const string DUNGEON_STATISTICS_PATH = "../dungeon_statistics.txt";
        public const string TIMER_X3_FILE_PATH = "../timerx3spd.txt";

        public const string CURRENT_SETTINGS_FILE_PATH = "clickerSettings.json";
        public const string LOG_FILE_PATH = "../gc.log";
        public const string CAPTCHA_LOG_FILE_PATH = "../captcha.log";

        public const string DEFAULT_DUNGEON_STATISTICS = "black:\r\nB: 0\r\nA: 0\r\n\r\nred:\r\n0\r\n0\r\n0\r\n\r\nsin:\r\nB: 0\r\nA: 0\r\nS: 0\r\n\r\nleg:\r\n0\r\n0\r\n0\r\n\r\nbone:\r\nA: 0\r\nS: 0\r\nE: 0\r\n";

        public static System.Drawing.Color SkyColor => System.Drawing.Color.FromArgb(255, 231, 237, 246);
        public static System.Drawing.Color CastleUpgradeColor => System.Drawing.Color.FromArgb(255, 77, 173, 234);

        public static System.Drawing.Color BlueLineColor => System.Drawing.Color.FromArgb(255, 84, 188, 255);

        public static System.Drawing.Color White => System.Drawing.Color.FromArgb(255, 255, 255, 255);
        public static System.Drawing.Color Black => System.Drawing.Color.FromArgb(255, 0, 0, 0);

        public const int WINDOW_WIDTH = 1520;
        public const int WINDOW_HEIGHT = 865;

#if DEBUG
        public static readonly SolidColorBrush DefaultBackground = System.Windows.Media.Brushes.Gold;
        public static readonly SolidColorBrush RunningBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(74, 255, 74));
#else
        public static readonly SolidColorBrush DefaultBackground = System.Windows.Media.Brushes.White;
        public static readonly SolidColorBrush RunningBackground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(179, 255, 179));
#endif


    }
}
