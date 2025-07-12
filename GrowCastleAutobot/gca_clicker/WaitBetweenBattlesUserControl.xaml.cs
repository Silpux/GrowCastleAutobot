using gca_clicker.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        public event Action<WaitBetweenBattlesUserControl> OnRemove = null!;

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

        public WaitBetweenBattlesUserControl()
        {
            InitializeComponent();

            SetChecked(true);
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


        public WaitBetweenBattleSetting GetSetting(bool throwIfError)
        {

            int triggerMin = 0;
            int triggerMax = 0;

            int waitMin = 0;
            int waitMax = 0;

            if(!int.TryParse(MinTriggerSecTextBox.Text, out triggerMin) && throwIfError)
            {
                throw new ArgumentException(nameof(triggerMin));
            }

            if (!int.TryParse(MaxTriggerSecTextBox.Text, out triggerMax) && throwIfError)
            {
                throw new ArgumentException(nameof(triggerMax));
            }

            if(throwIfError && triggerMin > triggerMax)
            {
                throw new ArgumentException($"{nameof(triggerMin)} > {nameof(triggerMax)}");
            }

            if (!int.TryParse(MinWaitSecTextBox.Text, out waitMin) && throwIfError)
            {
                throw new ArgumentException(nameof(waitMin));
            }

            if (!int.TryParse(MaxWaitSecTextBox.Text, out waitMax) && throwIfError)
            {
                throw new ArgumentException(nameof(waitMax));
            }

            if (throwIfError && waitMin > waitMax)
            {
                throw new ArgumentException($"{nameof(waitMin)} > {nameof(waitMax)}");
            }

            return new WaitBetweenBattleSetting()
            {
                TriggerMin = triggerMin,
                TriggerMax = triggerMax,
                WaitMin = waitMin,
                WaitMax = waitMax,
                IsChecked = EnableCheckbox.IsChecked == true
            };

        }

        public void SetFromSettings(WaitBetweenBattleSetting settings)
        {

            EnableCheckbox.IsChecked = settings.IsChecked;

            MinTriggerSecTextBox.Text = settings.TriggerMin.ToString();
            MaxTriggerSecTextBox.Text = settings.TriggerMax.ToString();

            MinWaitSecTextBox.Text = settings.WaitMin.ToString();
            MaxWaitSecTextBox.Text = settings.WaitMax.ToString();

        }
    }
}
