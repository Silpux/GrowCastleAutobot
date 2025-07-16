using gca_clicker.Classes.SettingsScripts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public event Action<WaitBetweenBattlesUserControl> OnRemove = null!;

        private static readonly string timeLeftFormat = "hh\\:mm\\:ss\\:fff";

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

        public WaitBetweenBattlesUserControl()
        {
            InitializeComponent();

            SetChecked(true);
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
            });
        }

        public void ResetUI()
        {
            Dispatcher.Invoke(() =>
            {
                StatusLabel.Content = $"";
                TimeLeftLabel.Content = $"";
                ContainerBorder.Background = defaultColor;
            });
        }

        public WaitBetweenBattlesSetting GetSetting(bool throwIfError)
        {

            int triggerMin = 0;
            int triggerMax = 0;

            int waitMin = 0;
            int waitMax = 0;

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

            return new WaitBetweenBattlesSetting()
            {
                IsChecked = EnableCheckbox.IsChecked == true,
                TriggerMin = triggerMin,
                TriggerMax = triggerMax,
                WaitMin = waitMin,
                WaitMax = waitMax,
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

        }
    }
}
