using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace gca_clicker
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
                DungeonComboBox.IsEnabled = true;
                MakeReplaysIfDungeonDoesntLoadCheckBox.IsEnabled = true;
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
                DungeonComboBox.IsEnabled = false;
                MakeReplaysIfDungeonDoesntLoadCheckBox.IsEnabled = false;
                CastDelayInDungeonLabel.IsEnabled = false;
                CastOnBossDelayTextBox.IsEnabled = false;
                CastOnBossCheckbox.IsEnabled = false;
                SetActiveMatButtons(false);
            }
        }
        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ComboBox combo && combo.Items.Count > 0)
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
            if (ABModeCheckbox.IsChecked == true || ABWaveCancelingCheckbox.IsChecked == true)
            {
                GabRadioButton.IsEnabled = true;
                TabRadioButton.IsEnabled = true;
                if (ABModeCheckbox.IsChecked == true)
                {
                    BreakABOn30CrystalsCheckbox.IsEnabled = true;
                    FiveWavesBetweenSkipsCheckbox.IsChecked = false;
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
                }
            }
            else
            {
                GabRadioButton.IsEnabled = false;
                TabRadioButton.IsEnabled = false;

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
            }
        }

        private void BuildToPlayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }

        private void FarmDungeonCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            SetMatAndDungeonButtonsState();
            ABWaveCancelingCheckbox.IsChecked = false;
            ABModeCheckbox.IsChecked = false;
            SkipWavesCheckbox.IsChecked = false;
            ReplaysCheckbox.IsChecked = false;
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

        private void ReplaysCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            FarmDungeonCheckbox.IsChecked = false;
            SkipWavesCheckbox.IsChecked = false;
            ABModeCheckbox.IsChecked = false;
            ABWaveCancelingCheckbox.IsChecked = false;
            RewriteCurrentSettings(sender);
        }

        private void ReplaysCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }


        private void SkipWavesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            FiveWavesBetweenSkipsCheckbox.IsEnabled = true;
            SkipWithOrangesCheckbox.IsEnabled = true;

            UpgradeCastleCheckbox.IsChecked = false;
            UpgradeHeroForCrystalsCheckbox.IsChecked = false;
            ReplaysCheckbox.IsChecked = false;
            FarmDungeonCheckbox.IsChecked = false;

            if (AdForCoinsCheckbox.IsChecked == true)
            {
                AdAfterSkipOnlyCheckbox.IsEnabled = true;
            }

            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void SkipWavesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            FiveWavesBetweenSkipsCheckbox.IsEnabled = false;
            SkipWithOrangesCheckbox.IsEnabled = false;

            AdAfterSkipOnlyCheckbox.IsEnabled = false;

            SkipWithOrangesCheckbox.IsEnabled = false;
            FiveWavesBetweenSkipsCheckbox.IsEnabled = false;

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
            ABWaveCancelingCheckbox.IsChecked = false;
            ReplaysCheckbox.IsChecked = false;
            FarmDungeonCheckbox.IsChecked = false;
            SetABParameters();
            RewriteCurrentSettings(sender);
        }


        private void ABModeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void ABWaveCancelingCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            ABModeCheckbox.IsChecked = false;
            ReplaysCheckbox.IsChecked = false;
            FarmDungeonCheckbox.IsChecked = false;
            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void ABWaveCancelingCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetABParameters();
            RewriteCurrentSettings(sender);
        }

        private void GabRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings(sender);
        }


        private void GabRadioButton_Unchecked(object sender, RoutedEventArgs e)
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
        private void TextBox_RewriteSettings(object sender, TextChangedEventArgs e)
        {
            RewriteCurrentSettings(sender);
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
                if (c is CheckBox cb)
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
                if (c is CheckBox cb)
                    cb.IsChecked = false;
            }
            openToRewrite = true;
            RewriteCurrentSettings();
        }










    }
}
