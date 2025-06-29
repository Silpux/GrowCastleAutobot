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
        private bool stopRequested = false;

        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true);
        private ManualResetEvent stopWaitHandle = new ManualResetEvent(false);

        private void OnStartHotkey()
        {
            Log.I($"Start hotkey pressed");
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
                        StopButton.IsEnabled = true;
                        ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Pause.png", UriKind.Relative));

                        stopRequested = false;
                        stopWaitHandle.Reset();
                        pauseEvent.Set();

                        isRunning = true;
                        isActive = true;

                        Log.I($"Starting clicker thread");
                        clickerThread = new Thread(WorkerLoop)
                        {
                            IsBackground = true
                        };

                        ThreadStatusLabel.Content = $"Running";
                        ThreadStatusLabel.Foreground = Brushes.Green;
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
                        ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Continue.png", UriKind.Relative));
                        pauseEvent.Reset();
                        OnPaused();
                        isRunning = false;
                    }
                    else
                    {
                        ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Pause.png", UriKind.Relative));
                        OnResumed();
                        pauseEvent.Set();
                        isRunning = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.C($"Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }

        }

        private void OnStopHotkey()
        {
            Log.W($"Stop hotkey pressed");
            if (isActive)
            {
                Log.I($"Stopped by stop pressing");
                ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Start.png", UriKind.Relative));
                StopButton.IsEnabled = false;

                stopRequested = true;
                isRunning = false;
                isActive = false;
                pauseEvent.Set();
                stopWaitHandle.Set();

                ThreadStatusLabel.Content = $"Stopped";
                ThreadStatusLabel.Foreground = Brushes.Black;
            }
        }

        private void Halt()
        {
            Log.I($"Stop by halt");
            Dispatcher.Invoke(() =>
            {
                ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Start.png", UriKind.Relative));
                StopButton.IsEnabled = false;

                ThreadStatusLabel.Content = $"Stopped";
                ThreadStatusLabel.Foreground = Brushes.Black;
            });
            stopRequested = true;
            isRunning = false;
            isActive = false;
            pauseEvent.Set();
            stopWaitHandle.Set();
            throw new OperationCanceledException();
        }

        private void OnPaused()
        {
            Log.I($"Paused");

            Dispatcher.Invoke(() =>
            {
                InfoLabel.Content = "Thread paused at: " + DateTime.Now.ToString("HH:mm:ss.fff");

                ThreadStatusLabel.Content = $"Paused";
                ThreadStatusLabel.Foreground = Brushes.Orange;
            });
        }

        private void OnResumed()
        {
            Log.I($"Resumed");
            Dispatcher.Invoke(() =>
            {
                InfoLabel.Content = "Thread Resumed at: " + DateTime.Now.ToString("HH:mm:ss.fff");

                ThreadStatusLabel.Content = $"Running";
                ThreadStatusLabel.Foreground = Brushes.Green;
            });
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Log.I($"StartButton_Click");
            OnStartHotkey();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Log.I($"StopButton_Click");
            OnStopHotkey();
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
