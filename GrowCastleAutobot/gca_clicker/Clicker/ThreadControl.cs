using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                if (!isActive)
                {
                    if (clickerThread is null)
                    {

                        if (!Init(out string message))
                        {
                            MessageBox.Show(message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }

        }

        private void OnStopHotkey()
        {
            if (isActive)
            {
                Debug.WriteLine("STOP");
                stopRequested = true;
                isRunning = false;
                isActive = false;
                pauseEvent.Set();
                stopWaitHandle.Set();
            }
        }

        private void Halt()
        {
            Debug.WriteLine("halt");
            stopRequested = true;
            isRunning = false;
            isActive = false;
            pauseEvent.Set();
            stopWaitHandle.Set();
            throw new OperationCanceledException();
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
