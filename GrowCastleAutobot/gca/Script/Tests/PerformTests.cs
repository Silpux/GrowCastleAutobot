using gca.Classes;
using gca.Classes.Exceptions;
using gca.Enums;
using gca.Script.Tests;
using System.Windows;

namespace gca
{
    public partial class MainWindow : Window
    {
        public void PerformTestMode(TestMode testMode)
        {

            switch (testMode)
            {
                case TestMode.MouseMovement1:
                    foreach (var p in TestFunctions.GetCirclePointsClockwise(773, 470, 300))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            InfoLabel.Content = p.ToString();
                        });
                        SetCursor(p.x, p.y);
                        Wait(1);
                    }
                    break;
                case TestMode.MouseMovement2:
                    foreach (var p in TestFunctions.GetRectangleBorderClockwise(305, 96, 265, 406))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            InfoLabel.Content = p.ToString();
                        });
                        SetCursor(p.x, p.y);
                        Wait(1);
                    }
                    break;
                case TestMode.MouseMovement3:
                    foreach (var p in TestFunctions.GetSpiral(773, 470, 3000))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            InfoLabel.Content = p.ToString();
                        });
                        SetCursor(p.x, p.y);
                        Wait(1);
                    }
                    break;
                case TestMode.MouseMove:
                    int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            x1 = int.Parse(X1MouseMovementTestTextBox.Text);
                            y1 = int.Parse(Y1MouseMovementTestTextBox.Text);
                            x2 = int.Parse(X2MouseMovementTestTextBox.Text);
                            y2 = int.Parse(Y2MouseMovementTestTextBox.Text);
                        });
                    }
                    catch (Exception e)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            InfoLabel.Content = $"Error: {e.Message}";
                        });
                        break;
                    }
                    SetCursor(x1, y1);
                    previousMousePosition.x = x1;
                    previousMousePosition.y = y1;
                    Move(x2, y2);
                    break;
                case TestMode.CrystalsCount:

                    bool darkMode = false;

                    Dispatcher.Invoke(() =>
                    {
                        darkMode = DarkModeCrystalsCountTestCheckBox.IsChecked == true;
                    });
                    if (!darkMode && !CheckSky())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            CrystalsCountLabel.Content = "Go to gc menu";
                        });
                        break;
                    }
                    else if (darkMode && CheckSky())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            CrystalsCountLabel.Content = "Open hero window";
                        });
                        break;
                    }

                    int crystals = CountCrystals(!darkMode, true);

                    Dispatcher.Invoke(() =>
                    {
                        string s = CrystalsCountLabel.Content.ToString()!;
                        s += $" Result: {crystals}";
                        CrystalsCountLabel.Content = s;
                    });

                    break;
                case TestMode.Restart:

                    Restart();

                    Dispatcher.Invoke(() =>
                    {
                        RestartTestLabel.Content = "Restart done";
                    });

                    break;
                case TestMode.Reset:

                    Reset();

                    Dispatcher.Invoke(() =>
                    {
                        RestartTestLabel.Content = "Reset done";
                    });

                    break;
                case TestMode.Cleanup:

                    MakeCleanup();

                    Dispatcher.Invoke(() =>
                    {
                        RestartTestLabel.Content = "Clenup done";
                    });

                    break;
                case TestMode.UpgradeHero:

                    if (!CheckGCMenu())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UpgradeTestLabel.Content = "Not in gc menu";
                        });
                        break;
                    }

                    UpgradeHero();

                    Dispatcher.Invoke(() =>
                    {
                        UpgradeTestLabel.Content = "Finished hero upgrading";
                    });

                    break;
                case TestMode.UpgradeCastle:

                    if (!CheckGCMenu())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            UpgradeTestLabel.Content = "Not in gc menu";
                        });
                        break;
                    }

                    UpgradeTower();

                    Dispatcher.Invoke(() =>
                    {
                        UpgradeTestLabel.Content = "Finished castle upgrading";
                    });

                    break;
                case TestMode.OnlineActions:

                    if (!CheckGCMenu())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            OnlineActionsTestStatusLabel.Content = "Not in gc menu";
                        });
                        break;
                    }

                    OnlineActions onlineActions = OnlineActions.None;

                    Dispatcher.Invoke(() =>
                    {
                        if (OpenGuildTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.OpenGuild;
                        }
                        if (OpenRandomProfileFromGuildTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.OpenRandomProfileFromMyGuild;
                        }
                        if (OpenGuildsChatTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.OpenGuildChat;
                        }
                        if (OpenGuildsTopTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.OpenGuildsTop;
                        }
                        if (OpenTopTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.OpenTop;
                        }
                        if (OpenTopSeasonTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.OpenTopSeason;
                        }
                        if (OpenHellSeasonMyTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.OpenTopHellSeasonMy;
                        }
                        if (OpenHellSeasonTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.OpenTopHellSeason;
                        }
                        if (OpenWavesTopMyTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.OpenTopWavesMy;
                        }
                        if (OpenWavesTopTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.OpenTopWavesOverall;
                        }
                        if (CraftStonesTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.CraftStones;
                        }
                        if (DoSaveTestCheckbox.IsChecked == true)
                        {
                            onlineActions |= OnlineActions.DoSave;
                        }
                    });
                    try
                    {

                        Dispatcher.Invoke(() =>
                        {
                            OnlineActionsTestStatusLabel.Content = "Doing guild actions";
                        });
                        PerformGuildActions(onlineActions);
                        Dispatcher.Invoke(() =>
                        {
                            OnlineActionsTestStatusLabel.Content = "Doing top actions";
                        });
                        PerformTopActions(onlineActions);
                        Dispatcher.Invoke(() =>
                        {
                            OnlineActionsTestStatusLabel.Content = "Craft stones";
                        });
                        PerformCraftStonesActions(onlineActions);
                        Dispatcher.Invoke(() =>
                        {
                            OnlineActionsTestStatusLabel.Content = "Do save";
                        });
                        PerformSaveActions(onlineActions);
                    }
                    catch (OnlineActionsException e)
                    {
                        string message = $"Error happened during online actions: {e.Info}";
                        Log.F(message);
                        Dispatcher.Invoke(() =>
                        {
                            OnlineActionsTestStatusLabel.Content = message;
                        });
                        break;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        OnlineActionsTestStatusLabel.Content = "Finished online actions";
                    });

                    break;
                case TestMode.ShowGameStatus:

                    Dispatcher.Invoke(() =>
                    {
                        GameStatusTestLabel.Content = "";
                    });
                    List<string> status = new List<string>();
                    G();

                    if (CheckSky(false))
                    {
                        if (CheckEmptyGame(false))
                        {
                            status.Add("Empty acc");
                        }
                        else if (CheckGCMenu(false))
                        {
                            status.Add("In gc menu");
                            if (IsAdForCoinsOnScreen(false))
                            {
                                status.Add("Has ad for coins");
                            }
                            if (IsHellButtonsOpened())
                            {
                                status.Add("Hell buttons open");
                            }
                            else if (IsReplayButtonsOpened())
                            {
                                status.Add("Replay buttons open");
                            }
                        }
                        else if (IsInTown(false))
                        {
                            int forgePos = FindForgePosition(false);
                            status.Add($"Is in town.\nForge position: {forgePos}");
                        }
                        else
                        {
                            if (IsInBattle(false))
                            {
                                status.Add("Is in battle");
                            }
                            if (IsMidWave(false))
                            {
                                status.Add("Is mid wave");
                            }
                            if (IsSkipPanelOnScreen(false))
                            {
                                status.Add("Skip panel on screen");
                            }
                        }
                    }
                    else
                    {
                        if (HasPausePanel(false))
                        {
                            status.Add("Paused");
                            if (HasExitAfterBattlePanel(false))
                            {
                                status.Add("Has Exit after battle button");
                            }
                        }
                        else if (HasExitPanel(false))
                        {
                            status.Add("Exit panel");
                        }
                        else if (IsInForge(false))
                        {
                            status.Add("Is in forge");
                            if (IsOnTopOfForge(false))
                            {
                                status.Add("On top on forge");
                            }
                            else
                            {
                                status.Add("Scrolled down in forge");
                            }
                        }
                        else if (IsLoseABPanelOnScreen(false))
                        {
                            status.Add("Lose AB panel");
                        }
                        else if (CaptchaOnScreen(false))
                        {
                            status.Add("Captcha");
                        }
                        else if (IsHeroPanelOnScreen(false))
                        {
                            status.Add("Hero opened");
                        }
                        else if (IsChooseClassPanelOnScreen(false))
                        {
                            status.Add("Choose class opened");
                        }
                        else if (IsInShop(false))
                        {
                            status.Add("Is in shop");
                        }
                        else if (IsItemOnScreen(false))
                        {
                            status.Add("Item");
                            ItemGrade grade = GetItemGrade();
                            if (grade == ItemGrade.NoItem)
                            {
                                status.Add("Item detected, then not");
                            }
                            else if (grade != ItemGrade.NoItem && grade != ItemGrade.None)
                            {
                                status.Add($"Grade: {grade}");
                            }
                            else if (IsRuneOnScreen(false))
                            {
                                status.Add($"Rune");
                            }
                            else
                            {
                                status.Add($"Couldn't identify item");
                            }

                        }
                        else if (IsInTop(false))
                        {
                            status.Add($"Is in top");
                            TopSection section = GetCurrentTopSection();
                            bool globalTop = IsTopGlobalOpen();
                            status.Add($"Top section {section}.\nGlobal: {globalTop}");
                        }
                        else if (IsInGuild(false))
                        {
                            status.Add($"In guild");
                        }
                        else if (IsSaveGamePanelOpened(false))
                        {
                            status.Add($"Save game");
                        }
                        else if (IsInPlayerProfile(false))
                        {
                            status.Add($"Player profile");
                        }
                        else if (IsInNoxMainMenu(false))
                        {
                            status.Add($"Nox main menu");
                        }
                        else
                        {
                            status.Add($"Couldn't identify");
                        }
                    }

                    if (IsPopupOnScreen(false))
                    {
                        status.Add($"Has popup");
                    }

                    Dispatcher.Invoke(() =>
                    {
                        GameStatusTestLabel.Content = string.Join("\n", status);
                    });

                    break;
                case TestMode.SolveCaptcha:
                    if (!CaptchaOnScreen(false))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            SolveCaptchaTestLabel.Content = "Need to open captcha";
                        });
                        return;
                    }
                    SolveCaptcha(true);

                    Dispatcher.Invoke(() =>
                    {
                        SolveCaptchaTestLabel.Content = "Finished";
                    });
                    break;
            }

        }

    }
}
