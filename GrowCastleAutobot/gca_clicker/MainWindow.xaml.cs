using gca_clicker.Classes;
using gca_clicker.Clicker;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace gca_clicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public const bool CAPTCHA_TEST_MODE = false;

        [DllImport("gca_captcha_solver.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int execute(byte[] data, int width, int height, int channels, int count, bool saveScreenshots, bool failMode, out int trackedNumber, out int ans, out double ratio0_1, int testVal);

        private bool openToRewrite;

        private Random rand = new Random();

        private List<int[]> castPatterns = new List<int[]>()
        {
            // snake
            new int[]{0,1,2,5,4,3,12,6,7,8,11,10,9,13,14},
            new int[]{0,1,2,12,3,4,5,8,7,6,9,10,11,13,14},
            new int[]{14,13,9,10,11,8,7,6,5,4,3,12,0,1,2},
            new int[]{11,8,5,2,1,4,7,10,9,6,3,0,12,13,14},
            new int[]{9,10,11,8,7,6,3,4,5,2,1,0,12,13,14},
            new int[]{0,3,6,9,10,7,4,1,2,5,8,11,14,13,12},

            new int[]{0,3,4,1,2,5,8,11,10,7,6,9,14,13,12},

            // spiral
            new int[]{0,1,2,5,8,11,10,9,6,12,3,4,7,13,14},
            new int[]{2,1,0,12,3,6,9,13,14,10,11,8,5,4,7},
        };



        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closed += OnClosed;

            B1.OnUpdate += RewriteCurrentSettings;
            B2.OnUpdate += RewriteCurrentSettings;
            B3.OnUpdate += RewriteCurrentSettings;
            B4.OnUpdate += RewriteCurrentSettings;
            B5.OnUpdate += RewriteCurrentSettings;

            ApplyCurrentSettings();

            Log.I("App started");

            openToRewrite = true;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.WindowTop >= 0 && Settings.Default.WindowLeft >= 0)
            {
                this.Top = Settings.Default.WindowTop;
                this.Left = Settings.Default.WindowLeft;
            }

            var helper = new WindowInteropHelper(this);
            windowHandle = helper.Handle;
            source = HwndSource.FromHwnd(windowHandle);
            source.AddHook(HwndHook);

            try
            {
                SaveShortcut(StartClickerShortcutBox.Text, HOTKEY_START_ID);
            }
            catch
            {
                StartClickerShortcutBox.Text = DEFAULT_START_HOTKEY;
                SaveShortcut(DEFAULT_START_HOTKEY, HOTKEY_START_ID);
            }
            try
            {
                SaveShortcut(StopClickerShortcutBox.Text, HOTKEY_STOP_ID);
            }
            catch
            {
                StopClickerShortcutBox.Text = DEFAULT_STOP_HOTKEY;
                SaveShortcut(DEFAULT_STOP_HOTKEY, HOTKEY_STOP_ID);
            }

            //WinAPI.RegisterHotKey(helper.Handle, HOTKEY_START_ID, WinAPI.MOD_ALT, (uint)KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.F1));
            //WinAPI.RegisterHotKey(helper.Handle, HOTKEY_STOP_ID, WinAPI.MOD_ALT, (uint)KeyInterop.VirtualKeyFromKey(System.Windows.Input.Key.F2));

        }

        private void OnClosed(object sender, EventArgs e)
        {
            OnStopHotkey();
            source.RemoveHook(HwndHook);
            WinAPI.UnregisterHotKey(source.Handle, HOTKEY_START_ID);

            Settings.Default.WindowTop = this.Top;
            Settings.Default.WindowLeft = this.Left;
            Settings.Default.Save();
        }

        public void RewriteCurrentSettings()
        {
            if (openToRewrite)
            {
                ClickerSettings settings = GetClickerSettings();
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                };
                string json = JsonSerializer.Serialize(settings, options);
                json = json.Replace("  ", "    ");
                File.WriteAllText(Cst.CURRENT_SETTINGS_FILE_PATH, json);
            }
        }

        public void ApplyCurrentSettings()
        {
            ClickerSettings settings = null!;
            try
            {
                string json = File.ReadAllText(Cst.CURRENT_SETTINGS_FILE_PATH);
                settings = JsonSerializer.Deserialize<ClickerSettings>(json);
            }
            catch
            {
                settings = new();
            }

            SetFromSettings(settings);
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            return int.TryParse(text, out _);
        }

        private void MyTabControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var tabPanel = FindVisualChild<TabPanel>(MyTabControl);
            if (tabPanel != null)
            {
                System.Windows.Point mousePos = e.GetPosition(tabPanel);
                Rect bounds = new Rect(0, 0, tabPanel.ActualWidth, tabPanel.ActualHeight);

                if (bounds.Contains(mousePos))
                {
                    int currentIndex = MyTabControl.SelectedIndex;
                    int totalTabs = MyTabControl.Items.Count;

                    if (e.Delta < 0)
                        currentIndex = (currentIndex + 1) % totalTabs;
                    else if (e.Delta > 0)
                        currentIndex = (currentIndex - 1 + totalTabs) % totalTabs;

                    MyTabControl.SelectedIndex = currentIndex;
                    e.Handled = true;
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t)
                {
                    return t;
                }

                T childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //thread.Start();

            InfoLabel.Content = "";
            IntPtr hwnd = WinAPI.FindWindow(null, WindowName.Text);
            if (hwnd != IntPtr.Zero)
            {

                if (int.TryParse(XCoord.Text, out int x) && int.TryParse(YCoord.Text, out int y))
                {
                    LeftClickBackground((nint)hwnd, x, y);
                }
                else
                {
                    InfoLabel.Content = "number parse error";
                }

            }
            else
            {
                InfoLabel.Content = "Cannot find window";
            }


        }

        private void TestButton(object sender, RoutedEventArgs e)
        {

            backgroundMode = false;
            nint hWnd = WndFind(WindowName.Text);

            if (hWnd != IntPtr.Zero)
            {

                //(int x, int y, int width, int height) info = GetWindowInfo(hWnd);

                //Debug.WriteLine(info);
                //Getscreen();
                //Debug.WriteLine(Pxl(714, 120));
                //WinAPI.RestoreWindow(hWnd);

                hwnd = hWnd;

                //InfoLabel.Content = "Crystals: " + CountCrystals(true);

                //System.Drawing.Color col = System.Drawing.Color.FromArgb(1, 1, 1, 1);

                //Debug.WriteLine(GetLine(Cst.DUNGEON_STATISTICS_PATH, 2));

                //InsertLine(Cst.DUNGEON_STATISTICS_PATH, 2, "444");

                //RemoveLine(Cst.DUNGEON_STATISTICS_PATH, 2);

                //ReplaceLine(Cst.DUNGEON_STATISTICS_PATH, 5, "123123");
                


            }



        }



        public ClickerSettings GetClickerSettings()
        {
            ClickerSettings s = new ClickerSettings();

            s.WindowName = WindowName.Text;
            s.BuildToPlayIndex = BuildToPlayComboBox.SelectedIndex;

            s.StartShortcut = StartClickerShortcutBox.Text;
            s.StopShortcut = StopClickerShortcutBox.Text;

            s.FarmDungeon = FarmDungeonCheckbox.IsChecked == true;
            s.DungeonIndex = DungeonComboBox.SelectedIndex;
            s.MatB = MatBCheckbox.IsChecked == true;
            s.MatA = MatACheckbox.IsChecked == true;
            s.MatS = MatSCheckbox.IsChecked == true;
            s.MatL = MatLCheckbox.IsChecked == true;
            s.MatE = MatECheckbox.IsChecked == true;

            s.CastOnBossInDungeon = CastOnBossCheckbox.IsChecked == true;
            try
            {
                s.CastOnBossInDungeonDelay = int.Parse(CastOnBossDelayTextBox.Text);
            }
            catch
            {
                s.CastOnBossInDungeonDelay = 0;
            }
            s.MakeReplaysIfDungeonDontLoad = MakeReplaysIfDungeonDoesntLoadCheckBox.IsChecked == true;

            s.MakeReplays = ReplaysCheckbox.IsChecked == true;

            s.SkipWaves = SkipWavesCheckbox.IsChecked == true;
            s.FiveWavesBetweenSpiks = FiveWavesBetweenSkipsCheckbox.IsChecked == true;

            s.SkipWithOranges = SkipWithOrangesCheckbox.IsChecked == true;

            s.ABMode = ABModeCheckbox.IsChecked == true;
            s.ABGabOrTab = TabRadioButton.IsChecked == true;

            s.ABWaveCanceling = ABWaveCancelingCheckbox.IsChecked == true;

            s.BreakAbOn30Crystals = BreakABOn30CrystalsCheckbox.IsChecked == true;

            try
            {
                s.TimeToBreakAB = int.Parse(TimeToBreakABTextBox.Text);
            }
            catch
            {
                s.TimeToBreakAB = 0;
            }

            try
            {
                s.SkipsBetweenABSessions = int.Parse(SkipsBetweenABSessionsTextBox.Text);
            }
            catch
            {
                s.SkipsBetweenABSessions = 0;
            }

            s.BackgroundMode = BackgroundModeCheckbox.IsChecked == true;

            s.SolveCaptcha = SolveCaptchaCheckbox.IsChecked == true;

            s.RestartOnCaptcha = RestartOnCaptchaCheckbox.IsChecked == true;

            s.UpgradeCastle = UpgradeCastleCheckbox.IsChecked == true;
            s.UpgradeHero = UpgradeHeroForCrystalsCheckbox.IsChecked == true;

            s.FloorToUpgradeCastle = FloorToUpgradeCastleComboBox.SelectedIndex;

            s.SlotToUpgradeHero = SlotToUpgradeHeroComboBox.SelectedIndex;

            s.AdForSpeed = AdForSpeedCheckbox.IsChecked == true;
            s.AdForCoins = AdForCoinsCheckbox.IsChecked == true;
            s.AdDuringX3 = AdDuringx3Checkbox.IsChecked == true;
            s.AdAfterSkipOnly = AdAfterSkipOnlyCheckbox.IsChecked == true;

            s.HealAltar = HealAltarCheckbox.IsChecked == true;
            s.DeathAltar = DeathAltarCheckbox.IsChecked == true;

            s.PwOnBoss = PwOnBossCheckbox.IsChecked == true;

            try
            {
                s.PwOnBossDelay = int.Parse(PwOnBossDelayTextBox.Text);
            }
            catch
            {
                s.PwOnBossDelay = 0;
            }


            s.ScreenshotItems = ScreenshotItemsCheckbox.IsChecked == true;
            s.ScreenshotRunes = ScreenshotRunesCheckbox.IsChecked == true;
            s.ScreenshotSolvedCaptchas = ScreenshotSolvedCaptchasCheckbox.IsChecked == true;
            s.ScreenshotFailedCaptchas = ScreenshotFailedCaptchasCheckbox.IsChecked == true;
            s.ScreenshotCaptchaErrors = ScreenshotCaptchaErrors.IsChecked == true;
            s.ScreenshotOnEsc = ScreenshotOnEscCheckbox.IsChecked == true;
            s.ScreenshotLongLoad = ScreenshotLongLoadCheckbox.IsChecked == true;
            s.ScreenshotLongWave = ScreenshotLongWaveCheckbox.IsChecked == true;
            s.ScreenshotAfter10Esc = ScreenshotAfter10EscCheckbox.IsChecked == true;
            s.ScreenshotNoxLoadFail = ScreenshotNoxLoadFailCheckbox.IsChecked == true;
            s.ScreenshotNoxMainMenuLoadFail = ScreenshotNoxMainMenuLoadFailCheckbox.IsChecked == true;
            s.ScreenshotClearAllFail = ScreenshotNoxClearAllFailCheckbox.IsChecked == true;

            s.Build = new BuildSettings[5];

            s.Build[0] = B1.GetBuildSettings();
            s.Build[1]= B2.GetBuildSettings();
            s.Build[2] = B3.GetBuildSettings();
            s.Build[3] = B4.GetBuildSettings();
            s.Build[4] = B5.GetBuildSettings();

            return s;
        }

        public void SetFromSettings(ClickerSettings s)
        {
            if(s == null)
            {
                return;
            }

            WindowName.Text = s.WindowName;
            BuildToPlayComboBox.SelectedIndex = s.BuildToPlayIndex;

            StartClickerShortcutBox.Text = s.StartShortcut;
            StopClickerShortcutBox.Text = s.StopShortcut;

            FarmDungeonCheckbox.IsChecked = s.FarmDungeon;
            DungeonComboBox.SelectedIndex = s.DungeonIndex;
            MatBCheckbox.IsChecked = s.MatB;
            MatACheckbox.IsChecked = s.MatA;
            MatSCheckbox.IsChecked = s.MatS;
            MatLCheckbox.IsChecked = s.MatL;
            MatECheckbox.IsChecked = s.MatE;

            CastOnBossCheckbox.IsChecked = s.CastOnBossInDungeon;

            CastOnBossDelayTextBox.Text = s.CastOnBossInDungeonDelay.ToString();

            MakeReplaysIfDungeonDoesntLoadCheckBox.IsChecked = s.MakeReplaysIfDungeonDontLoad;

            ReplaysCheckbox.IsChecked = s.MakeReplays;

            SkipWavesCheckbox.IsChecked = s.SkipWaves;

            FiveWavesBetweenSkipsCheckbox.IsChecked = s.FiveWavesBetweenSpiks;
            SkipWithOrangesCheckbox.IsChecked = s.SkipWithOranges;

            ABModeCheckbox.IsChecked = s.ABMode;
            TabRadioButton.IsChecked = s.ABGabOrTab;
            GabRadioButton.IsChecked = !s.ABGabOrTab;

            ABWaveCancelingCheckbox.IsChecked = s.ABWaveCanceling;
            BreakABOn30CrystalsCheckbox.IsChecked = s.BreakAbOn30Crystals;

            TimeToBreakABTextBox.Text = s.TimeToBreakAB.ToString();
            SkipsBetweenABSessionsTextBox.Text = s.SkipsBetweenABSessions.ToString();

            BackgroundModeCheckbox.IsChecked = s.BackgroundMode;

            SolveCaptchaCheckbox.IsChecked = s.SolveCaptcha;
            RestartOnCaptchaCheckbox.IsChecked = s.RestartOnCaptcha;

            UpgradeCastleCheckbox.IsChecked = s.UpgradeCastle;
            FloorToUpgradeCastleComboBox.SelectedIndex = s.FloorToUpgradeCastle;

            UpgradeHeroForCrystalsCheckbox.IsChecked = s.UpgradeHero;
            SlotToUpgradeHeroComboBox.SelectedIndex = s.SlotToUpgradeHero;

            AdForSpeedCheckbox.IsChecked = s.AdForSpeed;
            AdForCoinsCheckbox.IsChecked = s.AdForCoins;

            AdDuringx3Checkbox.IsChecked = s.AdDuringX3;
            AdAfterSkipOnlyCheckbox.IsChecked = s.AdAfterSkipOnly;

            HealAltarCheckbox.IsChecked = s.HealAltar;
            DeathAltarCheckbox.IsChecked = s.DeathAltar;

            PwOnBossCheckbox.IsChecked = s.PwOnBoss;
            PwOnBossDelayTextBox.Text = s.PwOnBossDelay.ToString();

            ScreenshotItemsCheckbox.IsChecked = s.ScreenshotItems;
            ScreenshotRunesCheckbox.IsChecked = s.ScreenshotRunes;

            ScreenshotSolvedCaptchasCheckbox.IsChecked = s.ScreenshotSolvedCaptchas;
            ScreenshotFailedCaptchasCheckbox.IsChecked = s.ScreenshotFailedCaptchas;
            ScreenshotCaptchaErrors.IsChecked = s.ScreenshotCaptchaErrors;
            ScreenshotOnEscCheckbox.IsChecked = s.ScreenshotOnEsc;
            ScreenshotLongLoadCheckbox.IsChecked = s.ScreenshotLongLoad;
            ScreenshotLongWaveCheckbox.IsChecked = s.ScreenshotLongWave;
            ScreenshotAfter10EscCheckbox.IsChecked = s.ScreenshotAfter10Esc;
            ScreenshotNoxLoadFailCheckbox.IsChecked = s.ScreenshotNoxLoadFail;
            ScreenshotNoxMainMenuLoadFailCheckbox.IsChecked = s.ScreenshotNoxMainMenuLoadFail;
            ScreenshotNoxClearAllFailCheckbox.IsChecked = s.ScreenshotClearAllFail;

            BuildUserControl[] controls = new BuildUserControl[5]
            {
                B1, B2, B3, B4, B5
            };

            for(int i = 0; i < 5; i++)
            {
                controls[i].SetBuildSettings(s.Build[i]);
            }

        }

        private void SaveSettingsButton(object sender, RoutedEventArgs e)
        {

            ClickerSettings settings = GetClickerSettings();

            var dialog = new SaveFileDialog
            {
                Title = "Select file to save settings",
                Filter = "JSON Files (*.json)|*.json|All files|*.*",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dialog.FileName, json);
            }
        }

        private void LoadSettingsButton(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All files|*.*",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(dialog.FileName);
                ClickerSettings settings = JsonSerializer.Deserialize<ClickerSettings>(json);
                // if its wrong json file, will return default settings

                openToRewrite = false;

                SetFromSettings(settings);

                openToRewrite = true;
                RewriteCurrentSettings();
            }
        }

    }
}