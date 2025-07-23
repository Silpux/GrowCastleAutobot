using gca_clicker.Classes.SettingsScripts;
using gca_clicker.Classes.Tooltips;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gca_clicker
{
    /// <summary>
    /// Interaction logic for WaitBetweenBattlesUserControl.xaml
    /// </summary>
    public partial class WaitBetweenBattlesUserControl : UserControl
    {

        private static readonly SolidColorBrush defaultColor = Brushes.White;
        private static readonly SolidColorBrush runningColor = new SolidColorBrush(Color.FromRgb(149, 255, 128));
        private static readonly SolidColorBrush suspendedColor = new SolidColorBrush(Color.FromRgb(255, 255, 128));
        private static readonly SolidColorBrush elapsedColor = new SolidColorBrush(Color.FromRgb(255, 128, 128));
        private static readonly SolidColorBrush activeWaitColor = new SolidColorBrush(Color.FromRgb(128, 202, 255));
        private static readonly SolidColorBrush ignoredWaitColor = new SolidColorBrush(Color.FromRgb(255, 181, 128));
        private static readonly SolidColorBrush performingOnlineActionsColor = new SolidColorBrush(Color.FromRgb(128, 128, 255));

        public event Action<WaitBetweenBattlesUserControl> OnRemove = null!;

        private static readonly string timeLeftFormat = "hh\\:mm\\:ss\\:ff";

        public event Action<object> OnUpdate = null!;

        private int number;
        public int Number
        {
            get => number;
            set
            {
                number = value;
                EnableCheckbox.Content = $"Wait {value}";
            }
        }

        private bool ignoreWait;
        public bool IgnoreWait => ignoreWait;

        public bool IsChecked => EnableCheckbox.IsChecked == true;

        private ScrollViewer scrollViewerContainer;

        private List<CheckBox> allCheckboxes = new List<CheckBox>();
        private List<TextBox> allTextBoxes = new List<TextBox>();

        public WaitBetweenBattlesUserControl(ScrollViewer scrollViewer)
        {
            InitializeComponent();

            SetChecked(true);

            CollectUIObjects(this);

            scrollViewerContainer = scrollViewer;
            scrollViewerContainer.ScrollChanged += ContainerScrollViewerScrolled;

            OnlineActionsPopup.Opened += (s, e) =>
            {
                if (OnlineActionsPopup.Child is FrameworkElement child)
                {
                    TooltipHelper.AttachTooltipCursorWatcherTo(child);
                }
            };
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
                else if (child is DependencyObject depChild)
                {
                    CollectUIObjects(depChild);
                }
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
        }


        public void ContainerScrollViewerScrolled(object sender, ScrollChangedEventArgs e)
        {
            if(e.VerticalChange != 0 && OnlineActionsPopup.IsOpen)
            {
                double offset = OnlineActionsPopup.HorizontalOffset;
                OnlineActionsPopup.HorizontalOffset = offset + 0.01;
                OnlineActionsPopup.HorizontalOffset = offset;
            }
        }

        public void DisableUI()
        {
            EnableCheckbox.IsEnabled = false;
            RemoveButton.IsEnabled = false;
        }

        public void EnableUI()
        {
            EnableCheckbox.IsEnabled = true;
            RemoveButton.IsEnabled = true;
        }

        public void SetIgnoredWaitState(bool ignore)
        {
            WaitIgnoredLabel.Visibility = ignore ? Visibility.Visible : Visibility.Collapsed;
            ignoreWait = ignore;
        }

        public void SetChecked(bool state)
        {
            EnableCheckbox.IsChecked = state;
            SetActiveLines(!state);
        }

        private void SetActiveLines(bool state)
        {
            if (state)
            {
                RightLine.Visibility = Visibility.Visible;
                LeftLine.Visibility = Visibility.Visible;
            }
            else
            {
                RightLine.Visibility = Visibility.Collapsed;
                LeftLine.Visibility = Visibility.Collapsed;
            }
        }

        private void TextBox_RewriteSettings(object sender, TextChangedEventArgs e)
        {
            OnUpdate?.Invoke(sender);
        }

        private void TextBox_Insert0OnError(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (!int.TryParse(tb.Text, out _))
                {
                    tb.Text = "0";
                }
            }
        }



        private void EnableCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OnUpdate?.Invoke(sender);
            SetActiveLines(false);
        }

        private void EnableCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OnUpdate?.Invoke(sender);
            SetActiveLines(true);
        }


        private void OpenGuildCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OpenGuildChanceLabel.IsEnabled = true;
            OpenGuildChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void OpenGuildCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenGuildChanceLabel.IsEnabled = false;
            OpenGuildChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }
        private void OpenGuildsTopCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OpenGuildsTopChanceLabel.IsEnabled = true;
            OpenGuildsTopChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void OpenGuildsTopCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenGuildsTopChanceLabel.IsEnabled = false;
            OpenGuildsTopChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }

        private void OpenGuildsChatCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OpenGuildsChatChanceLabel.IsEnabled = true;
            OpenGuildsChatChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void OpenGuildsChatCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenGuildsChatChanceLabel.IsEnabled = false;
            OpenGuildsChatChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }


        private void OpenRandomProfileFromGuildCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OpenRandomProfileFromGuildChanceLabel.IsEnabled = true;
            OpenRandomProfileFromGuildChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void OpenRandomProfileFromGuildCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenRandomProfileFromGuildChanceLabel.IsEnabled = false;
            OpenRandomProfileFromGuildChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }
        private void OpenTopCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OpenTopChanceLabel.IsEnabled = true;
            OpenTopChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void OpenTopCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenTopChanceLabel.IsEnabled = false;
            OpenTopChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }
        private void OpenTopSeasonCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OpenTopSeasonChanceLabel.IsEnabled = true;
            OpenTopSeasonChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void OpenTopSeasonCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenTopSeasonChanceLabel.IsEnabled = false;
            OpenTopSeasonChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }
        private void OpenHellSeasonCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OpenHellSeasonChanceLabel.IsEnabled = true;
            OpenHellSeasonChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void OpenHellSeasonCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenHellSeasonChanceLabel.IsEnabled = false;
            OpenHellSeasonChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }
        private void OpenHellSeasonMyCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OpenHellSeasonMyChanceLabel.IsEnabled = true;
            OpenHellSeasonMyChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void OpenHellSeasonMyCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenHellSeasonMyChanceLabel.IsEnabled = false;
            OpenHellSeasonMyChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }
        private void OpenWavesTopCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OpenWavesTopChanceLabel.IsEnabled = true;
            OpenWavesTopChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void OpenWavesTopCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenWavesTopChanceLabel.IsEnabled = false;
            OpenWavesTopChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }
        private void OpenWavesTopMyCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OpenWavesTopMyChanceLabel.IsEnabled = true;
            OpenWavesTopMyChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void OpenWavesTopMyCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            OpenWavesTopMyChanceLabel.IsEnabled = false;
            OpenWavesTopMyChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }
        private void CraftStonesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CraftStonesChanceLabel.IsEnabled = true;
            CraftStonesChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void CraftStonesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CraftStonesChanceLabel.IsEnabled = false;
            CraftStonesChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }
        private void DoSaveCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            DoSaveChanceLabel.IsEnabled = true;
            DoSaveChanceTextBox.IsEnabled = true;
            OnUpdate?.Invoke(sender);
        }

        private void DoSaveCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            DoSaveChanceLabel.IsEnabled = false;
            DoSaveChanceTextBox.IsEnabled = false;
            OnUpdate?.Invoke(sender);
        }

        private void TimeOfOnlineActionCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            OnUpdate?.Invoke(sender);
        }

        private void TimeOfOnlineActionCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBoxes = new[] { BeforeWaitCheckbox, AfterWaitCheckbox};
            int checkedCount = checkBoxes.Count(cb => cb.IsChecked == true);

            if(checkedCount == 0)
            {
                ((CheckBox)sender).IsChecked = true;
            }
            else
            {
                OnUpdate?.Invoke(sender);
            }

        }


        private bool IsTextNumeric(string text)
        {
            return int.TryParse(text, out _);
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




        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            scrollViewerContainer.ScrollChanged -= ContainerScrollViewerScrolled;
            OnRemove?.Invoke(this);
        }


        public void SetRunningUI()
        {
            Dispatcher.Invoke(() =>
            {
                StatusLabel.Content = $"Running";
                ContainerBorder.Background = runningColor;
            });
        }

        public void SetTriggerTimeLeft(TimeSpan time)
        {
            Dispatcher.Invoke(() =>
            {
                TimeLeftLabel.Content = $"{time.ToString(timeLeftFormat)}";
            });
        }

        public void SetWaitingTimeLeft(TimeSpan waitingTime)
        {
            Dispatcher.Invoke(() =>
            {
                TimeLeftLabel.Content = $"{waitingTime.ToString(timeLeftFormat)}";
            });
        }

        public void SetActiveWaitUI()
        {
            Dispatcher.Invoke(() =>
            {
                StatusLabel.Content = $"Waiting...";
                ContainerBorder.Background = activeWaitColor;
            });
        }

        public void SetIgnoredWaitUI()
        {
            Dispatcher.Invoke(() =>
            {
                StatusLabel.Content = $"Ignore wait";
                ContainerBorder.Background = ignoredWaitColor;
            });
        }

        public void SetSuspendedUI()
        {
            Dispatcher.Invoke(() =>
            {
                StatusLabel.Content = $"Suspended";
                ContainerBorder.Background = suspendedColor;
            });
        }

        public void SetOnlineActionsUI(string status, string actionsSequence)
        {
            Dispatcher.Invoke(() =>
            {
                string display = actionsSequence.Length == 0 ? "Nothing" : actionsSequence;
                TimeLeftLabel.Content = $"{display}";
                StatusLabel.Content = status;
                ContainerBorder.Background = performingOnlineActionsColor;
            });
        }

        public void SetElapsedUI()
        {
            Dispatcher.Invoke(() =>
            {
                StatusLabel.Content = $"Elapsed";
                TimeLeftLabel.Content = $"{TimeSpan.Zero.ToString(timeLeftFormat)}";
                ContainerBorder.Background = elapsedColor;
            });
        }
        public void ResetUIQueued()
        {
            Dispatcher.BeginInvoke(() =>
            {
                StatusLabel.Content = $"";
                TimeLeftLabel.Content = $"";
                ContainerBorder.Background = defaultColor;
                ResetColors();
            });
        }


        public void ResetUI()
        {
            Dispatcher.Invoke(() =>
            {
                StatusLabel.Content = $"";
                TimeLeftLabel.Content = $"";
                ContainerBorder.Background = defaultColor;
                ResetColors();
            });
        }

        public WaitBetweenBattlesSetting GetSetting(bool throwIfError)
        {

            int triggerMin = 0;
            int triggerMax = 0;

            int waitMin = 0;
            int waitMax = 0;

            int openGuildChance;
            int openGuildsTopChance;
            int openGuildsChatChance;
            int openRandomGuildProfileChance;
            int openTopChance;
            int openTopSeasonChance;
            int openHellTopSeasonChance;
            int openHellTopSeasonMyChance;
            int openTopWavesChance;
            int openTopWavesMyChance;
            int craftStonesChance;
            int doSaveChance;


            if(!int.TryParse(MinTriggerSecTextBox.Text, out triggerMin) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(triggerMin)} wrong value");
            }

            if (!int.TryParse(MaxTriggerSecTextBox.Text, out triggerMax) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(triggerMax)} wrong value");
            }

            if(throwIfError && triggerMin > triggerMax)
            {
                throw new ArgumentException($"Wait {number}: {nameof(triggerMin)} > {nameof(triggerMax)}");
            }

            if (!int.TryParse(MinWaitSecTextBox.Text, out waitMin) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(waitMin)} wrong value");
            }

            if (!int.TryParse(MaxWaitSecTextBox.Text, out waitMax) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(waitMax)} wrong value");
            }

            if (throwIfError && waitMin > waitMax)
            {
                throw new ArgumentException($"Wait {number}: {nameof(waitMin)} > {nameof(waitMax)}");
            }


            if (!int.TryParse(OpenGuildChanceTextBox.Text, out openGuildChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(openGuildChance)} wrong value");
            }

            if (!int.TryParse(OpenGuildsTopChanceTextBox.Text, out openGuildsTopChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(openGuildsTopChance)} wrong value");
            }
            if (!int.TryParse(OpenGuildsChatChanceTextBox.Text, out openGuildsChatChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(openGuildsChatChance)} wrong value");
            }
            if (!int.TryParse(OpenRandomProfileFromGuildChanceTextBox.Text, out openRandomGuildProfileChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(openRandomGuildProfileChance)} wrong value");
            }
            if (!int.TryParse(OpenTopChanceTextBox.Text, out openTopChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(openTopChance)} wrong value");
            }
            if (!int.TryParse(OpenTopSeasonChanceTextBox.Text, out openTopSeasonChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(openTopSeasonChance)} wrong value");
            }
            if (!int.TryParse(OpenHellSeasonChanceTextBox.Text, out openHellTopSeasonChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(openHellTopSeasonChance)} wrong value");
            }
            if (!int.TryParse(OpenHellSeasonMyChanceTextBox.Text, out openHellTopSeasonMyChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(openHellTopSeasonMyChance)} wrong value");
            }
            if (!int.TryParse(OpenWavesTopChanceTextBox.Text, out openTopWavesChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(openTopWavesChance)} wrong value");
            }
            if (!int.TryParse(OpenWavesTopMyChanceTextBox.Text, out openTopWavesMyChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(openTopWavesMyChance)} wrong value");
            }
            if (!int.TryParse(CraftStonesChanceTextBox.Text, out craftStonesChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(craftStonesChance)} wrong value");
            }
            if (!int.TryParse(DoSaveChanceTextBox.Text, out doSaveChance) && throwIfError)
            {
                throw new ArgumentException($"Wait {number}: {nameof(doSaveChance)} wrong value");
            }

            return new WaitBetweenBattlesSetting()
            {
                IsChecked = EnableCheckbox.IsChecked == true,
                TriggerMin = triggerMin,
                TriggerMax = triggerMax,
                WaitMin = waitMin,
                WaitMax = waitMax,

                OpenGuild = OpenGuildCheckbox.IsChecked == true,
                OpenGuildChance = openGuildChance,

                OpenGuildsTop = OpenGuildsTopCheckbox.IsChecked == true,
                OpenGuildsTopChance = openGuildsTopChance,

                OpenGuildsChat = OpenGuildsChatCheckbox.IsChecked == true,
                OpenGuildsChatChance = openGuildsChatChance,

                OpenRandomProfileInGuild = OpenRandomProfileFromGuildCheckbox.IsChecked == true,
                OpenRandomProfileInGuildChance = openRandomGuildProfileChance,

                OpenTop = OpenTopCheckbox.IsChecked == true,
                OpenTopChance = openTopChance,

                OpenTopSeason = OpenTopSeasonCheckbox.IsChecked == true,
                OpenTopSeasonChance = openTopSeasonChance,

                OpenTopHellSeason = OpenHellSeasonCheckbox.IsChecked == true,
                OpenTopHellSeasonChance = openHellTopSeasonChance,

                OpenTopHellSeasonMy = OpenHellSeasonMyCheckbox.IsChecked == true,
                OpenTopHellSeasonMyChance = openHellTopSeasonMyChance,

                OpenTopWavesOverall = OpenWavesTopCheckbox.IsChecked == true,
                OpenTopWavesOverallChance = openTopWavesChance,

                OpenTopWavesOverallMy = OpenWavesTopMyCheckbox.IsChecked == true,
                OpenTopWavesOverallMyChance = openTopWavesMyChance,

                CraftStones = CraftStonesCheckbox.IsChecked == true,
                CraftStonesChance = craftStonesChance,

                DoSave = DoSaveCheckbox.IsChecked == true,
                DoSaveChance = doSaveChance,

                BeforeWait = BeforeWaitCheckbox.IsChecked == true,
                AfterWait = AfterWaitCheckbox.IsChecked == true,

                UserControl = this
            };

        }

        public void SetFromSettings(WaitBetweenBattlesSetting settings)
        {

            EnableCheckbox.IsChecked = settings.IsChecked;

            MinTriggerSecTextBox.Text = settings.TriggerMin.ToString();
            MaxTriggerSecTextBox.Text = settings.TriggerMax.ToString();

            MinWaitSecTextBox.Text = settings.WaitMin.ToString();
            MaxWaitSecTextBox.Text = settings.WaitMax.ToString();

            OpenGuildCheckbox.IsChecked = settings.OpenGuild;
            OpenGuildChanceTextBox.Text = settings.OpenGuildChance.ToString();

            OpenGuildsTopCheckbox.IsChecked = settings.OpenGuildsTop;
            OpenGuildsTopChanceTextBox.Text = settings.OpenGuildsTopChance.ToString();

            OpenGuildsChatCheckbox.IsChecked = settings.OpenGuildsChat;
            OpenGuildsChatChanceTextBox.Text = settings.OpenGuildsChatChance.ToString();

            OpenRandomProfileFromGuildCheckbox.IsChecked = settings.OpenRandomProfileInGuild;
            OpenRandomProfileFromGuildChanceTextBox.Text = settings.OpenRandomProfileInGuildChance.ToString();

            OpenTopCheckbox.IsChecked = settings.OpenTop;
            OpenTopChanceTextBox.Text = settings.OpenTopChance.ToString();

            OpenTopSeasonCheckbox.IsChecked = settings.OpenTopSeason;
            OpenTopSeasonChanceTextBox.Text = settings.OpenTopSeasonChance.ToString();

            OpenHellSeasonCheckbox.IsChecked = settings.OpenTopHellSeason;
            OpenHellSeasonChanceTextBox.Text = settings.OpenTopHellSeasonChance.ToString();

            OpenHellSeasonMyCheckbox.IsChecked = settings.OpenTopHellSeasonMy;
            OpenHellSeasonMyChanceTextBox.Text = settings.OpenTopHellSeasonMyChance.ToString();

            OpenWavesTopCheckbox.IsChecked = settings.OpenTopWavesOverall;
            OpenWavesTopChanceTextBox.Text = settings.OpenTopWavesOverallChance.ToString();

            OpenWavesTopMyCheckbox.IsChecked = settings.OpenTopWavesOverallMy;
            OpenWavesTopMyChanceTextBox.Text = settings.OpenTopWavesOverallMyChance.ToString();

            CraftStonesCheckbox.IsChecked = settings.CraftStones;
            CraftStonesChanceTextBox.Text = settings.CraftStonesChance.ToString();

            bool beforeWait = settings.BeforeWait;
            bool afterWait = settings.AfterWait;

            if(!beforeWait && !afterWait)
            {
                beforeWait = true;
            }

            BeforeWaitCheckbox.IsChecked = beforeWait;
            AfterWaitCheckbox.IsChecked = afterWait;

            DoSaveCheckbox.IsChecked = settings.DoSave;
            DoSaveChanceTextBox.Text = settings.DoSaveChance.ToString();

        }
    }
}
