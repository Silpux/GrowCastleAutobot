using gca_clicker.Classes;
using gca_clicker.Clicker;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using gca_clicker.Classes.SettingsScripts;
using gca_clicker.Classes.Tooltips;
using System.Reflection;
using System.Windows.Media.Media3D;
using System;
using System.Numerics;

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

        private List<System.Windows.Controls.CheckBox> allCheckboxes = new List<System.Windows.Controls.CheckBox>();
        private List<System.Windows.Controls.TextBox> allTextBoxes = new List<System.Windows.Controls.TextBox>();
        private List<System.Windows.Controls.ComboBox> allComboBoxes = new List<System.Windows.Controls.ComboBox>();
        private List<System.Windows.Controls.RadioButton> allRadioButtons = new List<System.Windows.Controls.RadioButton>();

        private bool isSwappingWbbuc = false;
        private int swapWbbucAnimationDuration = 250;

        private ScreenshotCache screenshotCache = new();

        private int coordNotTakenCounter = 0;

        private NotifyIcon trayIcon;

        MediaPlayer mediaPlayer = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closed += OnClosed;

            SetBackground(Cst.DefaultBackground, false);

            this.Title = Cst.APP_TITLE;

            this.PreviewMouseMove += Window_PreviewMouseMove;

            B1.OnUpdate += RewriteCurrentSettings;
            B2.OnUpdate += RewriteCurrentSettings;
            B3.OnUpdate += RewriteCurrentSettings;
            B4.OnUpdate += RewriteCurrentSettings;
            B5.OnUpdate += RewriteCurrentSettings;

            ApplyCurrentSettings();

            CollectUIObjects(this);

            string version = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion ?? "unknown";

            VersionLabel.Content = version;
            Debug.WriteLine(version);

            UpdateWaitBetweenBattlesWaitState();

            trayIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule!.FileName!)
                       ?? SystemIcons.Information,
                Visible = true,
#if DEBUG
                Text = "GCA DEBUG"
#else
                Text = "GCA"
#endif
            };
            trayIcon.Click += (s, ev) =>
            {
                if (WindowState == WindowState.Minimized)
                {
                    WindowState = WindowState.Normal;
                }

                Activate();
                Topmost = true;
                Topmost = false;
                Focus();
            };

            SetTooltips();

            Log.U($"App started. App version: {version}");

            openToRewrite = true;
        }

        public void SetTooltips()
        {
            string tooltip1 = $"There must be audio in this location:\n{Path.GetFullPath(Cst.AUDIO_30_CRYSTALS_1_PATH)}";
            TooltipHelper.SetEnabledTooltip(Audio1RadioButton, tooltip1);

            string tooltip2 = $"There must be audio in this location:\n{Path.GetFullPath(Cst.AUDIO_30_CRYSTALS_2_PATH)}";
            TooltipHelper.SetEnabledTooltip(Audio2RadioButton, tooltip2);
        }

        public void ShowBalloon(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            trayIcon.BalloonTipTitle = title;
            trayIcon.BalloonTipText = message;
            trayIcon.BalloonTipIcon = icon;

            trayIcon.ShowBalloonTip(4000);
        }

        public void PlayAudio(string path, double vol)
        {
            string pathToFile = Utils.FindFile(path);
            if (pathToFile is not null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    mediaPlayer.Volume = vol * vol;
                    mediaPlayer.Open(new Uri($"file:///{pathToFile.Replace("\\", "/")}"));
                    mediaPlayer.Play();
                });
            }
            else
            {
                Log.E($"File \"{Path.GetFullPath(path)}\" doesn't exist");
            }
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

        private void OnClosed(object? sender, EventArgs e)
        {
            if (source != null)
            {
                OnStopHotkey();
                source.RemoveHook(HwndHook);
                WinAPI.UnregisterHotKey(source.Handle, HOTKEY_START_ID);

                Settings.Default.WindowTop = this.Top;
                Settings.Default.WindowLeft = this.Left;
                Settings.Default.Save();

            }
            
            if(trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }

            Log.U("App closed");
        }

        private bool IsDescendantOf(DependencyObject child, DependencyObject ancestor)
        {
            while (child != null)
            {
                if (child == ancestor)
                {
                    return true;
                }
                if (child is Visual || child is Visual3D)
                {
                    child = VisualTreeHelper.GetParent(child);
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public void Window_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Mouse.DirectlyOver is not DependencyObject mouseOver || !IsDescendantOf(mouseOver, this))
            {
                return;
            }
            System.Windows.Point pos = e.GetPosition(this);
            HitTestResult result = VisualTreeHelper.HitTest(this, pos);
            if (result != null)
            {
                DependencyObject element = result.VisualHit;

                while (element != null)
                {
                    if (element is UIElement uiElement)
                    {
                        bool isEnabled = uiElement.IsEnabled;
                        string tooltip = isEnabled ? TooltipHelper.GetEnabledTooltip(uiElement) : TooltipHelper.GetDisabledTooltip(uiElement);

                        if (tooltip != null)
                        {
                            Mouse.OverrideCursor = TooltipHelper.GetTooltipCursor(uiElement);
                            return;
                        }
                    }

                    element = VisualTreeHelper.GetParent(element);
                }
                Mouse.OverrideCursor = null;

            }
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
            if (sender is System.Windows.Controls.CheckBox cb)
            {
                cb.Background = new SolidColorBrush(Colors.Orange);
            }
            else if (sender is System.Windows.Controls.TextBox tb)
            {
                tb.Background = new SolidColorBrush(Colors.Orange);
            }
            else if (sender is System.Windows.Controls.ComboBox cbx)
            {
                cbx.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (sender is System.Windows.Controls.Button b)
            {
                b.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (sender is System.Windows.Controls.RadioButton rb)
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

        public void SetBackground(SolidColorBrush color, bool inDispatcher = true)
        {
            if (inDispatcher)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    AdvancedTabScrollViewer.Background = color;
                });
            }
            else
            {
                AdvancedTabScrollViewer.Background = color;
            }
        }

        public void SetCanvasChildrenState(Canvas canvas, bool state)
        {
            foreach (UIElement element in canvas.Children)
            {
                if (element is System.Windows.Controls.Control control)
                {
                    control.IsEnabled = state;
                }
            }
        }
        public void ApplyCurrentSettings()
        {
            ClickerSettings settings = null!;
            try
            {
                string json = File.ReadAllText(Cst.CURRENT_SETTINGS_FILE_PATH);
                settings = JsonSerializer.Deserialize<ClickerSettings>(json)!;
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
            if (sender is not System.Windows.Controls.TextBox textBox)
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
            return null!;
        }

        private void CollectUIObjects(DependencyObject obj)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(obj))
            {
                if (child is System.Windows.Controls.CheckBox cb)
                {
                    allCheckboxes.Add(cb);
                }
                else if (child is System.Windows.Controls.TextBox tb)
                {
                    allTextBoxes.Add(tb);
                }
                else if (child is System.Windows.Controls.ComboBox cbx)
                {
                    allComboBoxes.Add(cbx);
                }
                else if (child is System.Windows.Controls.RadioButton rb)
                {
                    allRadioButtons.Add(rb);
                }
                else if (child is DependencyObject depChild)
                {
                    CollectUIObjects(depChild);
                }
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
        private void ParseIntOrDefault(System.Windows.Controls.TextBox textBox, Action<int> assign, string propName, bool throwIfError)
        {
            try
            {
                assign(int.Parse(textBox.Text));
            }
            catch
            {
                if (throwIfError)
                {
                    throw new Exception($"{propName} wrong value");
                }
                assign(0);
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

            ParseIntOrDefault(MatTimeMinTextBox, n => s.MatGetDelayMin = n, nameof(s.MatGetDelayMin), throwIfError);
            ParseIntOrDefault(MatTimeMaxTextBox, n => s.MatGetDelayMax = n, nameof(s.MatGetDelayMax), throwIfError);

            s.CastOnBossInDungeon = CastOnBossCheckbox.IsChecked == true;

            ParseIntOrDefault(CastOnBossDelayTextBox, n => s.CastOnBossInDungeonDelay = n, nameof(s.CastOnBossInDungeonDelay), throwIfError);

            s.MakeReplaysIfDungeonDontLoad = MakeReplaysIfDungeonDoesntLoadCheckBox.IsChecked == true;

            s.MissclickOnDungeons = MissclicksOnDungeonsCheckbox.IsChecked == true;
            s.MissclickOnDungeonsIncludeDiagonals = MissclicksOnDungeonsIncludeDiagonalsCheckbox.IsChecked == true;

            ParseIntOrDefault(MissclickOnDungeonsChanceTextBox, n => s.MissclickOnDungeonsChance = n, nameof(s.MissclickOnDungeonsChance), throwIfError);

            s.MakeReplays = ReplaysCheckbox.IsChecked == true;

            s.SkipWaves = SkipWavesCheckbox.IsChecked == true;

            s.SkipWithOranges = SkipWithOrangesCheckbox.IsChecked == true;

            s.ABMode = ABModeCheckbox.IsChecked == true;
            s.ABGabOrTab = TabRadioButton.IsChecked == true;

            s.ABWaveCanceling = ABWaveCancelingCheckbox.IsChecked == true;

            s.BreakAbOn30Crystals = BreakABOn30CrystalsCheckbox.IsChecked == true;

            ParseIntOrDefault(TimeToBreakABMinTextBox, n => s.TimeToBreakABMin = n, nameof(s.TimeToBreakABMin), throwIfError);
            ParseIntOrDefault(TimeToBreakABMaxTextBox, n => s.TimeToBreakABMax = n, nameof(s.TimeToBreakABMax), throwIfError);

            ParseIntOrDefault(SkipsBetweenABSessionsMinTextBox, n => s.SkipsBetweenABSessionsMin = n, nameof(s.SkipsBetweenABSessionsMin), throwIfError);

            ParseIntOrDefault(SkipsBetweenABSessionsMaxTextBox, n => s.SkipsBetweenABSessionsMax = n, nameof(s.SkipsBetweenABSessionsMax), throwIfError);

            s.DesktopNotificationOn30Crystals = DesktopNotificationOn30CrystalsCheckbox.IsChecked == true;

            ParseIntOrDefault(DesktopNotification30CrystalsIntervalTextBox, n => s.DesktopNotificationOn30CrystalsInterval = n, nameof(s.DesktopNotificationOn30CrystalsInterval), throwIfError);

            s.PlayAudioOn30Crystals = PlayAudioOn30CrystalsCheckbox?.IsChecked == true;

            ParseIntOrDefault(PlayAudio30CrystalsIntervalTextBox, n => s.PlayAudioOn30CrystalsInterval = n, nameof(s.PlayAudioOn30CrystalsInterval), throwIfError);

            ParseIntOrDefault(PlayAudio1_30CrystalsVolumeTextBox, n => s.PlayAudio1On30CrystalsVolume = n, nameof(s.PlayAudio1On30CrystalsVolume), throwIfError);

            ParseIntOrDefault(PlayAudio2_30CrystalsVolumeTextBox, n => s.PlayAudio2On30CrystalsVolume = n, nameof(s.PlayAudio2On30CrystalsVolume), throwIfError);

            s.Audio30CrystalsIndex = Audio2RadioButton.IsChecked == true ? 1 : 0;

            s.NotificationOnlyMode = NotificationOnlyModeCheckbox.IsChecked == true;
            s.Log30CrystalsDetection = Log30CrystalsCollectionTimeCheckbox.IsChecked == true;

            s.BackgroundMode = BackgroundModeCheckbox.IsChecked == true;
            s.SimulateMouseMovement = SimulateMouseMovementCheckbox.IsChecked == true;
            s.MonitorFreezing = MonitorFreezingCheckbox.IsChecked == true;

            s.RandomizeCastSequence = RandomizeCastSequenceCheckbox.IsChecked == true;

            ParseIntOrDefault(HeroClickWaitMinTextBox, n => s.HeroClickWaitMin = n, nameof(s.HeroClickWaitMin), throwIfError);
            ParseIntOrDefault(HeroClickWaitMaxTextBox, n => s.HeroClickWaitMax = n, nameof(s.HeroClickWaitMax), throwIfError);
            ParseIntOrDefault(WaitBetweenCastsMinTextBox, n => s.WaitBetweenCastsMin = n, nameof(s.WaitBetweenCastsMin), throwIfError);
            ParseIntOrDefault(WaitBetweenCastsMaxTextBox, n => s.WaitBetweenCastsMax = n, nameof(s.WaitBetweenCastsMax), throwIfError);
            ParseIntOrDefault(WaitOnBattleButtonsMinTextBox, n => s.WaitOnBattleButtonsMin = n, nameof(s.WaitOnBattleButtonsMin), throwIfError);
            ParseIntOrDefault(WaitOnBattleButtonsMaxTextBox, n => s.WaitOnBattleButtonsMax = n, nameof(s.WaitOnBattleButtonsMax), throwIfError);

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

            ParseIntOrDefault(MaxBattleLengthTextBox, n => s.MaxBattleLengthMs = n, nameof(s.MaxBattleLengthMs), throwIfError);
            ParseIntOrDefault(CleanupIntervalMinTextBox, n => s.CleanupIntervalSecMin = n, nameof(s.CleanupIntervalSecMin), throwIfError);
            ParseIntOrDefault(CleanupIntervalMaxTextBox, n => s.CleanupIntervalSecMax = n, nameof(s.CleanupIntervalSecMax), throwIfError);

            s.DoResetOnCleanup = ResetRadioButton.IsChecked == true;

            s.DisableResetCleanupCheck = DisableResetCleanupCheck.IsChecked == true;

            s.DoRestarts = DoRestartsCheckBox.IsChecked == true;

            ParseIntOrDefault(DoRestartsIntervalMinTextBox, n => s.RestartsIntervalMin = n, nameof(s.RestartsIntervalMin), throwIfError);

            ParseIntOrDefault(DoRestartsIntervalMaxTextBox, n => s.RestartsIntervalMax = n, nameof(s.RestartsIntervalMax), throwIfError);

            ParseIntOrDefault(MaxRestartsForResetTextBox, n => s.MaxRestartsForReset = n, nameof(s.MaxRestartsForReset), throwIfError);

            s.OrcbandOnSkipOnly = OrcbandOnSkipOnlyCheckbox.IsChecked == true;
            s.MilitaryFOnSkipOnly = MilitaryFOnSkipOnlyCheckbox.IsChecked == true;

            s.IHaveX3 = IHaveX3Checkbox.IsChecked == true;

            s.CollectMimic = CollectMimicCheckbox.IsChecked == true;

            s.SpeedupOnItemDrop = SpeedupOnItemDropCheckbox.IsChecked == true;

            s.DoSaveOnCleanup = DoSaveBeofreCleanupCheckbox.IsChecked == true;

            ParseIntOrDefault(CollectMimicChanceTextBox, n => s.CollectMimicChance = n, nameof(s.CollectMimicChance), throwIfError);
            ParseIntOrDefault(GcLoadingLimitTextBox, n => s.GcLoadingLimit = n, nameof(s.GcLoadingLimit), throwIfError);
            ParseIntOrDefault(FixedAdWaitTextBox, n => s.FixedAdWait = n, nameof(s.FixedAdWait), throwIfError);

            s.PwOnBoss = PwOnBossCheckbox.IsChecked == true;

            ParseIntOrDefault(PwOnBossDelayTextBox, n => s.PwOnBossDelay = n, nameof(s.PwOnBossDelay), throwIfError);


            foreach (var c in GetWaitBetweenBattlesUserControls())
            {
                s.WaitBetweenBattlesSettings.Add(c.GetSetting(throwIfError));
            }

            s.IgnoreWaitsOnABMode = IgnoreWaitsOnABModeCheckbox.IsChecked == true;

            s.ScreenshotItems = ScreenshotItemsCheckbox.IsChecked == true;
            s.ScreenshotRunes = ScreenshotRunesCheckbox.IsChecked == true;
            s.ScreenshotPopups = ScreenshotPopupsCheckbox.IsChecked == true;
            s.ScreenshotSolvedCaptchas = ScreenshotSolvedCaptchasCheckbox.IsChecked == true;
            s.ScreenshotFailedCaptchas = ScreenshotFailedCaptchasCheckbox.IsChecked == true;
            s.ScreenshotCaptchaErrors = ScreenshotCaptchaErrorsCheckbox.IsChecked == true;
            s.ScreenshotOnEsc = ScreenshotOnEscCheckbox.IsChecked == true;
            s.ScreenshotLongLoad = ScreenshotLongLoadCheckbox.IsChecked == true;
            s.ScreenshotLongWave = ScreenshotLongWaveCheckbox.IsChecked == true;
            s.ScreenshotAfter10Esc = ScreenshotAfter10EscCheckbox.IsChecked == true;
            s.ScreenshotABErrors = ScreenshotABErrorsCheckbox.IsChecked == true;
            s.ScreenshotOnFreezing = ScreenshotOnFreezingCheckbox.IsChecked == true;
            s.ScreenshotNoxLoadFail = ScreenshotNoxLoadFailCheckbox.IsChecked == true;
            s.ScreenshotNoxMainMenuLoadFail = ScreenshotNoxMainMenuLoadFailCheckbox.IsChecked == true;
            s.ScreenshotClearAllFail = ScreenshotNoxClearAllFailCheckbox.IsChecked == true;

            s.SaveScreenshotsCacheOnError = SaveScreenshotsOnErrorCheckbox.IsChecked == true;

            ParseIntOrDefault(CacheDurationSecondsTextBox, n => s.CacheDurationSeconds = n, nameof(s.CacheDurationSeconds), throwIfError);
            ParseIntOrDefault(CacheIntervalMsTextBox, n => s.CacheIntervalMs = n, nameof(s.CacheIntervalMs), throwIfError);
            ParseIntOrDefault(CacheImageQualityTextBox, n => s.CacheImageQuality = n, nameof(s.CacheImageQuality), throwIfError);

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

            TimeToBreakABMinTextBox.Text = s.TimeToBreakABMin.ToString();
            TimeToBreakABMaxTextBox.Text = s.TimeToBreakABMax.ToString();

            SkipsBetweenABSessionsMinTextBox.Text = s.SkipsBetweenABSessionsMin.ToString();
            SkipsBetweenABSessionsMaxTextBox.Text = s.SkipsBetweenABSessionsMax.ToString();

            ABWaveCancelingCheckbox.IsChecked = s.ABWaveCanceling;
            BreakABOn30CrystalsCheckbox.IsChecked = s.BreakAbOn30Crystals;
            DesktopNotificationOn30CrystalsCheckbox.IsChecked = s.DesktopNotificationOn30Crystals;
            DesktopNotification30CrystalsIntervalTextBox.Text = s.DesktopNotificationOn30CrystalsInterval.ToString();
            PlayAudioOn30CrystalsCheckbox.IsChecked = s.PlayAudioOn30Crystals;
            PlayAudio30CrystalsIntervalTextBox.Text = s.PlayAudioOn30CrystalsInterval.ToString();
            PlayAudio1_30CrystalsVolumeTextBox.Text = s.PlayAudio1On30CrystalsVolume.ToString();
            PlayAudio2_30CrystalsVolumeTextBox.Text = s.PlayAudio2On30CrystalsVolume.ToString();

            Audio1RadioButton.IsChecked = s.Audio30CrystalsIndex == 0;
            Audio2RadioButton.IsChecked = s.Audio30CrystalsIndex == 1;

            NotificationOnlyModeCheckbox.IsChecked = s.NotificationOnlyMode;
            Log30CrystalsCollectionTimeCheckbox.IsChecked = s.Log30CrystalsDetection;

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
            CleanupIntervalMinTextBox.Text = s.CleanupIntervalSecMin.ToString();
            CleanupIntervalMaxTextBox.Text = s.CleanupIntervalSecMax.ToString();

            DoRestartsCheckBox.IsChecked = s.DoRestarts;
            DoRestartsIntervalMinTextBox.Text = s.RestartsIntervalMin.ToString();
            DoRestartsIntervalMaxTextBox.Text = s.RestartsIntervalMax.ToString();

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

            DisableResetCleanupCheck.IsChecked = s.DisableResetCleanupCheck;

            IgnoreWaitsOnABModeCheckbox.IsChecked = s.IgnoreWaitsOnABMode;

            foreach (var wbb in WaitBetweenBattlesUCStackPanel.Children.Cast<UIElement>().ToList())
            {
                if (wbb is WaitBetweenBattlesUserControl wbbuc)
                {
                    RemoveWaitBetweenBattlesUserControl(wbbuc);
                }
            }

            foreach (var wbb in s.WaitBetweenBattlesSettings)
            {
                WaitBetweenBattlesUserControl uc = new WaitBetweenBattlesUserControl(AdvancedTabScrollViewer);
                uc.SetFromSettings(wbb);
                AddWaitBetweenBattlesUserControl(uc);
            }
            UpdateWaitBetweenBattlesWaitState();

            ScreenshotItemsCheckbox.IsChecked = s.ScreenshotItems;
            ScreenshotRunesCheckbox.IsChecked = s.ScreenshotRunes;

            ScreenshotPopupsCheckbox.IsChecked = s.ScreenshotPopups;

            ScreenshotSolvedCaptchasCheckbox.IsChecked = s.ScreenshotSolvedCaptchas;
            ScreenshotFailedCaptchasCheckbox.IsChecked = s.ScreenshotFailedCaptchas;
            ScreenshotCaptchaErrorsCheckbox.IsChecked = s.ScreenshotCaptchaErrors;
            ScreenshotOnEscCheckbox.IsChecked = s.ScreenshotOnEsc;
            ScreenshotLongLoadCheckbox.IsChecked = s.ScreenshotLongLoad;
            ScreenshotLongWaveCheckbox.IsChecked = s.ScreenshotLongWave;
            ScreenshotAfter10EscCheckbox.IsChecked = s.ScreenshotAfter10Esc;
            ScreenshotABErrorsCheckbox.IsChecked = s.ScreenshotABErrors;
            ScreenshotOnFreezingCheckbox.IsChecked = s.ScreenshotOnFreezing;
            ScreenshotNoxLoadFailCheckbox.IsChecked = s.ScreenshotNoxLoadFail;
            ScreenshotNoxMainMenuLoadFailCheckbox.IsChecked = s.ScreenshotNoxMainMenuLoadFail;
            ScreenshotNoxClearAllFailCheckbox.IsChecked = s.ScreenshotClearAllFail;

            SaveScreenshotsOnErrorCheckbox.IsChecked = s.SaveScreenshotsCacheOnError;
            CacheDurationSecondsTextBox.Text = s.CacheDurationSeconds.ToString();
            CacheIntervalMsTextBox.Text = s.CacheIntervalMs.ToString();
            CacheImageQualityTextBox.Text = s.CacheImageQuality.ToString();

            BuildUserControl[] controls = [B1, B2, B3, B4, B5];

            for (int i = 0; i < 5; i++)
            {
                controls[i].SetBuildSettings(s.Build[i]);
            }

        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {

            ClickerSettings settings = GetClickerSettings();

            var dialog = new Microsoft.Win32.SaveFileDialog
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
            var dialog = new Microsoft.Win32.OpenFileDialog
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
            DirectoryInfo dirInfo = new DirectoryInfo(appDirectory);
            Process.Start("explorer.exe", dirInfo.Parent == null ? appDirectory : $"/select,\"{appDirectory}\"");
        }
        private void OpenLogFile_Click(object sender, RoutedEventArgs e)
        {
            CreateAndOpen(Cst.LOG_FILE_PATH);
        }
        private void OpenLog30Crystals_Click(object sender, RoutedEventArgs e)
        {
            CreateAndOpen(Cst.CRYSTALS_COLLECTED_TIME_FILE_PATH);
        }
        private void OpenCaptchaLog_Click(object sender, RoutedEventArgs e)
        {
            CreateAndOpen(Cst.CAPTCHA_LOG_FILE_PATH);
        }
        private void OpenDungeonStatistics_Click(object sender, RoutedEventArgs e)
        {
            CreateAndOpen(Cst.DUNGEON_STATISTICS_PATH);
        }
        private void OpenManual_Click(object sender, RoutedEventArgs e)
        {
            string fullPath = Path.GetFullPath(Cst.MANUAL_FILE_PATH);
            if (!File.Exists(fullPath))
            {
                System.Windows.MessageBox.Show($"Guide file doesn't exist:\n{fullPath}", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
        }

        private void CreateAndOpen(string path)
        {
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }

        private void OpenScreenshotsInExplorer_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Utils.GetFullPathAndCreate(Cst.SCREENSHOT_PATH));
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
            if (tabIndex >= 0)
            {
                MyTabControl.SelectedIndex = tabIndex + 1;
            }
        }

        private void SetPosButtonClick(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = WinAPI.FindWindow(null!, WindowName.Text);
            if (hwnd != IntPtr.Zero)
            {
                SetDefaultNoxState(hwnd);
                WinAPI.RestoreWindow(hwnd);
                WinAPI.SetWindowPos(hwnd, hwnd, 0, 0, Cst.WINDOW_WIDTH + 1, Cst.WINDOW_HEIGHT + 1, WinAPI.SWP_NOZORDER);
                SetDefaultNoxState(hwnd);
            }
            else
            {
                WinAPI.ForceBringWindowToFront(this);
                System.Windows.MessageBox.Show($"Can't find window: {WindowName.Text}", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
        }

    }
}