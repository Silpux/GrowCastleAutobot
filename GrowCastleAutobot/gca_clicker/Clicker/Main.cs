using gca_clicker.Classes;
using gca_clicker.Classes.Exceptions;
using gca_clicker.Clicker;
using gca_clicker.Clicker.Tests;
using gca_clicker.Enums;
using gca_clicker.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static gca_clicker.Classes.Utils;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        private bool backgroundMode;
        private nint hwnd;

        private Bitmap currentScreen;

        private bool restartRequested = false;


        private void WorkerLoop()
        {

            try
            {

                if(testMode != TestMode.None)
                {
                    PerformTestMode(testMode);
                    Halt();
                }

                Dispatcher.Invoke(() =>
                {
                    NextCleanupTimeLabel.Content = $"Next cleanup: {lastCleanupTime + cleanupIntervalTimeSpan:dd.MM.yyyy HH:mm:ss}";
                });
                int prevFrameStatus = 0;
                while (true)
                {
                    if (CaptchaOnScreen())
                    {
                        if (solveCaptcha)
                        {
                            SolveCaptcha();
                        }
                        else
                        {
                            StopClicker();
                        }
                    }

                    if (CheckSky())
                    {

                        if (CheckGCMenu())
                        {
                            Log.I("Gc menu detected");

                            if (CheckEmptyGame())
                            {
                                Wait(500);
                                if (CheckEmptyGame())
                                {
                                    Log.C("Empty game detected. Halt");
                                    if (saveScreenshotsOnError)
                                    {
                                        screenshotCache.SaveAllToFolder(Cst.SCREENSHOT_ERROR_SCREEN_CACHE_PATH);
                                    }
                                    Halt();
                                }
                            }

                            if (DateTime.Now - lastCleanupTime > cleanupIntervalTimeSpan)
                            {

                                try
                                {
                                    if (doSaveBeforeCleanup)
                                    {
                                        DoSave();
                                    }
                                }
                                catch(OnlineActionsException e)
                                {
                                    Log.E($"Error happened while doing save before cleanup: {e.Info}");
                                }

                                MakeCleanup();
                                continue;
                            }
                            else if (doRestarts && DateTime.Now > nextRestartDt)
                            {
                                Log.I("Restart time's up. Do restart");
                                Wait(300);
                                Restart();
                                Log.I("Restart made");
                                Wait(200);
                                continue;
                            }
                            else
                            {
                                if (prevFrameStatus == 1 && dungeonFarm && dungeonToFarm.IsDungeon())
                                {
                                    ShowBattleLength();
                                }
                                CheckAdForX3();
                                WaitForAdAndWatch();
                                TryUpgradeHero();
                                TryUpgradeTower();

                                foreach (var rt in waitBetweenBattlesRuntimes)
                                {
                                    rt.Suspend();
                                }

                                DoActionsBeforeReplay();

                                foreach (var rt in waitBetweenBattlesRuntimes)
                                {
                                    if (!rt.IsActive)
                                    {
                                        rt.Start();
                                    }
                                    else
                                    {
                                        rt.Resume();
                                    }
                                }
                                Log.I($"Check ended");

                                Replay();

                                prevFrameStatus = 2;
                            }
                        }
                        else
                        {

                            if (IsInTown())
                            {
                                SwitchTown();
                                prevFrameStatus = 0;
                                continue;
                            }

                            G();
                            if (dungeonFarm)
                            {
                                ActivateHeroesDun();
                            }
                            else
                            {
                                ActivateHeroes();
                            }
                            prevFrameStatus = 1;
                        }
                    }
                    else
                    {

                        Log.I("Sky not clear. wait 4s");

                        CheckNoxState();

                        bool quitWaiting = false;
                        if (WaitUntil(() => CheckSky() || quitWaiting, () =>
                        {
                            quitWaiting = CloseOverlap();
                        }, 4000, 10))
                        {
                            if (!quitWaiting)
                            {
                                Log.I("sky cleared. continue");
                            }
                        }
                        else
                        {
                            Log.Q("4s waited");
                            EscClickStart();
                        }

                        prevFrameStatus = 0;

                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.V($"Stop clicker thread. Time running: {RunningTime:hh\\:mm\\:ss\\.fff}");
                SetStoppedUI();
            }
            catch (OnlineActionsException e)
            {
                string message = $"Stop clicker. Online action exception: {e.Info}\n\nTime running: {RunningTime:hh\\:mm\\:ss\\.fff}";
                Log.C(message);
                SetStoppedUI();
                WinAPI.ForceBringWindowToFront(this);
                MessageBox.Show(message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                Log.C($"Unhandled exception:\n{e.Message}\n\nInner message: {e.InnerException?.Message}\n\nCall stack:\n{e.StackTrace}\n\nTime running: {RunningTime:hh\\:mm\\:ss\\.fff}");
                SetStoppedUI();

                WinAPI.ForceBringWindowToFront(this);
                MessageBox.Show($"Error happened while executing clicker:\n{e.Message}\n\nInner message: {e.InnerException?.Message}\n\nCall stack:\n{e.StackTrace}\n\nTime running: {RunningTime:hh\\:mm\\:ss\\.fff}\n\nCurrent time: {DateTime.Now:dd.MM.yyyy HH:mm:ss.fff}", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }

            clickerThread = null!;
            SetDefaultThreadState();

            if (restartRequested)
            {
                Dispatcher.Invoke(RestartThread);
            }

        }

        public void PerformTestMode(TestMode testMode)
        {

            switch (testMode)
            {
                case TestMode.MouseMovement1:
                    foreach(var p in TestFunctions.GetCirclePointsClockwise(773, 470, 300))
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

                    if (!CheckSky())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            CrystalsCountLabel.Content = "Go to gc menu";
                        });
                        break;
                    }

                    int crystals = CountCrystals(true, true);

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
                    catch(OnlineActionsException e)
                    {
                        string message = $"Error happened during online actions: {e.Info}";
                        Log.C(message);
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

                    if (CheckSky())
                    {
                        if (CheckEmptyGame())
                        {
                            status.Add("Empty acc");
                        }
                        else if (CheckGCMenu())
                        {
                            status.Add("In gc menu");
                            if (IsAdForCoinsOnScreen())
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
                        else if (IsInTown())
                        {
                            int forgePos = FindForgePosition();
                            status.Add($"Is in town.\nForge position: {forgePos}");
                        }
                        else
                        {
                            status.Add("Is in battle");
                            if (IsSkipPanelOnScreen())
                            {
                                status.Add("Skip panel on screen");
                            }
                        }
                    }
                    else
                    {
                        if (HasPausePanel())
                        {
                            status.Add("Paused");
                        }
                        else if (HasExitPanel())
                        {
                            status.Add("Exit panel");
                        }
                        else if (IsInForge())
                        {
                            status.Add("Is in forge");
                            if (IsOnTopOfForge())
                            {
                                status.Add("On top on forge");
                            }
                            else
                            {
                                status.Add("Scrolled down in forge");
                            }
                        }
                        else if (IsLoseABPanelOnScreen())
                        {
                            status.Add("Lose AB panel");
                        }
                        else if (CaptchaOnScreen())
                        {
                            status.Add("Captcha");
                        }
                        else if (IsHeroPanelOnScreen())
                        {
                            status.Add("Hero opened");
                        }
                        else if (IsItemOnScreen())
                        {
                            status.Add("Item");
                            ItemGrade grade = GetItemGrade();
                            if(grade == ItemGrade.NoItem)
                            {
                                status.Add("Item detected, then not");
                            }
                            else if(grade != ItemGrade.NoItem)
                            {
                                status.Add($"Grade: {grade}");
                            }
                            else if (IsRuneOnScreen())
                            {
                                status.Add($"Rune");
                            }
                            else
                            {
                                status.Add($"Couldn't identify item");
                            }

                        }
                        else if (IsInTop())
                        {
                            status.Add($"Is in top");
                            TopSection section = GetCurrentTopSection();
                            bool globalTop = IsTopGlobalOpen();
                            status.Add($"Top section {section}.\nGlobal: {globalTop}");
                        }
                        else if (IsInGuild())
                        {
                            status.Add($"In guild");
                        }
                        else if (IsSaveGamePanelOpened())
                        {
                            status.Add($"Save game");
                        }
                        else if (IsInPlayerProfile())
                        {
                            status.Add($"Player profile");
                        }
                        else if (IsInNoxMainMenu())
                        {
                            status.Add($"Nox main menu");
                        }
                        else
                        {
                            status.Add($"Couldn't identify");
                        }
                    }

                    if (IsPopupOnScreen())
                    {
                        status.Add($"Has popup");
                    }

                    Dispatcher.Invoke(() =>
                    {
                        GameStatusTestLabel.Content = string.Join("\n", status);
                    });

                    break;
                case TestMode.SolveCaptcha:
                    if (!CaptchaOnScreen())
                    {
                        Dispatcher.Invoke(() =>
                        {
                            SolveCaptchaTestLabel.Content = "Need to open captcha";
                        });
                        return;
                    }
                    SolveCaptcha();

                    Dispatcher.Invoke(() =>
                    {
                        SolveCaptchaTestLabel.Content = "Finished";
                    });
                    break;
            }


        }


        public void DoActionsBeforeReplay()
        {
            Log.I($"Check {waitBetweenBattlesRuntimes.Count} timers");
            for (int i = waitBetweenBattlesRuntimes.Count - 1; i >= 0; i--)
            {
                if (waitBetweenBattlesRuntimes[i].IsElapsed)
                {
                    WaitBetweenBattlesRuntime activeRT = waitBetweenBattlesRuntimes[i];

                    Log.I($"{i + 1} elapsed");
                    ActionBetweenBattle actions = activeRT.GetActions();

                    activeRT.Reset();

                    while (--i >= 0)
                    {
                        if (waitBetweenBattlesRuntimes[i].IsElapsed)
                        {
                            Log.I($"{i + 1} was elapsed. Reset and ignore wait");
                            waitBetweenBattlesRuntimes[i].Reset();
                            waitBetweenBattlesRuntimes[i].IgnoreWait();
                        }
                    }

                    if (actions.OnlineActionsBeforeWait)
                    {
                        Log.I("Doing online actions before wait");
                        PerformOnlineActions(actions.OnlineActions, activeRT, "Doing online actions before wait");
                    }

                    activeRT.ConfirmWait();

                    int totalMs = (int)actions.TimeToWait.TotalMilliseconds;
                    DateTime finishWaitDateTime = DateTime.Now + TimeSpan.FromMilliseconds(totalMs);

                    Log.I($"wait until {finishWaitDateTime:dd.MM.yyyy HH:mm:ss.fff}");
                    WaitUntil(() => false,
                        () =>
                        {
                            TimeSpan left = finishWaitDateTime - DateTime.Now;
                            double percent = left.TotalMilliseconds/ totalMs;
                            activeRT.UserControl.SetWaitingTimeLeft(finishWaitDateTime - DateTime.Now, percent);
                        }, (int)actions.TimeToWait.TotalMilliseconds, 37);

                    actions = activeRT.GetActions();

                    if (actions.OnlineActionsAfterWait)
                    {
                        Log.I("Doing online actions after wait");
                        PerformOnlineActions(actions.OnlineActions, activeRT, "Doing online actions after wait");
                    }

                }
            }

        }


        public void PerformOnlineActions(OnlineActions actions, WaitBetweenBattlesRuntime rt, string status)
        {
            try
            {

                if((actions & OnlineActions.AnyAction) == 0)
                {
                    Log.I($"No online actions to do");
                    return;
                }

                Wait(100);

                List<(string name, Action action)> methods = new(4);
                if ((actions & (OnlineActions.OpenGuild)) != 0)
                {
                    methods.Add(("Guild", () =>
                    {
                        PerformGuildActions(actions);
                    }
                    ));
                }

                if ((actions & (OnlineActions.OpenTop)) != 0)
                {
                    methods.Add(("Top", () =>
                    {
                        PerformTopActions(actions);
                    }
                    ));
                }

                if ((actions & (OnlineActions.CraftStones)) != 0)
                {
                    methods.Add(("Craft", () =>
                    {
                        PerformCraftStonesActions(actions);
                    }
                    ));
                }

                if ((actions & (OnlineActions.DoSave)) != 0)
                {
                    methods.Add(("Save", () =>
                    {
                        PerformSaveActions(actions);
                    }
                    ));
                }

                List<string> actionsSequence = new List<string>(6);
                int n = methods.Count;

                while (n > 1)
                {
                    n--;
                    int k = rand.Next(n + 1);
                    (string, Action) t = methods[k];
                    methods[k] = methods[n];
                    methods[n] = t;
                    actionsSequence.Insert(0, methods[n].name);
                }

                if (methods.Count > 0)
                {
                    actionsSequence.Insert(0, methods[0].name);
                }

                rt.UserControl.SetOnlineActionsUI(status, string.Join(", ", actionsSequence));

                foreach (var method in methods)
                {
                    method.action();
                }
            }
            catch (OnlineActionsException e)
            {
                Log.E($"Error on online action: {e.Info}. Will skip");
            }

        }

    }
}
