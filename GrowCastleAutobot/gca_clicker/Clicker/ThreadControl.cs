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
                if(clickerThread is null)
                {

                    if(!Init(out string message)){
                        InfoLabel.Content = message;
                        return;
                    }

                    stopRequested = false;
                    stopWaitHandle.Reset();
                    pauseEvent.Set();

                    isRunning = true;
                    isActive = true;

                    clickerThread = new Thread(WorkerLoop)
                    {
                        IsBackground = true
                    };

                    clickerThread.Start();
                }

            }
            else
            {
                if (isRunning)
                {
                    pauseEvent.Reset();
                    OnPaused();
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
            Dispatcher.Invoke(() => InfoLabel.Content = "Thread paused at: " + DateTime.Now.ToString("HH:mm:ss.fff"));
        }

        private void OnResumed()
        {
            Dispatcher.Invoke(() => InfoLabel.Content = "Thread Resumed at: " + DateTime.Now.ToString("HH:mm:ss.fff"));
        }

        private void Wait(int milliseconds)
        {
            pauseEvent.Wait();

            if (stopRequested)
                throw new OperationCanceledException();

            stopWaitHandle.WaitOne(milliseconds);

            pauseEvent.Wait();

            if (stopRequested)
                throw new OperationCanceledException();
        }
        private void C()
        {
            pauseEvent.Wait();

            if (stopRequested)
                throw new OperationCanceledException();
        }

    }
}
