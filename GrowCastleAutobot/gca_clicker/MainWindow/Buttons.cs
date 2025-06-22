using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        private void WindowName_TextChanged(object sender, TextChangedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void BuildToPlayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void FarmDungeonCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void DungeonComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void MatBCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }
        private void MatACheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void MatSCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void MatLCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void MatECheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void CastOnBossCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void CastOnBossDelayTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void MakeReplaysIfDungeonDoesntLoadCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ReplaysCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void SkipWavesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void FiveWavesBetweenSkipsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void SkipWithOrangesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ABModeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ABWaveCancelingCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void GabRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void TabRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void BreakABOn30CrystalsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void TimeToBreakABTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void SkipsBetweenABSessionsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void BackgroundModeCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void SolveCaptchaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void RestartOnCaptchaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void UpgradeCastleCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void FloorToUpgradeCastle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void UpgradeHeroForCrystalsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void FloorToUpgradeHero_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void AdForSpeedCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void AdForCoinsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void AdDuringx3Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void AdAfterSkipOnlyCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void HealAltarCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void DeathAltarCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void PwOnBossCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void PwOnBossDelayTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotItemsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotRunesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotSolvedCaptchasCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotFailedCaptchasCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotOnEscCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotLongLoadCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotLongWaveCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotAfter10EscCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotNoxLoadFailCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotNoxMainMenuLoadFailCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

        private void ScreenshotNoxClearAllFailCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }







        private void FarmDungeonCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void MatBCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }
        private void MatACheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void MatSCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void MatLCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void MatECheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void CastOnBossCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void MakeReplaysIfDungeonDoesntLoadCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ReplaysCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void SkipWavesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void FiveWavesBetweenSkipsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void SkipWithOrangesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ABModeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ABWaveCancelingCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void GabRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void TabRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void BreakABOn30CrystalsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void BackgroundModeCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void SolveCaptchaCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void RestartOnCaptchaCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void UpgradeCastleCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void UpgradeHeroForCrystalsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void AdForSpeedCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void AdForCoinsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void AdDuringx3Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void AdAfterSkipOnlyCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void HealAltarCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void DeathAltarCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void PwOnBossCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotItemsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotRunesCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotSolvedCaptchasCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotFailedCaptchasCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotOnEscCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotLongLoadCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotLongWaveCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotAfter10EscCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotNoxLoadFailCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotNoxMainMenuLoadFailCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }


        private void ScreenshotNoxClearAllFailCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            RewriteCurrentSettings();
        }

    }
}
