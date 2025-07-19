using gca_clicker.Classes;
using gca_clicker.Classes.Exceptions;
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
                                MakeCleanup();
                                continue;
                            }
                            else
                            {
                                if (prevFrameStatus == 1 && dungeonFarm && dungeonNumber > 6)
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

                                try
                                {
                                    DoActionsBeforeReplay();
                                }
                                catch (OnlineActionsException e)
                                {
                                    Log.E($"Error on online action: {e.Info}");
                                    continue;
                                }
                                finally
                                {
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
                                }

                                Replay();

                                prevFrameStatus = 2;
                            }
                        }
                        else
                        {

                            Getscreen();
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
                Log.I("Stop clicker thread");
                clickerThread = null!;
                SetStoppedUI();
            }
            catch (Exception e)
            {
                clickerThread = null!;
                Log.C($"Unhandled exception:\n{e.Message}");
                SetStoppedUI();

                WinAPI.ForceBringWindowToFront(this);
                MessageBox.Show($"Error happened while executing clicker:\n{e.Message}", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }

            if (restartRequested)
            {
                Dispatcher.Invoke(RestartThread);
            }

        }


        public void DoActionsBeforeReplay()
        {
            Log.I($"Check {waitBetweenBattlesRuntimes.Count} timers");
            for (int i = waitBetweenBattlesRuntimes.Count - 1; i >= 0; i--)
            {
                if (waitBetweenBattlesRuntimes[i].IsElapsed)
                {
                    WaitBetweenBattlesRuntime activeWaitRT = waitBetweenBattlesRuntimes[i];

                    Log.I($"{i + 1} elapsed. Will wait for it");

                    ActionBetweenBattle actions = activeWaitRT.GetActions();

                    activeWaitRT.Reset();
                    activeWaitRT.ConfirmWait();

                    while (--i >= 0)
                    {
                        if (waitBetweenBattlesRuntimes[i].IsElapsed)
                        {
                            Log.I($"{i + 1} was elapsed. Reset and ignore wait");
                            waitBetweenBattlesRuntimes[i].Reset();
                            waitBetweenBattlesRuntimes[i].IgnoreWait();
                        }
                    }

                    DateTime finishWaitDateTime = DateTime.Now + TimeSpan.FromMilliseconds((int)actions.TimeToWait.TotalMilliseconds);

                    Log.I($"wait until {finishWaitDateTime:dd.MM.yyyy HH:mm:ss.fff}");
                    WaitUntil(() => false,
                        () =>
                        {
                            activeWaitRT.SetWaitingTimeLeft(finishWaitDateTime - DateTime.Now);
                        }, (int)actions.TimeToWait.TotalMilliseconds, 10);

                }
            }

        }

    }
}
