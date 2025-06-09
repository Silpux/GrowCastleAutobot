using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Clicker
{
    public static class Cst
    {

        public const string SCREENSHOT_RUNES_PATH = "../screens/Runes/Rune.png";

        public const string SCREENSHOT_ITEMS_B_PATH = "../screens/Items/B/Item_B.png";
        public const string SCREENSHOT_ITEMS_A_PATH = "../screens/Items/B/Item_A.png";
        public const string SCREENSHOT_ITEMS_S_PATH = "../screens/Items/B/Item_S.png";
        public const string SCREENSHOT_ITEMS_L_PATH = "../screens/Items/B/Item_L.png";
        public const string SCREENSHOT_ITEMS_E_PATH = "../screens/Items/B/Item_E.png";

        public const string SCREENSHOT_LONG_GC_LOAD_PATH = "../screens/Errors/LongGCLoad.png";
        public const string SCREENSHOT_NOX_LOAD_FAIL_PATH = "../screens/Errors/NoxLoadFail.png";
        public const string SCREENSHOT_CLEARALL_FAIL_PATH = "../screens/Errors/ClearAllFail.png";
        public const string SCREENSHOT_NOX_MAIN_MENU_LOAD_FAIL_PATH = "../screens/Errors/NoxMainMenuLoadFail.png";
        public const string SCREENSHOT_AB_ERROR_PATH = "../screens/Errors/AB_Error.png";
        public const string SCREENSHOT_AB_ERROR2_PATH = "../screens/Errors/AB_Error2.png";

        public const string SCREENSHOT_HINT_PATH = "../screens/Hint/Hint.png";

        public const string DUNGEON_STATISTICS_PATH = "../dungeon_statistics.txt";

        public static Color SkyColor => System.Drawing.Color.FromArgb(255, 231, 237, 246);
        public static Color CastleUpgradeColor => System.Drawing.Color.FromArgb(255, 77, 173, 234);

        public static Color BlueLineColor => System.Drawing.Color.FromArgb(255, 84, 188, 255);

        public static Color White => System.Drawing.Color.FromArgb(255, 255, 255, 255);
        public static Color Black => System.Drawing.Color.FromArgb(255, 0, 0, 0);

        public const int WINDOW_WIDTH = 1520;
        public const int WINDOW_HEIGHT = 865;


    }
}
