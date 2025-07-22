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
using gca_clicker.Classes.SettingsScripts;
using gca_clicker.Enums;

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

        private List<CheckBox> allCheckboxes = new List<CheckBox>();
        private List<TextBox> allTextBoxes = new List<TextBox>();
        private List<ComboBox> allComboBoxes = new List<ComboBox>();
        private List<RadioButton> allRadioButtons = new List<RadioButton>();

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

            CollectUIObjects(this);


            UpdateWaitBetweenBattlesWaitState();

            Log.V("App started");

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
            if(source != null)
            {
                OnStopHotkey();
                source.RemoveHook(HwndHook);
                WinAPI.UnregisterHotKey(source.Handle, HOTKEY_START_ID);

                Settings.Default.WindowTop = this.Top;
                Settings.Default.WindowLeft = this.Left;
                Settings.Default.Save();
            }
            Log.V("App closed");
        }

        public void RewriteCurrentSettings(object sender = null!)
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
            if (!isActive)
            {
                return;
            }
            if (sender is CheckBox cb)
            {
                cb.Background = new SolidColorBrush(Colors.Orange);
            }
            else if (sender is TextBox tb)
            {
                tb.Background = new SolidColorBrush(Colors.Orange);
            }
            else if (sender is ComboBox cbx)
            {
                cbx.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (sender is Button b)
            {
                b.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (sender is RadioButton rb)
            {
                rb.Background = new SolidColorBrush(Colors.Orange);
            }
        }

        public void ResetColors()
        {
            foreach (var cb in allCheckboxes)
            {
                cb.Background = new SolidColorBrush(Colors.White);
            }
            foreach (var tb in allTextBoxes)
            {
                tb.Background = new SolidColorBrush(Colors.White);
            }
            foreach (var cbx in allComboBoxes)
            {
                cbx.Foreground = new SolidColorBrush(Colors.Black);
            }
            foreach (var rb in allRadioButtons)
            {
                rb.Background = new SolidColorBrush(Colors.White);
            }
            B1.ResetColors();
            B2.ResetColors();
            B3.ResetColors();
            B4.ResetColors();
            B5.ResetColors();
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

        private void NumberOnlyMaxLength_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is not TextBox textBox)
            {
                return;
            }

            if (!IsTextNumeric(e.Text))
            {
                e.Handled = true;
                return;
            }

            if (!int.TryParse(textBox.Tag?.ToString(), out int maxLength))
            {
                maxLength = 4;
            }

            int currentLength = textBox.Text.Length;
            int selectionLength = textBox.SelectionLength;
            int finalLength = currentLength - selectionLength + e.Text.Length;

            if (finalLength > maxLength)
            {
                e.Handled = true;
            }
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

        private void CollectUIObjects(DependencyObject obj)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(obj))
            {
                if (child is CheckBox cb)
                {
                    allCheckboxes.Add(cb);
                }
                else if (child is TextBox tb)
                {
                    allTextBoxes.Add(tb);
                }
                else if (child is ComboBox cbx)
                {
                    allComboBoxes.Add(cbx);
                }
                else if (child is RadioButton rb)
                {
                    allRadioButtons.Add(rb);
                }
                else if (child is DependencyObject depChild)
                {
                    CollectUIObjects(depChild);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //thread.Start();

            InfoLabel.Content = "";
            IntPtr hwnd = WinAPI.FindWindow(null!, WindowName.Text);
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

        public IEnumerable<WaitBetweenBattlesUserControl> GetWaitBetweenBattlesUserControls()
        {
            foreach (var c in WaitBetweenBattlesUCStackPanel.Children)
            {
                if (c is WaitBetweenBattlesUserControl wbbuc)
                {
                    yield return wbbuc;
                }
            }
        }

        private void TestButton(object sender, RoutedEventArgs e)
        {

            backgroundMode = false;
            nint hWnd = WndFind(WindowName.Text);

            if (hWnd != IntPtr.Zero)
            {
                hwnd = hWnd;
                SendKey(Keys.Escape);

            }
            else
            {
                WinAPI.ForceBringWindowToFront(this);
                MessageBox.Show($"Window not found: {WindowName.Text}");
            }



        }

        public ClickerSettings GetClickerSettings(bool throwIfError = false)
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


            try
            {
                s.MatGetDelayMin = int.Parse(MatTimeMinTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.MatGetDelayMin)} wrong value");
                }
                s.MatGetDelayMin = 0;
            }
            try
            {
                s.MatGetDelayMax = int.Parse(MatTimeMaxTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.MatGetDelayMax)} wrong value");
                }
                s.MatGetDelayMax = 0;
            }


            s.CastOnBossInDungeon = CastOnBossCheckbox.IsChecked == true;
            try
            {
                s.CastOnBossInDungeonDelay = int.Parse(CastOnBossDelayTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.CastOnBossInDungeonDelay)} wrong value");
                }
                s.CastOnBossInDungeonDelay = 0;
            }
            s.MakeReplaysIfDungeonDontLoad = MakeReplaysIfDungeonDoesntLoadCheckBox.IsChecked == true;

            s.MissclickOnDungeons = MissclicksOnDungeonsCheckbox.IsChecked == true;
            s.MissclickOnDungeonsIncludeDiagonals = MissclicksOnDungeonsIncludeDiagonalsCheckbox.IsChecked == true;

            try
            {
                s.MissclickOnDungeonsChance = int.Parse(MissclickOnDungeonsChanceTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.MissclickOnDungeonsChance)} wrong value");
                }
                s.MissclickOnDungeonsChance = 0;
            }


            s.MakeReplays = ReplaysCheckbox.IsChecked == true;

            s.SkipWaves = SkipWavesCheckbox.IsChecked == true;

            s.SkipWithOranges = SkipWithOrangesCheckbox.IsChecked == true;

            s.ABMode = ABModeCheckbox.IsChecked == true;
            s.ABGabOrTab = TabRadioButton.IsChecked == true;

            s.ABWaveCanceling = ABWaveCancelingCheckbox.IsChecked == true;

            s.BreakAbOn30Crystals = BreakABOn30CrystalsCheckbox.IsChecked == true;

            try
            {
                s.TimeToBreakABMin = int.Parse(TimeToBreakABMinTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.TimeToBreakABMin)} wrong value");
                }
                s.TimeToBreakABMin = 0;
            }
            try
            {
                s.TimeToBreakABMax = int.Parse(TimeToBreakABMaxTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.TimeToBreakABMax)} wrong value");
                }
                s.TimeToBreakABMax = 0;
            }

            try
            {
                s.SkipsBetweenABSessionsMin = int.Parse(SkipsBetweenABSessionsMinTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.SkipsBetweenABSessionsMin)} wrong value");
                }
                s.SkipsBetweenABSessionsMin = 0;
            }
            try
            {
                s.SkipsBetweenABSessionsMax = int.Parse(SkipsBetweenABSessionsMaxTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.SkipsBetweenABSessionsMax)} wrong value");
                }
                s.SkipsBetweenABSessionsMax = 0;
            }

            s.BackgroundMode = BackgroundModeCheckbox.IsChecked == true;
            s.SimulateMouseMovement = SimulateMouseMovementCheckbox.IsChecked == true;
            s.MonitorFreezing = MonitorFreezingCheckbox.IsChecked == true;


            s.RandomizeCastSequence = RandomizeCastSequenceCheckbox.IsChecked == true;

            try
            {
                s.HeroClickWaitMin = int.Parse(HeroClickWaitMinTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.HeroClickWaitMin)} wrong value");
                }
                s.HeroClickWaitMin = 0;
            }
            try
            {
                s.HeroClickWaitMax = int.Parse(HeroClickWaitMaxTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.HeroClickWaitMax)} wrong value");
                }
                s.HeroClickWaitMax = 0;
            }

            try
            {
                s.WaitBetweenCastsMin = int.Parse(WaitBetweenCastsMinTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.WaitBetweenCastsMin)} wrong value");
                }
                s.WaitBetweenCastsMin = 0;
            }
            try
            {
                s.WaitBetweenCastsMax = int.Parse(WaitBetweenCastsMaxTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.WaitBetweenCastsMax)} wrong value");
                }
                s.WaitBetweenCastsMax = 0;
            }


            try
            {
                s.WaitOnBattleButtonsMin = int.Parse(WaitOnBattleButtonsMinTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.WaitOnBattleButtonsMin)} wrong value");
                }
                s.WaitOnBattleButtonsMin = 0;
            }
            try
            {
                s.WaitOnBattleButtonsMax = int.Parse(WaitOnBattleButtonsMaxTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.WaitOnBattleButtonsMax)} wrong value");
                }
                s.WaitOnBattleButtonsMax = 0;
            }

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

            try
            {
                s.MaxBattleLengthMs = int.Parse(MaxBattleLengthTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.MaxBattleLengthMs)} wrong value");
                }
                s.MaxBattleLengthMs = 0;
            }
            try
            {
                s.CleanupIntervalSec = int.Parse(CleanupIntervalTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.CleanupIntervalSec)} wrong value");
                }
                s.CleanupIntervalSec = 0;
            }

            s.DoResetOnCleanup = ResetRadioButton.IsChecked == true;


            try
            {
                s.MaxRestartsForReset = int.Parse(MaxRestartsForResetTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.MaxRestartsForReset)} wrong value");
                }
                s.MaxRestartsForReset = 0;
            }

            s.OrcbandOnSkipOnly = OrcbandOnSkipOnlyCheckbox.IsChecked == true;
            s.MilitaryFOnSkipOnly = MilitaryFOnSkipOnlyCheckbox.IsChecked == true;

            s.IHaveX3 = IHaveX3Checkbox.IsChecked == true;

            s.CollectMimic = CollectMimicCheckbox.IsChecked == true;

            s.SpeedupOnItemDrop = SpeedupOnItemDropCheckbox.IsChecked == true;

            s.DoSaveOnCleanup = DoSaveBeofreCleanupCheckbox.IsChecked == true;

            try
            {
                s.CollectMimicChance = int.Parse(CollectMimicChanceTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.CollectMimicChance)} wrong value");
                }
                s.CollectMimicChance = 0;
            }
            try
            {
                s.GcLoadingLimit = int.Parse(GcLoadingLimitTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.GcLoadingLimit)} wrong value");
                }
                s.GcLoadingLimit = 0;
            }
            try
            {
                s.FixedAdWait = int.Parse(FixedAdWaitTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.FixedAdWait)} wrong value");
                }
                s.FixedAdWait = 0;
            }

            s.PwOnBoss = PwOnBossCheckbox.IsChecked == true;

            try
            {
                s.PwOnBossDelay = int.Parse(PwOnBossDelayTextBox.Text);
            }
            catch
            {
                if (throwIfError)
                {
                    throw new($"{nameof(s.PwOnBossDelay)} wrong value");
                }
                s.PwOnBossDelay = 0;
            }

            foreach(var c in GetWaitBetweenBattlesUserControls())
            {
                s.WaitBetweenBattlesSettings.Add(c.GetSetting(throwIfError));
            }

            s.IgnoreWaitsOnABMode = IgnoreWaitsOnABModeCheckbox.IsChecked == true;

            s.ScreenshotItems = ScreenshotItemsCheckbox.IsChecked == true;
            s.ScreenshotRunes = ScreenshotRunesCheckbox.IsChecked == true;
            s.ScreenshotSolvedCaptchas = ScreenshotSolvedCaptchasCheckbox.IsChecked == true;
            s.ScreenshotFailedCaptchas = ScreenshotFailedCaptchasCheckbox.IsChecked == true;
            s.ScreenshotCaptchaErrors = ScreenshotCaptchaErrors.IsChecked == true;
            s.ScreenshotOnEsc = ScreenshotOnEscCheckbox.IsChecked == true;
            s.ScreenshotLongLoad = ScreenshotLongLoadCheckbox.IsChecked == true;
            s.ScreenshotLongWave = ScreenshotLongWaveCheckbox.IsChecked == true;
            s.ScreenshotAfter10Esc = ScreenshotAfter10EscCheckbox.IsChecked == true;
            s.ScreenshotABErrors = ScreenshotABErrorsCheckbox.IsChecked == true;
            s.ScreenshotNoxLoadFail = ScreenshotNoxLoadFailCheckbox.IsChecked == true;
            s.ScreenshotNoxMainMenuLoadFail = ScreenshotNoxMainMenuLoadFailCheckbox.IsChecked == true;
            s.ScreenshotClearAllFail = ScreenshotNoxClearAllFailCheckbox.IsChecked == true;

            s.Build = new BuildSettings[5];

            s.Build[0] = B1.GetBuildSettings();
            s.Build[1] = B2.GetBuildSettings();
            s.Build[2] = B3.GetBuildSettings();
            s.Build[3] = B4.GetBuildSettings();
            s.Build[4] = B5.GetBuildSettings();

            return s;
        }

        public void SetFromSettings(ClickerSettings s)
        {
            if (s == null)
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

            MatTimeMinTextBox.Text = s.MatGetDelayMin.ToString();
            MatTimeMaxTextBox.Text = s.MatGetDelayMax.ToString();

            CastOnBossCheckbox.IsChecked = s.CastOnBossInDungeon;

            CastOnBossDelayTextBox.Text = s.CastOnBossInDungeonDelay.ToString();

            MakeReplaysIfDungeonDoesntLoadCheckBox.IsChecked = s.MakeReplaysIfDungeonDontLoad;

            MissclicksOnDungeonsCheckbox.IsChecked = s.MissclickOnDungeons;
            MissclicksOnDungeonsIncludeDiagonalsCheckbox.IsChecked = s.MissclickOnDungeonsIncludeDiagonals;

            MissclickOnDungeonsChanceTextBox.Text = s.MissclickOnDungeonsChance.ToString();

            ReplaysCheckbox.IsChecked = s.MakeReplays;

            SkipWavesCheckbox.IsChecked = s.SkipWaves;

            SkipWithOrangesCheckbox.IsChecked = s.SkipWithOranges;

            ABModeCheckbox.IsChecked = s.ABMode;
            TabRadioButton.IsChecked = s.ABGabOrTab;
            GabRadioButton.IsChecked = !s.ABGabOrTab;

            ABWaveCancelingCheckbox.IsChecked = s.ABWaveCanceling;
            BreakABOn30CrystalsCheckbox.IsChecked = s.BreakAbOn30Crystals;

            TimeToBreakABMinTextBox.Text = s.TimeToBreakABMin.ToString();
            TimeToBreakABMaxTextBox.Text = s.TimeToBreakABMax.ToString();

            SkipsBetweenABSessionsMinTextBox.Text = s.SkipsBetweenABSessionsMin.ToString();
            SkipsBetweenABSessionsMaxTextBox.Text = s.SkipsBetweenABSessionsMax.ToString();

            BackgroundModeCheckbox.IsChecked = s.BackgroundMode;
            SimulateMouseMovementCheckbox.IsChecked = s.SimulateMouseMovement;
            RandomizeCastSequenceCheckbox.IsChecked = s.RandomizeCastSequence;
            MonitorFreezingCheckbox.IsChecked = s.MonitorFreezing;

            HeroClickWaitMinTextBox.Text = s.HeroClickWaitMin.ToString();
            HeroClickWaitMaxTextBox.Text = s.HeroClickWaitMax.ToString();

            WaitBetweenCastsMinTextBox.Text = s.WaitBetweenCastsMin.ToString();
            WaitBetweenCastsMaxTextBox.Text = s.WaitBetweenCastsMax.ToString();

            WaitOnBattleButtonsMinTextBox.Text = s.WaitOnBattleButtonsMin.ToString();
            WaitOnBattleButtonsMaxTextBox.Text = s.WaitOnBattleButtonsMax.ToString();

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

            MaxBattleLengthTextBox.Text = s.MaxBattleLengthMs.ToString();
            CleanupIntervalTextBox.Text = s.CleanupIntervalSec.ToString();

            MaxRestartsForResetTextBox.Text = s.MaxRestartsForReset.ToString();

            OrcbandOnSkipOnlyCheckbox.IsChecked = s.OrcbandOnSkipOnly;
            MilitaryFOnSkipOnlyCheckbox.IsChecked = s.MilitaryFOnSkipOnly;

            IHaveX3Checkbox.IsChecked = s.IHaveX3;

            SpeedupOnItemDropCheckbox.IsChecked = s.SpeedupOnItemDrop;

            CollectMimicCheckbox.IsChecked = s.CollectMimic;
            CollectMimicChanceTextBox.Text = s.CollectMimicChance.ToString();

            GcLoadingLimitTextBox.Text = s.GcLoadingLimit.ToString();
            FixedAdWaitTextBox.Text = s.FixedAdWait.ToString();

            ResetRadioButton.IsChecked = s.DoResetOnCleanup;
            CleanupRadioButton.IsChecked = !s.DoResetOnCleanup;
            DoSaveBeofreCleanupCheckbox.IsChecked = s.DoSaveOnCleanup;

            IgnoreWaitsOnABModeCheckbox.IsChecked = s.IgnoreWaitsOnABMode;

            foreach(var wbb in WaitBetweenBattlesUCStackPanel.Children)
            {
                if(wbb is WaitBetweenBattlesUserControl wbbuc)
                {
                    RemoveWaitBetweenBattlesUserControl(wbbuc);
                }
            }

            foreach(var wbb in s.WaitBetweenBattlesSettings)
            {
                WaitBetweenBattlesUserControl uc = new WaitBetweenBattlesUserControl(AdvancedTabScrollViewer);
                uc.SetFromSettings(wbb);
                AddWaitBetweenBattlesUserControl(uc);
            }
            UpdateWaitBetweenBattlesWaitState();

            ScreenshotItemsCheckbox.IsChecked = s.ScreenshotItems;
            ScreenshotRunesCheckbox.IsChecked = s.ScreenshotRunes;

            ScreenshotSolvedCaptchasCheckbox.IsChecked = s.ScreenshotSolvedCaptchas;
            ScreenshotFailedCaptchasCheckbox.IsChecked = s.ScreenshotFailedCaptchas;
            ScreenshotCaptchaErrors.IsChecked = s.ScreenshotCaptchaErrors;
            ScreenshotOnEscCheckbox.IsChecked = s.ScreenshotOnEsc;
            ScreenshotLongLoadCheckbox.IsChecked = s.ScreenshotLongLoad;
            ScreenshotLongWaveCheckbox.IsChecked = s.ScreenshotLongWave;
            ScreenshotAfter10EscCheckbox.IsChecked = s.ScreenshotAfter10Esc;
            ScreenshotABErrorsCheckbox.IsChecked = s.ScreenshotABErrors;
            ScreenshotNoxLoadFailCheckbox.IsChecked = s.ScreenshotNoxLoadFail;
            ScreenshotNoxMainMenuLoadFailCheckbox.IsChecked = s.ScreenshotNoxMainMenuLoadFail;
            ScreenshotNoxClearAllFailCheckbox.IsChecked = s.ScreenshotClearAllFail;

            BuildUserControl[] controls = new BuildUserControl[5]
            {
                B1, B2, B3, B4, B5
            };

            for (int i = 0; i < 5; i++)
            {
                controls[i].SetBuildSettings(s.Build[i]);
            }

        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
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

        private void LoadSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All files|*.*",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(dialog.FileName);
                ClickerSettings settings = JsonSerializer.Deserialize<ClickerSettings>(json)!;
                // if its wrong json file, will return default settings

                openToRewrite = false;

                SetFromSettings(settings);

                openToRewrite = true;
                RewriteCurrentSettings();
            }
        }
        private void OpenInExplorer_Click(object sender, RoutedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Process.Start("explorer.exe", appDirectory);
        }

        private void OpenGithub_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/Silpux/GrowCastleAutobot";
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void OpenCurrentBuild_Click(object sender, RoutedEventArgs e)
        {
            int tabIndex = BuildToPlayComboBox.SelectedIndex;
            if(tabIndex >= 0)
            {
                MyTabControl.SelectedIndex = tabIndex + 1;
            }
        }
    }
}