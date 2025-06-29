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

                lastCleanupTime = DateTime.Now;

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
                            Halt();
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

                        Log.I("Sky not clear. wait 4s");

                        CheckNoxState();

                        bool quitWaiting = false;
                        if(WaitUntil(() => CheckSky() || quitWaiting, () =>
                        {
                            quitWaiting = CloseOverlap();
                        }, 4000, 10))
                        {
                            Log.I("sky cleared. continue");
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
                if (!Dispatcher.HasShutdownStarted && !Dispatcher.HasShutdownFinished)
                {
                    Dispatcher.Invoke(() => InfoLabel.Content = "Thread interrupted");
                }
                isRunning = false;
                clickerThread = null!;
            }
        }

    }
}
