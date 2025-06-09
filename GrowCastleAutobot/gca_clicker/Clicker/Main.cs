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
        private bool solveCaptcha;


        private Bitmap currentScreen;

        private void WorkerLoop()
        {
            try
            {
                Getscreen();

                dungeonFarm = true;
                dungeonNumber = 7;

                deathAltar = true;
                dungeonStartCastOnBoss = true;

                PerformDungeonStart();

                Halt();

                while (true)
                {
                    //Dispatcher.Invoke(() => InfoLabel.Content = "Thread running at: " + DateTime.Now.ToString("HH:mm:ss.fff"));
                    //C();

                    if (CaptchaOnScreen())
                    {
                        if (solveCaptcha)
                        {

                        }
                        else
                        {

                        }
                    }

                    if (CheckSky())
                    {

                        if (CheckGCMenu())
                        {
                            Debug.WriteLine("Gc menu");
                        }
                        else
                        {

                            Debug.WriteLine("Gc");
                        }


                    }
                    else
                    {

                        Debug.WriteLine("Closed");
                    }


                    Wait(100);
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
