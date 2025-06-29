using gca_clicker.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        private Thread clickerThread;

        private bool isActive = false;
        private bool isRunning = false;
        private bool stopRequested = true;

        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true);
        private ManualResetEvent stopWaitHandle = new ManualResetEvent(true);

        private void StartThread()
        {
            try
            {
                if (!isActive)
                {
                    if (clickerThread is null)
                    {

                        Log.I($"Initialize parameters");
                        if (!Init(out string message))
                        {
                            Log.C($"Init failed with message: {message}");
                            MessageBox.Show(message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                            InfoLabel.Content = message;
                            return;
                        }

                        Log.I($"Finished initialization");
                        SetRunningState();

                        Log.I($"Starting clicker thread");
                        clickerThread = new Thread(WorkerLoop)
                        {
                            IsBackground = true
                        };
                        clickerThread.Start();
                    }
                    else
                    {
                        Log.C($"Thread is not active and is not null");
                        MessageBox.Show("Error occurred. Restart app", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }

                }
                else
                {
                    if (isRunning)
                    {
                        SetPausedState();
                    }
                    else
                    {
                        OnResumed();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.C($"Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Call only inside of clicker thread
        /// </summary>
        /// <exception cref="OperationCanceledException"></exception>
        private void Halt()
        {
            Log.I($"Stop by halt");
            SetStoppedState();
            throw new OperationCanceledException();
        }

        private void SetStoppedState()
        {
            isActive = false;
            isRunning = false;
            stopRequested = true;

            pauseEvent.Set();
            stopWaitHandle.Set();
            clickerThread = null!;

            Dispatcher.Invoke(() =>
            {
                ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Start.png", UriKind.Relative));
                StopButton.IsEnabled = false;
                ThreadStatusLabel.Content = $"Stopped";
                ThreadStatusLabel.Foreground = Brushes.Black;
            });
        }
        private void SetPausedState()
        {
            Log.I($"Paused");
            isActive = true;
            isRunning = false;
            stopRequested = false;

            pauseEvent.Reset();
            stopWaitHandle.Reset();

            Dispatcher.Invoke(() =>
            {
                ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Continue.png", UriKind.Relative));
                InfoLabel.Content = "Thread paused at: " + DateTime.Now.ToString("HH:mm:ss.fff");
                ThreadStatusLabel.Content = $"Paused";
                ThreadStatusLabel.Foreground = Brushes.Orange;
            });
        }

        private void SetRunningState()
        {
            Log.I($"Run");
            isActive = true;
            isRunning = true;
            stopRequested = false;

            pauseEvent.Set();
            stopWaitHandle.Reset();

            Dispatcher.Invoke(() =>
            {
                InfoLabel.Content = "Thread Resumed at: " + DateTime.Now.ToString("HH:mm:ss.fff");
                StopButton.IsEnabled = true;
                ThreadStatusLabel.Content = $"Running";
                ThreadStatusLabel.Foreground = Brushes.Green;
            });
        }

        private void OnResumed()
        {
            Log.I($"Resumed");
            SetRunningState();
        }

        private void OnStartHotkey()
        {
            Log.I($"Start hotkey");
            StartThread();
        }

        private void OnStopHotkey()
        {
            Log.I($"Stop hotkey");
            SetStoppedState();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Log.I($"Start button click");
            StartThread();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Log.I($"Stop button click");
            SetStoppedState();
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
