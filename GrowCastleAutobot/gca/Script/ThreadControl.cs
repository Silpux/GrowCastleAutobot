using gca.Classes;
using gca.Script;
using gca.Enums;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace gca
{
    public partial class Autobot
    {

        private Thread clickerThread = null!;

        public bool IsActive { get; private set; } = false;
        public bool IsRunning { get; private set; } = false;

        private bool stopRequested = true;

        private ManualResetEventSlim pauseEvent = new ManualResetEventSlim(true);
        private ManualResetEvent stopWaitHandle = new ManualResetEvent(true);

        public bool IsPaused { get; private set; } = false;

        private TestMode testMode;

        private string windowName = null!;
        private IEnumerable<WaitBetweenBattlesUserControl> waitBetweenBattlesUserControls = null!;
        private BuildUserControl build = null!;


        private void StartThread(TestMode testMode = TestMode.None)
        {
            try
            {
                if (!IsActive)
                {
                    if (clickerThread is null)
                    {
                        Log.I($"Initialize parameters");
                        if (!InitParameters(out string message))
                        {
                            Log.F($"Init failed with message: {message}");
                            OnInitFailed?.Invoke(message);
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
                        OnInitFailed?.Invoke("Previous clicker thread was not finished.\nIf you keep seeing this error - restart app");
                    }

                }
                else
                {
                    if (clickerThread is null)
                    {
                        Log.F($"Thread is not active and is not null");
                        OnInitFailed?.Invoke($"Clicker thread is null and {nameof(IsActive)} is true.\nCannot run clicker.\nIf you keep seeing this error - restart app");
                        return;
                    }
                    if (IsRunning)
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

                OnInitFailed?.Invoke($"Error happened inside of {nameof(StartThread)}:\n{e.Message}\n\nInner message: {e.InnerException?.Message}\n\nCall stack:\n{e.StackTrace}");
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

        /// <summary>
        /// requests stop, will not stop immediately
        /// </summary>
        private void SetStoppedState()
        {
            if (IsActive)
            {
                OnStopRequested?.Invoke();
                SetDefaultThreadState();

                foreach (var rt in waitBetweenBattlesRuntimes)
                {
                    rt.Dispose();
                }
            }
        }

        private void SetPausedState()
        {
            Log.U($"Paused");
            IsActive = true;
            IsRunning = false;
            stopRequested = false;

            pauseEvent.Reset();
            stopWaitHandle.Reset();

            OnPauseRequested?.Invoke();

        }
        private void SetPausedUI()
        {
            OnPaused?.Invoke();
        }

        private void SetRunningState()
        {
            Log.U($"Run");
            IsActive = true;
            IsRunning = true;
            stopRequested = false;

            pauseEvent.Set();
            stopWaitHandle.Reset();

            SetRunningUI();
        }

        private void SetRunningUI()
        {
            OnStarted?.Invoke(notificationOnlyMode);
        }

        private void OnResumed()
        {
            Log.U($"Resumed");
            SetRunningState();
        }

        public void Start(TestMode testMode)
        {
            StartThread(testMode);
        }
        public void Stop()
        {
            SetStoppedState();
        }

        public void OnStopHotkey()
        {
            Log.U($"Stop hotkey");
            Stop();
        }

        /// <summary>
        /// Call only when clicker is off
        /// </summary>
        private void SetDefaultThreadState()
        {
            IsRunning = false;
            stopRequested = true;
            IsActive = false;
            IsPaused = false;

            pauseEvent.Set();
            stopWaitHandle.Set();
        }
        private void Wait(int milliseconds)
        {

            if (!pauseEvent.IsSet)
            {
                IsPaused = true;
                SetPausedUI();
            }

            pauseEvent.Wait();

            if (IsPaused)
            {
                IsPaused = false;
                SetRunningUI();
            }

            if (stopRequested)
                throw new OperationCanceledException();

            stopWaitHandle.WaitOne(milliseconds);

            if (!pauseEvent.IsSet)
            {
                IsPaused = true;
                SetPausedUI();
            }

            pauseEvent.Wait();

            if (IsPaused)
            {
                IsPaused = false;
                SetRunningUI();
            }

            if (stopRequested)
                throw new OperationCanceledException();

        }
        private void C()
        {
            if (!pauseEvent.IsSet)
            {
                IsPaused = true;
                SetPausedUI();
            }

            pauseEvent.Wait();

            if (IsPaused)
            {
                IsPaused = false;
                SetRunningUI();
            }

            if (stopRequested)
                throw new OperationCanceledException();
        }

    }
}
