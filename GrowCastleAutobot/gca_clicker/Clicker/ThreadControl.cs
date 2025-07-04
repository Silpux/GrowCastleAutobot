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
using static gca_clicker.Classes.Utils;

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

        private bool isPaused = false;

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

                        Log.I($"Starting clicker thread");
                        clickerThread = new Thread(WorkerLoop)
                        {
                            IsBackground = true
                        };

                        SetRunningState();
                        clickerThread.Start();
                    }
                    else
                    {
                        Log.C($"Thread is not active and is not null");
                        MessageBox.Show("Previous clicker thread was not finished.\nIf you keep seeing this error - restart app", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
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
                SetStoppedState();
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

        private void UpdateThreadStatusShortcutLabel()
        {
            if (!isActive)
            {
                ThreadStatusShortcutLabel.Content = $"To start: {StartClickerShortcutBox.Text}";
                return;
            }
            if (isRunning)
            {
                ThreadStatusShortcutLabel.Content = $"To stop: {StopClickerShortcutBox.Text}";
                return;
            }
            ThreadStatusShortcutLabel.Content = string.Empty;
        }

        private void SetStoppedUI()
        {
            Dispatcher.Invoke(() =>
            {
                ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Start.png", UriKind.Relative));
                StopButton.IsEnabled = false;
                StartButton.IsEnabled = true;
                ThreadStatusLabel.Content = $"Stopped";
                ThreadStatusShortcutLabel.Content = $"To start: {StartClickerShortcutBox.Text}";
                ThreadStatusLabel.Foreground = Brushes.Black;
                ABTimerLabel.Content = string.Empty;
            });
        }

        private void SetStoppedState()
        {
            if (isActive)
            {
                isRunning = false;
                stopRequested = true;
                isActive = false;

                pauseEvent.Set();
                stopWaitHandle.Set();

                Dispatcher.Invoke(() =>
                {
                    ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Start.png", UriKind.Relative));
                    StopButton.IsEnabled = false;
                    StartButton.IsEnabled = false;
                    ThreadStatusLabel.Content = $"Stop requested";
                    ThreadStatusShortcutLabel.Content = string.Empty;
                    ThreadStatusLabel.Foreground = Brushes.Red;
                });
            }
        }


        private void SetPausedUI()
        {
            Dispatcher.Invoke(() =>
            {
                ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Continue.png", UriKind.Relative));
                ThreadStatusLabel.Content = $"Paused";
                ThreadStatusShortcutLabel.Content = string.Empty;
                StopButton.IsEnabled = true;
                StartButton.IsEnabled = true;
                ThreadStatusLabel.Foreground = Brushes.Orange;
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
                ThreadStatusLabel.Content = $"Pause requested";
                ThreadStatusShortcutLabel.Content = string.Empty;
                StopButton.IsEnabled = true;
                StartButton.IsEnabled = false;
                ThreadStatusLabel.Foreground = Brushes.Red;
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

            SetRunningUI();
        }

        private void SetRunningUI()
        {
            Dispatcher.Invoke(() =>
            {
                ((Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Pause.png", UriKind.Relative));
                StopButton.IsEnabled = true;
                StartButton.IsEnabled = true;
                ThreadStatusLabel.Content = $"Running";
                ThreadStatusShortcutLabel.Content = $"To stop: {StopClickerShortcutBox.Text}";
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

            if (!pauseEvent.IsSet)
            {
                isPaused = true;
                SetPausedUI();
            }

            pauseEvent.Wait();

            if (isPaused)
            {
                isPaused = false;
                SetRunningUI();
            }

            if (stopRequested)
                throw new OperationCanceledException();

            stopWaitHandle.WaitOne(milliseconds);

            if (!pauseEvent.IsSet)
            {
                isPaused = true;
                SetPausedUI();
            }

            pauseEvent.Wait();

            if (isPaused)
            {
                isPaused = false;
                SetRunningUI();
            }

            if (stopRequested)
                throw new OperationCanceledException();

        }
        private void C()
        {
            if (!pauseEvent.IsSet)
            {
                isPaused = true;
                SetPausedUI();
            }

            pauseEvent.Wait();

            if (isPaused)
            {
                isPaused = false;
                SetRunningUI();
            }

            if (stopRequested)
                throw new OperationCanceledException();
        }

    }
}
