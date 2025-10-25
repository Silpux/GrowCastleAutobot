using gca.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace gca
{
    public partial class MainWindow : Window
    {

        private void SetActiveMatButtons(bool state)
        {
            MatLabel.IsEnabled = state;
            MatBCheckbox.IsEnabled = state;
            MatACheckbox.IsEnabled = state;
            MatSCheckbox.IsEnabled = state;
            MatLCheckbox.IsEnabled = state;
            MatECheckbox.IsEnabled = state;
        }

        private void SetMatAndDungeonButtonsState()
        {
            if (FarmDungeonCheckbox.IsChecked == true)
            {

                MatTimeLabel.IsEnabled = true;
                MatTimeMaxLabel.IsEnabled = true;
                MatTimeMinTextBox.IsEnabled = true;
                MatTimeMaxTextBox.IsEnabled = true;

                DungeonComboBox.IsEnabled = true;
                MakeReplaysIfDungeonDoesntLoadCheckBox.IsEnabled = true;
                MissclicksOnDungeonsCheckbox.IsEnabled = true;

                if (MissclicksOnDungeonsCheckbox.IsChecked == true)
                {
                    MissclickOnDungeonsChanceLabel.IsEnabled = true;
                    MissclickOnDungeonsChanceTextBox.IsEnabled = true;
                    MissclicksOnDungeonsIncludeDiagonalsCheckbox.IsEnabled = true;
                }
                else
                {
                    MissclickOnDungeonsChanceLabel.IsEnabled = false;
                    MissclickOnDungeonsChanceTextBox.IsEnabled = false;
                    MissclicksOnDungeonsIncludeDiagonalsCheckbox.IsEnabled = false;
                }

                if (DungeonComboBox.SelectedIndex > 5)
                {
                    CastOnBossCheckbox.IsEnabled = true;
                    if (CastOnBossCheckbox.IsChecked == true)
                    {
                        CastDelayInDungeonLabel.IsEnabled = true;
                        CastOnBossDelayTextBox.IsEnabled = true;
                    }
                    else
                    {
                        CastDelayInDungeonLabel.IsEnabled = false;
                        CastOnBossDelayTextBox.IsEnabled = false;
                    }
                    SetActiveMatButtons(false);
                }
                else
                {
                    CastOnBossCheckbox.IsEnabled = false;
                    CastDelayInDungeonLabel.IsEnabled = false;
                    CastOnBossDelayTextBox.IsEnabled = false;
                    SetActiveMatButtons(true);
                }
            }
            else
            {
                MatTimeLabel.IsEnabled = false;
                MatTimeMaxLabel.IsEnabled = false;
                MatTimeMinTextBox.IsEnabled = false;
                MatTimeMaxTextBox.IsEnabled = false;

                DungeonComboBox.IsEnabled = false;
                MakeReplaysIfDungeonDoesntLoadCheckBox.IsEnabled = false;
                CastDelayInDungeonLabel.IsEnabled = false;
                CastOnBossDelayTextBox.IsEnabled = false;
                CastOnBossCheckbox.IsEnabled = false;

                SetActiveMatButtons(false);

                MissclicksOnDungeonsCheckbox.IsEnabled = false;
                MissclickOnDungeonsChanceLabel.IsEnabled = false;
                MissclickOnDungeonsChanceTextBox.IsEnabled = false;
                MissclicksOnDungeonsIncludeDiagonalsCheckbox.IsEnabled = false;

            }
        }
        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is System.Windows.Controls.ComboBox combo && combo.Items.Count > 0)
            {
                int direction = e.Delta > 0 ? -1 : 1;
                int newIndex = combo.SelectedIndex + direction;

                if (newIndex < 0)
                    newIndex = 0;
                else if (newIndex >= combo.Items.Count)
                    newIndex = combo.Items.Count - 1;

                combo.SelectedIndex = newIndex;
                e.Handled = true;
            }
        }
        private void SetABParameters()
        {
            if (ABModeCheckbox.IsChecked == true || NotificationOnlyModeCheckbox.IsChecked == true)
            {
                if (ABModeCheckbox.IsChecked == true)
                {
                    GabRadioButton.IsEnabled = true;
                    TabRadioButton.IsEnabled = true;

                    DesktopNotificationOn30CrystalsCheckbox.IsEnabled = true;
                    PlayAudioOn30CrystalsCheckbox.IsEnabled = true;

                    BreakABOn30CrystalsCheckbox.IsEnabled = true;

                    if (PlayAudioOn30CrystalsCheckbox.IsChecked == true)
                    {

                        Audio1RadioButton.IsEnabled = true;
                        Audio2RadioButton.IsEnabled = true;

                        PlayAudio1_30CrystalsVolumeLabel.IsEnabled = Audio1RadioButton.IsChecked == true;
                        PlayAudio1_30CrystalsVolumeTextBox.IsEnabled = Audio1RadioButton.IsChecked == true;

                        PlayAudio2_30CrystalsVolumeLabel.IsEnabled = Audio2RadioButton.IsChecked == true;
                        PlayAudio2_30CrystalsVolumeTextBox.IsEnabled = Audio2RadioButton.IsChecked == true;

                    }

                    DesktopNotification30CrystalsIntervalLabel.IsEnabled = DesktopNotificationOn30CrystalsCheckbox.IsChecked == true;
                    DesktopNotification30CrystalsIntervalTextBox.IsEnabled = DesktopNotificationOn30CrystalsCheckbox.IsChecked == true;

                    TimeToBreakABLabel.IsEnabled = true;

                    TimeToBreakABMinLabel.IsEnabled = true;
                    TimeToBreakABMaxLabel.IsEnabled = true;

                    TimeToBreakABMinTextBox.IsEnabled = true;
                    TimeToBreakABMaxTextBox.IsEnabled = true;

                    SkipsBetweenABSessionsLabel.IsEnabled = SkipWavesCheckbox.IsChecked == true;

                    SkipsBetweenABSessionsMinLabel.IsEnabled = SkipWavesCheckbox.IsChecked == true;
                    SkipsBetweenABSessionsMaxLabel.IsEnabled = SkipWavesCheckbox.IsChecked == true;

                    SkipsBetweenABSessionsMinTextBox.IsEnabled = SkipWavesCheckbox.IsChecked == true;
                    SkipsBetweenABSessionsMaxTextBox.IsEnabled = SkipWavesCheckbox.IsChecked == true;

                }
                else
                {
                    GabRadioButton.IsEnabled = false;
                    TabRadioButton.IsEnabled = false;

                    DesktopNotificationOn30CrystalsCheckbox.IsEnabled = true;
                    PlayAudioOn30CrystalsCheckbox.IsEnabled = true;

                    BreakABOn30CrystalsCheckbox.IsEnabled = false;

                    TimeToBreakABLabel.IsEnabled = false;

                    TimeToBreakABMinLabel.IsEnabled = false;
                    TimeToBreakABMaxLabel.IsEnabled = false;

                    TimeToBreakABMinTextBox.IsEnabled = false;
                    TimeToBreakABMaxTextBox.IsEnabled = false;

                    SkipsBetweenABSessionsLabel.IsEnabled = false;

                    SkipsBetweenABSessionsMinLabel.IsEnabled = false;
                    SkipsBetweenABSessionsMaxLabel.IsEnabled = false;

                    SkipsBetweenABSessionsMinTextBox.IsEnabled = false;
                    SkipsBetweenABSessionsMaxTextBox.IsEnabled = false;

                    if (PlayAudioOn30CrystalsCheckbox.IsChecked == true)
                    {

                        Audio1RadioButton.IsEnabled = true;
                        Audio2RadioButton.IsEnabled = true;

                        PlayAudio1_30CrystalsVolumeLabel.IsEnabled = Audio1RadioButton.IsChecked == true;
                        PlayAudio1_30CrystalsVolumeTextBox.IsEnabled = Audio1RadioButton.IsChecked == true;

                        PlayAudio2_30CrystalsVolumeLabel.IsEnabled = Audio2RadioButton.IsChecked == true;
                        PlayAudio2_30CrystalsVolumeTextBox.IsEnabled = Audio2RadioButton.IsChecked == true;

                        DesktopNotification30CrystalsIntervalLabel.IsEnabled = DesktopNotificationOn30CrystalsCheckbox.IsChecked == true;
                        DesktopNotification30CrystalsIntervalTextBox.IsEnabled = DesktopNotificationOn30CrystalsCheckbox.IsChecked == true;

                    }

                }
            }
            else
            {
                GabRadioButton.IsEnabled = false;
                TabRadioButton.IsEnabled = false;

                DesktopNotificationOn30CrystalsCheckbox.IsEnabled = false;
                PlayAudioOn30CrystalsCheckbox.IsEnabled = false;

                BreakABOn30CrystalsCheckbox.IsEnabled = false;

                Audio1RadioButton.IsEnabled = false;
                Audio2RadioButton.IsEnabled = false;

                PlayAudio1_30CrystalsVolumeLabel.IsEnabled = false;
                PlayAudio1_30CrystalsVolumeTextBox.IsEnabled = false;

                PlayAudio2_30CrystalsVolumeLabel.IsEnabled = false;
                PlayAudio2_30CrystalsVolumeTextBox.IsEnabled = false;

                DesktopNotification30CrystalsIntervalLabel.IsEnabled = false;
                DesktopNotification30CrystalsIntervalTextBox.IsEnabled = false;

                TimeToBreakABLabel.IsEnabled = false;

                TimeToBreakABMinLabel.IsEnabled = false;
                TimeToBreakABMaxLabel.IsEnabled = false;

                TimeToBreakABMinTextBox.IsEnabled = false;
                TimeToBreakABMaxTextBox.IsEnabled = false;

                SkipsBetweenABSessionsLabel.IsEnabled = false;

                SkipsBetweenABSessionsMinLabel.IsEnabled = false;
                SkipsBetweenABSessionsMaxLabel.IsEnabled = false;

                SkipsBetweenABSessionsMinTextBox.IsEnabled = false;
                SkipsBetweenABSessionsMaxTextBox.IsEnabled = false;
            }
        }

        private void BuildToPlayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void FarmDungeonCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SetMatAndDungeonButtonsState();
            ABModeCheckbox.IsChecked = false;
            SkipWavesCheckbox.IsChecked = false;
            ReplaysCheckbox.IsChecked = false;
            NotificationOnlyModeCheckbox.IsChecked = false;
            RewriteCurrentSettings(sender);
        }

        private void FarmDungeonCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetMatAndDungeonButtonsState();
            DeathAltarCheckbox.IsChecked = false;
            RewriteCurrentSettings(sender);
        }

        private void DungeonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetMatAndDungeonButtonsState();
            RewriteCurrentSettings(sender);
        }

        private void MatBCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MatBCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MatACheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }
        private void MatACheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MatSCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MatSCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MatLCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }
        private void MatLCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MatECheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MatECheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void CastOnBossCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SetMatAndDungeonButtonsState();
            RewriteCurrentSettings(sender);
        }

        private void CastOnBossCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetMatAndDungeonButtonsState();
            RewriteCurrentSettings(sender);
        }

        private void MakeReplaysIfDungeonDoesntLoadCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MakeReplaysIfDungeonDoesntLoadCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MissclickOnDungeonsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetMatAndDungeonButtonsState();
            RewriteCurrentSettings(sender);
        }

        private void MissclickOnDungeonsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetMatAndDungeonButtonsState();
            RewriteCurrentSettings(sender);
        }

        private void MissclicksOnDungeonsIncludeDiagonalsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MissclicksOnDungeonsIncludeDiagonalsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ReplaysCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            FarmDungeonCheckbox.IsChecked = false;
            SkipWavesCheckbox.IsChecked = false;
            ABModeCheckbox.IsChecked = false;
            NotificationOnlyModeCheckbox.IsChecked = false;
            RewriteCurrentSettings(sender);
        }

        private void ReplaysCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void SkipWavesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SkipWithOrangesCheckbox.IsEnabled = true;

            UpgradeCastleCheckbox.IsChecked = false;
            UpgradeHeroForCrystalsCheckbox.IsChecked = false;
            ReplaysCheckbox.IsChecked = false;
            FarmDungeonCheckbox.IsChecked = false;
            NotificationOnlyModeCheckbox.IsChecked = false;

            if (AdForCoinsCheckbox.IsChecked == true)
            {
                AdAfterSkipOnlyCheckbox.IsEnabled = true;
            }

            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void SkipWavesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SkipWithOrangesCheckbox.IsEnabled = false;

            AdAfterSkipOnlyCheckbox.IsEnabled = false;

            SkipWithOrangesCheckbox.IsEnabled = false;

            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void FiveWavesBetweenSkipsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            ABModeCheckbox.IsChecked = false;
            RewriteCurrentSettings(sender);
        }

        private void FiveWavesBetweenSkipsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void SkipWithOrangesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void SkipWithOrangesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ABModeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            NotificationOnlyModeCheckbox.IsChecked = false;
            ReplaysCheckbox.IsChecked = false;
            FarmDungeonCheckbox.IsChecked = false;
            SetABParameters();
            UpdateWaitBetweenBattlesWaitState();
            RewriteCurrentSettings(sender);
        }

        private void ABModeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetABParameters();
            UpdateWaitBetweenBattlesWaitState();
            RewriteCurrentSettings(sender);
        }

        private void ABWaveCancelingCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            ABModeCheckbox.IsChecked = false;
            ReplaysCheckbox.IsChecked = false;
            FarmDungeonCheckbox.IsChecked = false;
            NotificationOnlyModeCheckbox.IsChecked = false;
            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void ABWaveCancelingCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void RadioButton_RewriteSettings(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void TabRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void TabRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void BreakABOn30CrystalsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void BreakABOn30CrystalsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }
        private void DesktopNotificationOn30CrystalsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void DesktopNotificationOn30CrystalsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetABParameters();
            RewriteCurrentSettings(sender);
        }
        private void PlayAudioOn30CrystalsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void PlayAudioOn30CrystalsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetABParameters();
            RewriteCurrentSettings(sender);
        }
        private void NotificationOnlyModeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            FarmDungeonCheckbox.IsChecked = false;
            SkipWavesCheckbox.IsChecked = false;
            ReplaysCheckbox.IsChecked = false;
            ABModeCheckbox.IsChecked = false;
            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void NotificationOnlyModeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetABParameters();
            RewriteCurrentSettings(sender);
        }
        private void Log30CrystalsDetectionCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void Log30CrystalsDetectionCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void AudioRadioButton_RewriteSettings(object sender, RoutedEventArgs e)
        {
            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void BackgroundModeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void BackgroundModeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void SimulateMouseMovementCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void SimulateMouseMovementCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void RandomizeCastSequenceCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void RandomizeCastSequenceCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void SolveCaptchaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void SolveCaptchaCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void RestartOnCaptchaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void RestartOnCaptchaCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void UpgradeCastleCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SkipWavesCheckbox.IsChecked = false;
            UpgradeHeroForCrystalsCheckbox.IsChecked = false;
            FloorToUpgradeCastleLabel.IsEnabled = true;
            FloorToUpgradeCastleComboBox.IsEnabled = true;

            SlotToUpgradeHeroLabel.IsEnabled = false;
            SlotToUpgradeHeroComboBox.IsEnabled = false;

            RewriteCurrentSettings(sender);
        }
        private void UpgradeCastleCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            FloorToUpgradeCastleComboBox.IsEnabled = false;
            FloorToUpgradeCastleLabel.IsEnabled = false;
            RewriteCurrentSettings(sender);
        }

        private void FloorToUpgradeCastle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void UpgradeHeroForCrystalsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SkipWavesCheckbox.IsChecked = false;
            UpgradeCastleCheckbox.IsChecked = false;
            SlotToUpgradeHeroLabel.IsEnabled = true;
            SlotToUpgradeHeroComboBox.IsEnabled = true;

            FloorToUpgradeCastleComboBox.IsEnabled = false;
            FloorToUpgradeCastleLabel.IsEnabled = false;
            RewriteCurrentSettings(sender);
        }

        private void UpgradeHeroForCrystalsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SlotToUpgradeHeroLabel.IsEnabled = false;
            SlotToUpgradeHeroComboBox.IsEnabled = false;
            RewriteCurrentSettings(sender);
        }

        private void FloorToUpgradeHero_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void AdForSpeedCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void AdForSpeedCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void AdForCoinsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            AdDuringx3Checkbox.IsEnabled = true;
            AdAfterSkipOnlyCheckbox.IsEnabled = SkipWavesCheckbox.IsChecked == true;

            RewriteCurrentSettings(sender);
        }

        private void AdForCoinsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            AdDuringx3Checkbox.IsEnabled = false;
            AdAfterSkipOnlyCheckbox.IsEnabled = false;
            RewriteCurrentSettings(sender);
        }

        private void AdDuringx3Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void AdDuringx3Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void AdAfterSkipOnlyCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void AdAfterSkipOnlyCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void HealAltarCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            DeathAltarCheckbox.IsChecked = false;
            RewriteCurrentSettings(sender);
        }

        private void HealAltarCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void DeathAltarCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            HealAltarCheckbox.IsChecked = false;
            RewriteCurrentSettings(sender);
        }

        private void DeathAltarCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void PwOnBossCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            PwOnBossDelayLabel.IsEnabled = true;
            PwOnBossDelayTextBox.IsEnabled = true;
            RewriteCurrentSettings(sender);
        }

        private void PwOnBossCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            PwOnBossDelayLabel.IsEnabled = false;
            PwOnBossDelayTextBox.IsEnabled = false;
            RewriteCurrentSettings(sender);
        }

        private void MonitorFreezingCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MonitorFreezingCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void IHaveX3Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void IHaveX3Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }
        private void SpeedupOnItemDropCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void SpeedupOnItemDropCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }
        private void DoSaveBeofreCleanup_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void DoSaveBeofreCleanup_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }
        private void CollectMimicCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CollectMimicLabel.IsEnabled = true;
            CollectMimicChanceTextBox.IsEnabled = true;
            RewriteCurrentSettings(sender);
        }

        private void CollectMimicCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectMimicLabel.IsEnabled = false;
            CollectMimicChanceTextBox.IsEnabled = false;
            RewriteCurrentSettings(sender);
        }

        private void OrcbandOnSkipOnlyCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void OrcbandOnSkipOnlyCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MilitaryFOnSkipOnlyCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void MilitaryFOnSkipOnlyCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void TextBox_RewriteSettings(object sender, TextChangedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }
        private void TextBox_Insert0OnError(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox tb)
            {
                if (!int.TryParse(tb.Text, out _))
                {
                    tb.Text = "0";
                }
            }
        }

        public void UpdateWaitBetweenBattlesWaitState()
        {
            bool abMode = ABModeCheckbox.IsChecked == true;
            bool ignoreMode = IgnoreWaitsOnABModeCheckbox.IsChecked == true;

            foreach (var wbbuc in GetWaitBetweenBattlesUserControls())
            {
                wbbuc.SetIgnoredWaitState(abMode && ignoreMode);
            }
        }

        private void IgnoreWaitsBetweenBattlesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateWaitBetweenBattlesWaitState();
            RewriteCurrentSettings(sender);
        }

        private void IgnoreWaitsBetweenBattlesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateWaitBetweenBattlesWaitState();
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotItemsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotItemsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotRunesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotRunesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotSolvedCaptchasCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotSolvedCaptchasCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotFailedCaptchasCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotFailedCaptchasCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotCaptchaErrorsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotCaptchaErrorsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotOnEscCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotOnEscCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotLongLoadCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }
        private void ScreenshotLongLoadCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotLongWaveCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotLongWaveCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotAfter10EscCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotAfter10EscCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotABErrorsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotABErrorsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }
        private void ScreenshotNoxLoadFailCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotNoxLoadFailCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotNoxMainMenuLoadFailCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotNoxMainMenuLoadFailCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotNoxClearAllFailCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void ScreenshotNoxClearAllFailCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void CheckAllScreenshots_Click(object sender, RoutedEventArgs e)
        {
            openToRewrite = false;
            foreach (var c in ScreenshotCheckboxesCanvas.Children)
            {
                if (c is System.Windows.Controls.CheckBox cb)
                    cb.IsChecked = true;
            }
            openToRewrite = true;
            RewriteCurrentSettings();
        }

        private void UncheckAllScreenshots_Click(object sender, RoutedEventArgs e)
        {
            openToRewrite = false;
            foreach (var c in ScreenshotCheckboxesCanvas.Children)
            {
                if (c is System.Windows.Controls.CheckBox cb)
                    cb.IsChecked = false;
            }
            openToRewrite = true;
            RewriteCurrentSettings();
        }

        private void UpdateWaitBetweenBattlesUCNumbers()
        {
            int c = 1;
            foreach (var o in WaitBetweenBattlesUCStackPanel.Children)
            {
                if (o is WaitBetweenBattlesUserControl w)
                {
                    w.Number = c++;
                }
            }
        }

        private void AddUserWaitBetweenBattlesControl_Click(object sender, RoutedEventArgs e)
        {
            if (isSwappingWbbuc)
            {
                return;
            }
            var uc = new WaitBetweenBattlesUserControl(AdvancedTabScrollViewer);
            AddWaitBetweenBattlesUserControl(uc);

            UpdateWaitBetweenBattlesWaitState();
            RewriteCurrentSettings();
        }

        private void RemoveWaitBetweenBattlesUserControl(WaitBetweenBattlesUserControl uc)
        {
            if (isSwappingWbbuc)
            {
                return;
            }
            uc.OnRemove -= RemoveWaitBetweenBattlesUserControl;
            uc.OnUpdate -= RewriteCurrentSettings;
            uc.OnSwapUp -= SwapWaitBetweenBattlesUserConstols;
            uc.OnSwapDown -= SwapWaitBetweenBattlesUserConstols;
            WaitBetweenBattlesUCStackPanel.Children.Remove(uc);
            UpdateWaitBetweenBattlesUCNumbers();
            RewriteCurrentSettings();
        }

        private void AddWaitBetweenBattlesUserControl(WaitBetweenBattlesUserControl uc)
        {
            uc.OnRemove += RemoveWaitBetweenBattlesUserControl;
            uc.OnUpdate += RewriteCurrentSettings;
            uc.OnSwapUp += SwapWaitBetweenBattlesUserConstols;
            uc.OnSwapDown += SwapWaitBetweenBattlesUserConstols;
            uc.Number = WaitBetweenBattlesUCStackPanel.Children.Count + 1;
            WaitBetweenBattlesUCStackPanel.Children.Add(uc);
        }

        private async void SwapWaitBetweenBattlesUserConstols(WaitBetweenBattlesUserControl control, SwapDirection direction)
        {

            if (isSwappingWbbuc)
            {
                return;
            }

            if (control == null)
            {
                return;
            }

            var children = WaitBetweenBattlesUCStackPanel.Children;
            int index = children.IndexOf(control);
            if (index < 0)
            {
                return;
            }

            int swapIndex = direction switch
            {
                SwapDirection.Up when index > 0 => index - 1,
                SwapDirection.Down when index < children.Count - 1 => index + 1,
                _ => -1
            };

            if (swapIndex < 0)
            {
                return;
            }

            WaitBetweenBattlesUserControl target = (WaitBetweenBattlesUserControl)children[swapIndex];

            children.RemoveAt(swapIndex);
            children.RemoveAt(index > swapIndex ? index - 1 : index);

            children.Insert(index > swapIndex ? swapIndex : index, control);
            children.Insert(index > swapIndex ? index : index, target);

            RewriteCurrentSettings();

            CallAfterDelay(TimeSpan.FromMilliseconds(swapWbbucAnimationDuration / 2), UpdateWaitBetweenBattlesUCNumbers);
            await PerformSwapAnimation(control, target, direction);

            isSwappingWbbuc = false;
        }

        private async Task PerformSwapAnimation(WaitBetweenBattlesUserControl current, WaitBetweenBattlesUserControl target, SwapDirection direction)
        {
            isSwappingWbbuc = true;
            double offset = current.ActualHeight > 0 ? current.ActualHeight : 30;

            var t1 = GetOrCreateTransform(current);
            var t2 = GetOrCreateTransform(target);

            double shift = direction == SwapDirection.Up ? -offset : offset;

            var anim1 = CreateTranslateYAnimation(-shift);
            var anim2 = CreateTranslateYAnimation(shift);

            var tcs = new TaskCompletionSource<bool>();
            int completedCount = 0;

            void OnComplete(object? s, EventArgs e)
            {
                if (++completedCount == 2)
                {
                    tcs.SetResult(true);
                }
            }

            anim1.Completed += OnComplete;
            anim2.Completed += OnComplete;

            t1.BeginAnimation(TranslateTransform.YProperty, anim1);
            t2.BeginAnimation(TranslateTransform.YProperty, anim2);

            await tcs.Task;

            t1.BeginAnimation(TranslateTransform.YProperty, null);
            t2.BeginAnimation(TranslateTransform.YProperty, null);

            t1.Y = 0;
            t2.Y = 0;

        }

        private TranslateTransform GetOrCreateTransform(UIElement element)
        {
            if (element.RenderTransform is TranslateTransform t)
                return t;

            var tt = new TranslateTransform();
            element.RenderTransform = tt;
            return tt;
        }

        private DoubleAnimation CreateTranslateYAnimation(double from)
        {
            return new DoubleAnimation
            {
                From = from,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(swapWbbucAnimationDuration),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
        }
        private void CallAfterDelay(TimeSpan delay, Action act)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(delay.TotalMilliseconds)
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                act();
            };
            timer.Start();
        }
        private void EnableAllWaitsBetweenBattles_Click(object sender, RoutedEventArgs e)
        {
            openToRewrite = false;
            foreach (var wbb in WaitBetweenBattlesUCStackPanel.Children)
            {
                if (wbb is WaitBetweenBattlesUserControl wbbuc)
                {
                    wbbuc.SetChecked(true);
                }
            }
            openToRewrite = true;
            RewriteCurrentSettings();
        }

        private void DisableAllWaitsBetweenBattles_Click(object sender, RoutedEventArgs e)
        {
            openToRewrite = false;
            foreach (var wbb in WaitBetweenBattlesUCStackPanel.Children)
            {
                if (wbb is WaitBetweenBattlesUserControl wbbuc)
                {
                    wbbuc.SetChecked(false);
                }
            }
            openToRewrite = true;
            RewriteCurrentSettings();
        }

    }
}
