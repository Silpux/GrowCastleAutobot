using gca_clicker.Classes;
using gca_clicker.Classes.Exceptions;
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

                        Log.W("Sky not clear. wait 4s");

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
                            Log.I("4s waited");
                            EscClickStart();
                        }

                        prevFrameStatus = 0;

                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.V("Stop clicker thread");
                clickerThread = null!;
                SetStoppedUI();
            }
            catch (OnlineActionsException e)
            {
                string message = $"Stop clicker. Online action exception: {e.Info}";
                Log.C(message);
                SetStoppedUI();
                WinAPI.ForceBringWindowToFront(this);
                MessageBox.Show(message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                clickerThread = null!;
                Log.C($"Unhandled exception:\n{e.Message}\n\nInner message: {e.InnerException?.Message}");
                SetStoppedUI();

                WinAPI.ForceBringWindowToFront(this);
                MessageBox.Show($"Error happened while executing clicker:\n{e.Message}\n\nInner message: {e.InnerException?.Message}", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }

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

                    DateTime finishWaitDateTime = DateTime.Now + TimeSpan.FromMilliseconds((int)actions.TimeToWait.TotalMilliseconds);

                    Log.I($"wait until {finishWaitDateTime:dd.MM.yyyy HH:mm:ss.fff}");
                    WaitUntil(() => false,
                        () =>
                        {
                            activeRT.UserControl.SetWaitingTimeLeft(finishWaitDateTime - DateTime.Now);
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
