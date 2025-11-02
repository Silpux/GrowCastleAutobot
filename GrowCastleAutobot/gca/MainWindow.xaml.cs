using gca.Classes;
using gca.Classes.SettingsScripts;
using gca.Classes.Tooltips;
using gca.Enums;
using gca.Script;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace gca
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool openToRewrite;

        private List<System.Windows.Controls.CheckBox> allCheckboxes = new List<System.Windows.Controls.CheckBox>();
        private List<System.Windows.Controls.TextBox> allTextBoxes = new List<System.Windows.Controls.TextBox>();
        private List<System.Windows.Controls.ComboBox> allComboBoxes = new List<System.Windows.Controls.ComboBox>();
        private List<System.Windows.Controls.RadioButton> allRadioButtons = new List<System.Windows.Controls.RadioButton>();

        private bool isSwappingWbbuc = false;
        private int swapWbbucAnimationDuration = 250;

        private NotifyIcon trayIcon;
        private HotkeyManager hotkeyManager;

        private MediaPlayer mediaPlayer = new MediaPlayer();

        Autobot autobot;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closed += OnClosed;

            autobot = new Autobot();

            autobot.OnStarted += SetRunningUI;
            autobot.OnStopRequested += SetStopRequestedUI;
            autobot.OnStopped += SetStoppedUI;
            autobot.OnPaused += SetPausedUI;
            autobot.OnPauseRequested += SetPauseRequestedUI;
            autobot.OnShowBalloon += ShowBalloon;

            autobot.OnInitFailed += ShowInitFailed;

            autobot.OnPlayAudio += PlayAudio;

            autobot.OnSwitchFromReplaysToDungeons += SwitchFromReplaysToDungeons;
            autobot.OnSwitchFromDungeonsToReplays += SwitchFromDungeonsToReplays;

            autobot.OnScriptError += OnScriptError;

            autobot.OnShowCrystalsCountResultLabel += ShowCrystalsCountResultLabel;

            autobot.OnShowNextRestartLabel += ShowNextRestartLabel;
            autobot.OnShowNextCleanupLabel += ShowNextCleanupLabel;

            autobot.OnDisableTowerUpgrade += DisableTowerUpgrade;

            autobot.OnABLabelUpdate += ABLabelUpdate;

            autobot.OnDungeonKillSpeedUpdate += DungeonKillSpeedUpdate;

            autobot.OnDisableSkipWithOranges += DisableSkipWithOranges;

            autobot.OnDisableAdForSpeed += DisableAdForSpeed;

            autobot.OnChangeBackground += ChangeBackground;

            autobot.OnInfoLabelUpdate += InfoLabelUpdate;

            autobot.OnCrystalsCountTestLabelUpdate += CrystalsCountTestLabelUpdate;

            autobot.OnRestartTestLabelUpdate += RestartTestLabelUpdate;
            autobot.OnUpgradeTestLabelUpdate += UpgradeTestLabelUpdate;

            autobot.OnGameStatusLabelUpdate += GameStatusLabelUpdate;
            autobot.OnCaptchaTestLabelUpdate += CaptchaTestLabelUpdate;
            autobot.OnOnlineActionsTestLabelUpdate += OnlineActionsTestLabelUpdate;

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

        private void OnlineActionsTestLabelUpdate(string str)
        {
            Dispatcher.Invoke(() =>
            {
                OnlineActionsTestStatusLabel.Content = str;
            });
        }

        private void CaptchaTestLabelUpdate(string str)
        {
            Dispatcher.Invoke(() =>
            {
                SolveCaptchaTestLabel.Content = str;
            });
        }

        private void GameStatusLabelUpdate(string str)
        {
            Dispatcher.Invoke(() =>
            {
                GameStatusTestLabel.Content = str;
            });
        }

        private void UpgradeTestLabelUpdate(string str)
        {
            Dispatcher.Invoke(() =>
            {
                UpgradeTestLabel.Content = str;
            });
        }

        private void RestartTestLabelUpdate(string str)
        {
            Dispatcher.Invoke(() =>
            {
                RestartTestLabel.Content = str;
            });
        }

        private void CrystalsCountTestLabelUpdate(string str)
        {
            Dispatcher.Invoke(() =>
            {
                CrystalsCountLabel.Content = str;
            });
        }

        private void InfoLabelUpdate(string value)
        {
            Dispatcher.Invoke(() =>
            {
                InfoLabel.Content = value;
            });
        }

        private void ChangeBackground(SolidColorBrush background)
        {
            SetBackground(background, true);
        }

        private void DisableAdForSpeed()
        {
            Dispatcher.Invoke(() =>
            {
                AdForSpeedCheckbox.Background = new SolidColorBrush(Colors.Red);
            });
        }

        private void DisableSkipWithOranges()
        {
            Dispatcher.Invoke(() =>
            {
                SkipWithOrangesCheckbox.Background = new SolidColorBrush(Colors.Red);
            });
        }

        private void DungeonKillSpeedUpdate(string label)
        {
            Dispatcher.Invoke(() =>
            {
                DungeonKillSpeedLabel.Content = label;
            });
        }

        private void ABLabelUpdate(string value)
        {
            Dispatcher.Invoke(() =>
            {
                ABTimerLabel.Content = value;
            });
        }

        private void DisableTowerUpgrade()
        {
            Dispatcher.Invoke(() =>
            {
                UpgradeCastleCheckbox.Background = new SolidColorBrush(Colors.Red);
            });
        }

        private void ShowNextCleanupLabel(DateTime dt)
        {
            Dispatcher.Invoke(() =>
            {
                NextCleanupTimeLabel.Content = $"Next cleanup: {dt:dd.MM.yyyy HH:mm:ss}";
            });
        }

        private void ShowNextRestartLabel(DateTime dt)
        {
            Dispatcher.Invoke(() =>
            {
                NextRestartTimeLabel.Content = $"Next restart: {dt:dd.MM.yyyy HH:mm:ss}";
            });
        }

        private void ShowCrystalsCountResultLabel(string label)
        {
            Dispatcher.Invoke(() =>
            {
                CrystalsCountLabel.Content = label;
            });
        }

        private void OnScriptError()
        {
            Dispatcher.Invoke(() =>
            {
                MyTabControl.Background = Cst.ErrorBackgrounColor;
            });
        }

        private void ShowInitFailed(string message)
        {
            WinAPI.ForceBringWindowToFront(this);
            System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            hotkeyManager = new(this);

            hotkeyManager.OnHotkeyFailed += () =>
            {
                WinAPI.ForceBringWindowToFront(this);
                System.Windows.MessageBox.Show("Failed to register hotkey. Choose another, this may be in use already", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            };

            hotkeyManager.OnHotkeyChanged += (string shortcut, Hotkey hotkey) =>
            {
                if (hotkey == Hotkey.Start)
                {
                    StartClickerShortcutBox.Text = shortcut;
                }
                else if (hotkey == Hotkey.Stop)
                {
                    StopClickerShortcutBox.Text = shortcut;
                }
                RewriteCurrentSettings();
                UpdateThreadStatusShortcutLabel();
            };

            hotkeyManager.OnStartHotkeyPressed += StartAutobot;
            hotkeyManager.OnStopHotkeyPressed += autobot.OnStopHotkey;

            if (Settings.Default.WindowTop >= 0 && Settings.Default.WindowLeft >= 0)
            {
                this.Top = Settings.Default.WindowTop;
                this.Left = Settings.Default.WindowLeft;
            }

            hotkeyManager.SaveShortcut(StartClickerShortcutBox.Text, Hotkey.Start);
            hotkeyManager.SaveShortcut(StopClickerShortcutBox.Text, Hotkey.Stop);
        }

        private void UpdateThreadStatusShortcutLabel()
        {
            if (!autobot.IsActive)
            {
                ThreadStatusShortcutLabel.Content = $"To start: {StartClickerShortcutBox.Text}";
                return;
            }
            if (autobot.IsRunning)
            {
                ThreadStatusShortcutLabel.Content = $"To stop: {StopClickerShortcutBox.Text}";
                return;
            }
            ThreadStatusShortcutLabel.Content = string.Empty;
        }
        public void PlayAudio(string path, double vol)
        {
            string pathToFile = Utils.FindFile(path);
            if (pathToFile is not null)
            {
                mediaPlayer.Volume = vol * vol;
                mediaPlayer.Open(new Uri($"file:///{pathToFile.Replace("\\", "/")}"));
                mediaPlayer.Play();
            }
            else
            {
                Log.E($"File \"{Path.GetFullPath(path)}\" doesn't exist");
            }
        }

        private void StartAutobot(TestMode testMode = TestMode.None)
        {
            IEnumerable<WaitBetweenBattlesUserControl> waitBetweenBattlesUserControls = GetWaitBetweenBattlesUserControls();
            string windowName = WindowName.Text;

            BuildUserControl build = BuildToPlayComboBox.SelectedIndex switch
            {
                0 => B1,
                1 => B2,
                2 => B3,
                3 => B4,
                4 => B5,
                _ => null!
            };

            autobot.Init(windowName, waitBetweenBattlesUserControls, build);
            autobot.Start(testMode);
        }

        public void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Log.U($"Start button click");
            StartAutobot();
        }

        public void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Log.U($"Stop button click");
            autobot.Stop();
        }

        private void SetRunningUI(bool notificationOnlyMode)
        {
            Dispatcher.BeginInvoke(() =>
            {
                ((System.Windows.Controls.Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Pause.png", UriKind.Relative));
                StopButton.IsEnabled = true;
                StartButton.IsEnabled = true;
                ThreadStatusLabel.Content = $"Running";
                ThreadStatusShortcutLabel.Content = $"To stop: {StopClickerShortcutBox.Text}";
                ThreadStatusLabel.Foreground = System.Windows.Media.Brushes.Green;
                SetCanvasChildrenState(TestCanvas, false);
                SetCanvasChildrenState(OnlineActionsTestCanvas, false);

                if (notificationOnlyMode)
                {
                    SetBackground(Cst.NotificationOnlyModeBackground, false);
                }
                else
                {
                    SetBackground(Cst.RunningBackground, false);
                }
                AddWaitBetweenBattlesButton.IsEnabled = false;
                EnableAllWaitsBetweenBattlesButton.IsEnabled = false;
                DisableAllWaitsBetweenBattlesButton.IsEnabled = false;
                SaveSettingsButton.IsEnabled = false;
                LoadSettingsButton.IsEnabled = false;

                foreach (var wbbuc in GetWaitBetweenBattlesUserControls())
                {
                    wbbuc.DisableUI();
                }
            });
        }

        private void SetStopRequestedUI()
        {
            Dispatcher.BeginInvoke(() =>
            {
                ((System.Windows.Controls.Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Start.png", UriKind.Relative));
                StopButton.IsEnabled = false;
                StartButton.IsEnabled = false;
                ThreadStatusLabel.Content = $"Stop requested";
                ThreadStatusShortcutLabel.Content = string.Empty;
                ThreadStatusLabel.Foreground = System.Windows.Media.Brushes.Red;
                SetBackground(Cst.StopRequestedBackground, false);
            });
        }

        private void SetStoppedUI()
        {
            Dispatcher.BeginInvoke(() =>
            {
                ((System.Windows.Controls.Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Start.png", UriKind.Relative));
                StopButton.IsEnabled = false;
                StartButton.IsEnabled = true;
                ThreadStatusLabel.Content = $"Stopped";
                ThreadStatusShortcutLabel.Content = $"To start: {StartClickerShortcutBox.Text}";
                ThreadStatusLabel.Foreground = System.Windows.Media.Brushes.Black;
                ABTimerLabel.Content = string.Empty;
                NextCleanupTimeLabel.Content = string.Empty;
                NextRestartTimeLabel.Content = string.Empty;
                ResetColors();
                SetCanvasChildrenState(TestCanvas, true);
                SetCanvasChildrenState(OnlineActionsTestCanvas, true);

                SetBackground(Cst.DefaultBackground, false);
                AddWaitBetweenBattlesButton.IsEnabled = true;
                EnableAllWaitsBetweenBattlesButton.IsEnabled = true;
                DisableAllWaitsBetweenBattlesButton.IsEnabled = true;
                SaveSettingsButton.IsEnabled = true;
                LoadSettingsButton.IsEnabled = true;

                foreach (var wbbuc in GetWaitBetweenBattlesUserControls())
                {
                    wbbuc.EnableUI();
                }
            });
        }

        public void SetPauseRequestedUI()
        {
            ((System.Windows.Controls.Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Continue.png", UriKind.Relative));
            ThreadStatusLabel.Content = $"Pause requested";
            ThreadStatusShortcutLabel.Content = string.Empty;
            StopButton.IsEnabled = true;
            StartButton.IsEnabled = false;
            ThreadStatusLabel.Foreground = System.Windows.Media.Brushes.Red;

            SetBackground(Cst.PauseRequestedBackground, false);
        }

        public void SetPausedUI()
        {
            Dispatcher.BeginInvoke(() =>
            {
                ((System.Windows.Controls.Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Continue.png", UriKind.Relative));
                ThreadStatusLabel.Content = $"Paused";
                ThreadStatusShortcutLabel.Content = string.Empty;
                StopButton.IsEnabled = true;
                StartButton.IsEnabled = true;
                ThreadStatusLabel.Foreground = System.Windows.Media.Brushes.Orange;

                SetBackground(Cst.PausedBackground, false);
            });
        }


        private void OnClosed(object? sender, EventArgs e)
        {
            autobot.OnStopHotkey();
            hotkeyManager.Close();
            Settings.Default.WindowTop = this.Top;
            Settings.Default.WindowLeft = this.Left;
            Settings.Default.Save();

            if (trayIcon != null)
            {
                trayIcon.Visible = false;
                trayIcon.Dispose();
            }

            Log.U("App closed");
        }


        private void StartClickerShortcutBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            hotkeyManager.StartListeningFor(Hotkey.Start, StartClickerShortcutBox);
            e.Handled = true;
        }

        private void StartClickerShortcutBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (hotkeyManager.ListeningFor == Hotkey.Start)
            {
                hotkeyManager.SaveShortcut(HotkeyManager.DEFAULT_START_HOTKEY, Hotkey.Start);
                hotkeyManager.StopListening();
            }
        }

        private void StartClickerShortcutBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            hotkeyManager.HandleKeyInput(StartClickerShortcutBox, e);
        }

        private void StopClickerShortcutBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            hotkeyManager.StartListeningFor(Hotkey.Stop, StopClickerShortcutBox);
            e.Handled = true;
        }

        private void StopClickerShortcutBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (hotkeyManager.ListeningFor == Hotkey.Stop)
            {
                hotkeyManager.SaveShortcut(HotkeyManager.DEFAULT_STOP_HOTKEY, Hotkey.Stop);
                hotkeyManager.StopListening();
            }
        }

        private void StopClickerShortcutBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            hotkeyManager.HandleKeyInput(StopClickerShortcutBox, e);
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
            if (autobot is null || !autobot.IsActive)
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

        private void SwitchFromReplaysToDungeons()
        {
            Dispatcher.Invoke(() =>
            {
                FarmDungeonCheckbox.Background = new SolidColorBrush(Colors.White);
                ReplaysCheckbox.Background = new SolidColorBrush(Colors.White);
            });
        }

        private void SwitchFromDungeonsToReplays()
        {
            Dispatcher.Invoke(() =>
            {
                FarmDungeonCheckbox.Background = new SolidColorBrush(Colors.Red);
                ReplaysCheckbox.Background = new SolidColorBrush(Colors.Lime);
            });
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

            s.InfiniteAB = InfiniteABCheckbox.IsChecked == true;

            ParseIntOrDefault(TimeToBreakABMinTextBox, n => s.TimeToBreakABMin = n, nameof(s.TimeToBreakABMin), throwIfError);
            ParseIntOrDefault(TimeToBreakABMaxTextBox, n => s.TimeToBreakABMax = n, nameof(s.TimeToBreakABMax), throwIfError);

            s.TryToSkipEveryBattle = TryToSkipEveryBattleCheckbox.IsChecked == true;

            ParseIntOrDefault(TimeBetweenSkipsMinTextBox, n => s.TimeBetweenSkipsMin = n, nameof(s.TimeBetweenSkipsMin), throwIfError);
            ParseIntOrDefault(TimeBetweenSkipsMaxTextBox, n => s.TimeBetweenSkipsMax = n, nameof(s.TimeBetweenSkipsMax), throwIfError);

            ParseIntOrDefault(BattlesWithSkipsMinTextBox, n => s.BattlesWithSkipsMin = n, nameof(s.BattlesWithSkipsMin), throwIfError);
            ParseIntOrDefault(BattlesWithSkipsMaxTextBox, n => s.BattlesWithSkipsMax = n, nameof(s.BattlesWithSkipsMax), throwIfError);

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

            ParseIntOrDefault(X1MouseMovementTestTextBox, n => s.TestMouseMovementX1 = n, nameof(s.TestMouseMovementX1), throwIfError);
            ParseIntOrDefault(X2MouseMovementTestTextBox, n => s.TestMouseMovementX2 = n, nameof(s.TestMouseMovementX2), throwIfError);
            ParseIntOrDefault(Y1MouseMovementTestTextBox, n => s.TestMouseMovementY1 = n, nameof(s.TestMouseMovementY1), throwIfError);
            ParseIntOrDefault(Y2MouseMovementTestTextBox, n => s.TestMouseMovementY2 = n, nameof(s.TestMouseMovementY2), throwIfError);

            s.TestCrystalsCountDarkMode = DarkModeCrystalsCountTestCheckBox.IsChecked == true;

            s.OnlineActionsTest_OpenGuildTest = OpenGuildTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_OpenRandomProfileFromGuildTest = OpenRandomProfileFromGuildTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_OpenGuildsChatTest = OpenGuildsChatTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_OpenGuildsTopTest = OpenGuildsTopTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_OpenTopTest = OpenTopTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_OpenTopSeasonTest = OpenTopSeasonTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_OpenHellSeasonMyTest = OpenHellSeasonMyTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_OpenHellSeasonTest = OpenHellSeasonTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_OpenWavesTopMyTest = OpenWavesTopMyTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_OpenWavesTopTest = OpenWavesTopTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_CraftStonesTest = CraftStonesTestCheckbox.IsChecked == true;
            s.OnlineActionsTest_DoSaveTest = DoSaveTestCheckbox.IsChecked == true;

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

            InfiniteABCheckbox.IsChecked = s.InfiniteAB;

            TimeToBreakABMinTextBox.Text = s.TimeToBreakABMin.ToString();
            TimeToBreakABMaxTextBox.Text = s.TimeToBreakABMax.ToString();

            TryToSkipEveryBattleCheckbox.IsChecked = s.TryToSkipEveryBattle;

            TimeBetweenSkipsMinTextBox.Text = s.TimeBetweenSkipsMin.ToString();
            TimeBetweenSkipsMaxTextBox.Text = s.TimeBetweenSkipsMax.ToString();

            BattlesWithSkipsMinTextBox.Text = s.BattlesWithSkipsMin.ToString();
            BattlesWithSkipsMaxTextBox.Text = s.BattlesWithSkipsMax.ToString();

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
                Utils.SetDefaultNoxState(hwnd);
                WinAPI.RestoreWindow(hwnd);
                WinAPI.SetWindowPos(hwnd, hwnd, 0, 0, Cst.WINDOW_WIDTH + 1, Cst.WINDOW_HEIGHT + 1, WinAPI.SWP_NOZORDER);
                Utils.SetDefaultNoxState(hwnd);
            }
            else
            {
                WinAPI.ForceBringWindowToFront(this);
                System.Windows.MessageBox.Show($"Can't find window: {WindowName.Text}", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
        }

    }
}