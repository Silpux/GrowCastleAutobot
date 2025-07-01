using gca_clicker.Classes;
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


        private void WorkerLoop()
        {
            int prevFrameStatus = 0;
            try
            {
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
                            
                            if(DateTime.Now - lastCleanupTime > TimeSpan.FromSeconds(cleanupInterval))
                            {
                                MakeCleanup();
                                continue;
                            }
                            else
                            {
                                if(prevFrameStatus == 1 && dungeonNumber > 6)
                                {
                                    ShowBattleLength();
                                }
                                CheckAdForX3();
                                WaitForAdAndWatch();
                                TryUpgradeHero();
                                TryUpgradeTower();
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
                        if(WaitUntil(() => CheckSky() || quitWaiting, () =>
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
                MessageBox.Show($"Error happened while executing clicker:\n{e.Message}", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
        }

    }
}
