using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {


        private Thread clickerThread;
        private bool isActive = false;
        private bool isRunning = false;
        private bool stopRequested = false;


        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true);
        private ManualResetEvent stopWaitHandle = new ManualResetEvent(false);



        private void OnStartHotkey()
        {
            if (!isActive)
            {
                stopRequested = false;
                stopWaitHandle.Reset();
                pauseEvent.Set();

                clickerThread = new Thread(WorkerLoop)
                {
                    IsBackground = true
                };
                clickerThread.Start();

                isRunning = true;
                isActive = true;
            }
            else
            {
                if (isRunning)
                {
                    OnPaused();
                    pauseEvent.Reset();
                    isRunning = false;
                }
                else
                {
                    OnResumed();
                    pauseEvent.Set();
                    isRunning = true;
                }
            }
        }

        private void OnStopHotkey()
        {
            if (isActive)
            {
                stopRequested = true;
                isRunning = false;
                isActive = false;
                stopWaitHandle.Set();
                pauseEvent.Set();
            }
        }

        private void OnPaused()
        {
            Dispatcher.Invoke(() => InfoLabel.Content = "Thread paused at: " + DateTime.Now);
        }

        private void OnResumed()
        {
            Dispatcher.Invoke(() => InfoLabel.Content = "Thread Resumed at: " + DateTime.Now);
        }

        private void WorkerLoop()
        {
            try
            {
                while (true)
                {
                    Dispatcher.Invoke(() => InfoLabel.Content = "Thread running at: " + DateTime.Now);
                    CheckControl();
                }
            }
            catch (OperationCanceledException)
            {
                Dispatcher.Invoke(() => InfoLabel.Content = "Thread interrupted");
                isRunning = false;
            }
        }

        private void Wait(int milliseconds)
        {
            pauseEvent.Wait();

            if (stopRequested)
                throw new OperationCanceledException();

            int index = WaitHandle.WaitAny(new WaitHandle[]
            {
                stopWaitHandle
            }, milliseconds);

            pauseEvent.Wait();

            if (stopRequested)
                throw new OperationCanceledException();
        }
        private void CheckControl()
        {
            pauseEvent.Wait();

            if (stopRequested)
                throw new OperationCanceledException();
        }












    }
}
