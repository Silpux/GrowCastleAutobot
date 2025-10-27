using gca.Structs;
using System.Windows.Media;

namespace gca.Script
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
        public const string CRYSTALS_COLLECTED_TIME_FILE_PATH = "../30crystalsTime.log";
        public const string MANUAL_FILE_PATH = "../gca_guide.pdf";

        public const string DEFAULT_DUNGEON_STATISTICS = "black:\r\nB: 0\r\nA: 0\r\n\r\nred:\r\n0\r\n0\r\n0\r\n\r\nsin:\r\nB: 0\r\nA: 0\r\nS: 0\r\n\r\nleg:\r\n0\r\n0\r\n0\r\n\r\nbone:\r\nA: 0\r\nS: 0\r\nE: 0\r\n";

        public const string AUDIO_30_CRYSTALS_1_PATH = "Audio1";
        public const string AUDIO_30_CRYSTALS_2_PATH = "Audio2";

        public const int STEP_BACK_FOR_PAUSE_WAIT = 1250;
        public const int MAX_WAIT_FOR_PAUSE_ON_STEPPING_BACK = 5000;

        public const int WAIT_START_TIMEOUT = 10_000;
        public const int WAIT_FOR_NEXT_WAVE_TIMEOUT = 120_000;
        public const int WAIT_FOR_END_OF_WAVE_TIMEOUT = 120_000;

        public static readonly Bounds ItemBounds = new(401, 75, 1192, 703);
        public static readonly Bounds GetButtonBounds = new(335, 188, 1140, 700);
        public static readonly Bounds CrystalPriceBounds = new(958, 586, 1126, 621);
        public static readonly Bounds AltarBounds = new(116, 215, 172, 294);
        public static readonly Bounds MimicBounds = new(437, 794, 1339, 829);
        public static readonly Bounds RuneBounds = new(429, 340, 1080, 740);

        public static readonly Bounds Skip10ButtonBounds = new(498, 419, 587, 494);
        public static readonly Bounds Skip20ButtonBounds = new(698, 414, 783, 491);
        public static readonly Bounds Skip30ButtonBounds = new(889, 411, 984, 496);

        public static readonly Bounds PausePanelExitButtonBounds = new(787, 477, 1048, 539);
        public static readonly Bounds PausePanelContinueButtonBounds = new(477, 487, 689, 535);
        public static readonly Bounds ExitPanelContinueButtonBounds = new(477, 487, 689, 535);
        public static readonly Bounds ExitAfterBattleButtonBounds = new(531, 623, 962, 668);

        public static readonly Bounds DungeonsButtonBounds = new(699, 280, 752, 323);
        
        public static readonly Bounds GreenDradonButtonBounds = new(57, 168, 371, 218);
        public static readonly Bounds BlackDradonButtonBounds = new(539, 170, 903, 227);
        public static readonly Bounds RedDradonButtonBounds = new(1082, 166, 1368, 212);
        public static readonly Bounds SinButtonBounds = new(57, 308, 302, 366);
        public static readonly Bounds LegendaryDragonButtonBounds = new(544, 304, 891, 365);
        public static readonly Bounds BoneDradonButtonBounds = new(1094, 301, 1367, 367);
        public static readonly Bounds BeginnerDungeonButtonBounds = new(160, 443, 414, 483);
        public static readonly Bounds IntermediateDungeonButtonBounds = new(625, 444, 879, 485);
        public static readonly Bounds ExpertDungeonButtonBounds = new(1113, 438, 1361, 486);

        public static readonly Bounds BattleDungeonButtonBounds = new(1039, 728, 1141, 770);

        public static readonly Tupfel WaveWhitePxlCoords = new(805, 96);

        public static readonly Tupfel[] HerosBlueLinePositions =
        [
            new(360, 88),
            new(456, 92),
            new(547, 91),
            new(364, 202),
            new(455, 202),
            new(549, 201),
            new(362, 311),
            new(455, 310),
            new(547, 311),
            new(362, 414),
            new(456, 414),
            new(548, 415),
            new(271, 203),
            new(183, 452),
            new(182, 587),
        ];

        public static Bounds UpgradeForCrystalsButtonBounds = new(958, 554, 1108, 606);

        public static readonly Bounds[] HerosBounds =
        [
            new(322, 110, 363, 165),
            new(418, 110, 455, 165),
            new(498, 110, 546, 165),
            new(322, 203, 363, 276),
            new(418, 203, 455, 276),
            new(498, 203, 546, 276),
            new(322, 311, 363, 387),
            new(418, 311, 455, 387),
            new(498, 311, 546, 387),
            new(322, 414, 363, 492),
            new(418, 414, 455, 492),
            new(498, 414, 546, 492),
            new(218, 197, 267, 266),
            new(96, 476, 235, 546),
            new(90, 597, 232, 667),
        ];

        public static readonly Bounds[] CaptchaBoxesBounds =
        [
            new (719, 278, 762, 338),
            new (823, 312, 866, 373),
            new (857, 414, 899, 477),
            new (820, 517, 864, 584),
            new (719, 551, 760, 618),
            new (615, 516, 661, 582),
            new (582, 411, 625, 478),
            new (616, 309, 660, 372),
        ];

        public static readonly System.Drawing.Color SkyColor = System.Drawing.Color.FromArgb(231, 237, 246);
        public static readonly System.Drawing.Color CastleUpgradeColor = System.Drawing.Color.FromArgb(77, 173, 234);

        public static readonly System.Drawing.Color BlueLineColor = System.Drawing.Color.FromArgb(84, 188, 255);

        public static readonly System.Drawing.Color White = System.Drawing.Color.FromArgb(255, 255, 255);
        public static readonly System.Drawing.Color Black = System.Drawing.Color.FromArgb(0, 0, 0);

        public static readonly System.Drawing.Color LightCrystalColor = White;
        public static readonly System.Drawing.Color DimmedCrystalColor = System.Drawing.Color.FromArgb(89, 89, 89);
        public static readonly System.Drawing.Color CrystalPriceColor = System.Drawing.Color.FromArgb(0, 221, 255);


        public static readonly System.Drawing.Color BStoneColor = System.Drawing.Color.FromArgb(134, 163, 166);
        public static readonly System.Drawing.Color AStoneColor = System.Drawing.Color.FromArgb(24, 205, 235);
        public static readonly System.Drawing.Color SStoneColor = System.Drawing.Color.FromArgb(237, 14, 212);
        public static readonly System.Drawing.Color LStoneColor = System.Drawing.Color.FromArgb(227, 40, 44);

        public static readonly System.Drawing.Color BWordColor = System.Drawing.Color.FromArgb(218, 218, 218);
        public static readonly System.Drawing.Color AWordColor = System.Drawing.Color.FromArgb(68, 255, 218);
        public static readonly System.Drawing.Color SWordColor = System.Drawing.Color.FromArgb(244, 86, 233);
        public static readonly System.Drawing.Color LWordColor = System.Drawing.Color.FromArgb(255, 50, 50);
        public static readonly System.Drawing.Color EWordColor = System.Drawing.Color.FromArgb(255, 216, 0);



        public static readonly System.Drawing.Color GET_BUTTON_COLOR = System.Drawing.Color.FromArgb(239, 209, 104);


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
