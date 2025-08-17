using gca.Classes;
using gca.Script;
using gca.Enums;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace gca
{
    public partial class MainWindow : Window
    {

        private Thread clickerThread = null!;

        private bool isActive = false;
        private bool isRunning = false;
        private bool stopRequested = true;

        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true);
        private ManualResetEvent stopWaitHandle = new ManualResetEvent(true);

        private bool isPaused = false;

        private TestMode testMode;

        private void StartThread(TestMode testMode = TestMode.None)
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
                            Log.F($"Init failed with message: {message}");
                            WinAPI.ForceBringWindowToFront(this);
                            System.Windows.MessageBox.Show(message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                            return;
                        }

                        this.testMode = testMode;

                        if (saveScreenshotsOnError)
                        {
                            screenshotCache = new ScreenshotCache(cacheDurationSec, cacheImageQuality, cacheIntervalMs);
                        }
                        else
                        {
                            screenshotCache = new ScreenshotCache(120, 10, 300);
                        }

                        UpdateRestartTime();
                        UpdateCleanupTime();

                        Log.I($"Finished initialization");

                        Log.U($"Starting clicker thread");
                        clickerThread = new Thread(WorkerLoop)
                        {
                            IsBackground = true
                        };

                        SetRunningState();

                        clickerStopwatch = Stopwatch.StartNew();
                        clickerThread.Start();
                    }
                    else
                    {
                        Log.F($"Thread is not active and is not null");
                        WinAPI.ForceBringWindowToFront(this);
                        System.Windows.MessageBox.Show("Previous clicker thread was not finished.\nIf you keep seeing this error - restart app", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }

                }
                else
                {
                    if (clickerThread is null)
                    {
                        Log.F($"Thread is not active and is not null");
                        WinAPI.ForceBringWindowToFront(this);
                        System.Windows.MessageBox.Show($"Clicker thread is null and {nameof(isActive)} is true.\nCannot run clicker.\nIf you keep seeing this error - restart app", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                        return;
                    }
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
            catch (Exception e)
            {
                Log.F($"Unhandled exception:\n{e.Message}\n\nInner message: {e.InnerException?.Message}\n\nCall stack:\n{e.StackTrace}");
                SetStoppedState();

                WinAPI.ForceBringWindowToFront(this);
                System.Windows.MessageBox.Show($"Error happened inside of {nameof(StartThread)}:\n{e.Message}\n\nInner message: {e.InnerException?.Message}\n\nCall stack:\n{e.StackTrace}", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Call only from inside of clicker thread
        /// </summary>
        /// <exception cref="OperationCanceledException"></exception>
        private void Halt()
        {
            Log.I($"Stop by halt");
            SetStoppedState();
            throw new OperationCanceledException();
        }

        /// <summary>
        /// Don't call from inside of clicker thread
        /// </summary>
        private void RestartThread()
        {
            Log.I($"Starting new thread");
            restartRequested = false;

            StartThread();
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

        /// <summary>
        /// requests stop, will not stop immediately
        /// </summary>
        private void SetStoppedState()
        {
            if (isActive)
            {
                SetDefaultThreadState();

                Dispatcher.BeginInvoke(() =>
                {
                    ((System.Windows.Controls.Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Start.png", UriKind.Relative));
                    StopButton.IsEnabled = false;
                    StartButton.IsEnabled = false;
                    ThreadStatusLabel.Content = $"Stop requested";
                    ThreadStatusShortcutLabel.Content = string.Empty;
                    ThreadStatusLabel.Foreground = System.Windows.Media.Brushes.Red;
                    SetBackground(Cst.StopRequestedBackground, false);
                });

                foreach (var rt in waitBetweenBattlesRuntimes)
                {
                    rt.Dispose();
                }
            }
        }

        /// <summary>
        /// call after clicker thread stopped
        /// </summary>
        private void SetStoppedUI()
        {
            Dispatcher.BeginInvoke(() =>
            {
                ((System.Windows.Controls.Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Start.png", UriKind.Relative));
                StopButton.IsEnabled = false;
                StartButton.IsEnabled = true;
                ThreadStatusLabel.Content = $"Stopped";
                ThreadStatusShortcutLabel.Content = $"To start: {StartClickerShortcutBox.Text}";
                ThreadStatusLabel.Foreground = System.Windows.Media.Brushes.Black;
                ABTimerLabel.Content = string.Empty;
                NextCleanupTimeLabel.Content = string.Empty;
                NextRestartTimeLabel.Content = string.Empty;
                ResetColors();
                SetCanvasChildrenState(TestCanvas, true);
                SetCanvasChildrenState(OnlineActionsTestCanvas, true);

                SetBackground(Cst.DefaultBackground, false);
                AddWaitBetweenBattlesButton.IsEnabled = true;
                EnableAllWaitsBetweenBattlesButton.IsEnabled = true;
                DisableAllWaitsBetweenBattlesButton.IsEnabled = true;
                SaveSettingsButton.IsEnabled = true;
                LoadSettingsButton.IsEnabled = true;

                foreach (var wbbuc in GetWaitBetweenBattlesUserControls())
                {
                    wbbuc.EnableUI();
                }
            });
        }

        private void SetPausedState()
        {
            Log.U($"Paused");
            isActive = true;
            isRunning = false;
            stopRequested = false;

            pauseEvent.Reset();
            stopWaitHandle.Reset();

            ((System.Windows.Controls.Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Continue.png", UriKind.Relative));
            ThreadStatusLabel.Content = $"Pause requested";
            ThreadStatusShortcutLabel.Content = string.Empty;
            StopButton.IsEnabled = true;
            StartButton.IsEnabled = false;
            ThreadStatusLabel.Foreground = System.Windows.Media.Brushes.Red;

            SetBackground(Cst.PauseRequestedBackground, false);

        }
        private void SetPausedUI()
        {
            Dispatcher.BeginInvoke(() =>
            {
                ((System.Windows.Controls.Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Continue.png", UriKind.Relative));
                ThreadStatusLabel.Content = $"Paused";
                ThreadStatusShortcutLabel.Content = string.Empty;
                StopButton.IsEnabled = true;
                StartButton.IsEnabled = true;
                ThreadStatusLabel.Foreground = System.Windows.Media.Brushes.Orange;

                SetBackground(Cst.PausedBackground, false);
            });
        }

        private void SetRunningState()
        {
            Log.U($"Run");
            isActive = true;
            isRunning = true;
            stopRequested = false;

            pauseEvent.Set();
            stopWaitHandle.Reset();

            SetRunningUI();
        }

        private void SetRunningUI()
        {
            Dispatcher.BeginInvoke(() =>
            {
                ((System.Windows.Controls.Image)StartButton.Content).Source = new BitmapImage(new Uri("Images/Pause.png", UriKind.Relative));
                StopButton.IsEnabled = true;
                StartButton.IsEnabled = true;
                ThreadStatusLabel.Content = $"Running";
                ThreadStatusShortcutLabel.Content = $"To stop: {StopClickerShortcutBox.Text}";
                ThreadStatusLabel.Foreground = System.Windows.Media.Brushes.Green;
                SetCanvasChildrenState(TestCanvas, false);
                SetCanvasChildrenState(OnlineActionsTestCanvas, false);

                if (notificationOnlyMode)
                {
                    SetBackground(Cst.NotificationOnlyModeBackground, false);
                }
                else
                {
                    SetBackground(Cst.RunningBackground, false);
                }
                AddWaitBetweenBattlesButton.IsEnabled = false;
                EnableAllWaitsBetweenBattlesButton.IsEnabled = false;
                DisableAllWaitsBetweenBattlesButton.IsEnabled = false;
                SaveSettingsButton.IsEnabled = false;
                LoadSettingsButton.IsEnabled = false;

                foreach (var wbbuc in GetWaitBetweenBattlesUserControls())
                {
                    wbbuc.DisableUI();
                }
            });
        }

        private void OnResumed()
        {
            Log.U($"Resumed");
            SetRunningState();
        }

        private void OnStartHotkey()
        {
            Log.U($"Start hotkey");
            StartThread();
        }

        private void OnStopHotkey()
        {
            Log.U($"Stop hotkey");
            SetStoppedState();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Log.U($"Start button click");
            StartThread();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Log.U($"Stop button click");
            SetStoppedState();
        }

        /// <summary>
        /// Call only when clicker is off
        /// </summary>
        private void SetDefaultThreadState()
        {
            isRunning = false;
            stopRequested = true;
            isActive = false;
            isPaused = false;

            pauseEvent.Set();
            stopWaitHandle.Set();
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
