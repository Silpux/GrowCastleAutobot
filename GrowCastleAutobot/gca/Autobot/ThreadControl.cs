using gca.Classes;
using gca.Script;
using gca.Enums;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

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
                            OnFailed?.Invoke(message);
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
                        OnFailed?.Invoke("Previous clicker thread was not finished.\nIf you keep seeing this error - restart app");
                    }

                }
                else
                {
                    if (clickerThread is null)
                    {
                        Log.F($"Thread is not active and is not null");
                        OnFailed?.Invoke($"Clicker thread is null and {nameof(IsActive)} is true.\nCannot run clicker.\nIf you keep seeing this error - restart app");
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

                OnFailed?.Invoke($"Error happened inside of {nameof(StartThread)}:\n{e.Message}\n\nInner message: {e.InnerException?.Message}\n\nCall stack:\n{e.StackTrace}");
            }

        }

        public bool LDConsoleReboot()
        {

            ProcessStartInfo psiQuit = new ProcessStartInfo
            {
                FileName = ldConsolePath,
                Arguments = $"quit --name \"{windowName}\"",

                UseShellExecute = false,
                CreateNoWindow = true,

                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using Process quitProcess = new Process
            {
                StartInfo = psiQuit
            };

            Log.I("Start quit process");
            quitProcess.Start();

            string stdout = quitProcess.StandardOutput.ReadToEnd();
            string stderr = quitProcess.StandardError.ReadToEnd();

            quitProcess.WaitForExit();

            string output = stdout;

            if (!string.IsNullOrWhiteSpace(stderr))
            {
                output += Environment.NewLine + stderr;
            }

            if(output.Length > 0)
            {
                Log.E($"Quit failed with message: {output}");
                return false;
            }

            if(!WaitUntil(() => !WinAPI.WindowExists(hwnd), delegate { }, 30_000, 100))
            {
                Log.E($"Cannot finish current LDPlayer instance");
                return false;
            }

            Log.I("Finished LDPlayer instance");

            ProcessStartInfo psiLaunch = new ProcessStartInfo
            {
                FileName = ldConsolePath,
                Arguments = $"launch --name \"{windowName}\"",

                UseShellExecute = false,
                CreateNoWindow = true,

                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using Process launchProcess = new Process
            {
                StartInfo = psiLaunch
            };

            Log.I("Start launch process");
            launchProcess.Start();

            stdout = launchProcess.StandardOutput.ReadToEnd();
            stderr = launchProcess.StandardError.ReadToEnd();

            launchProcess.WaitForExit();

            output = stdout;

            if (!string.IsNullOrWhiteSpace(stderr))
            {
                output += Environment.NewLine + stderr;
            }

            if (output.Length > 0)
            {
                Log.E($"Launch failed with message: {output}");
                return false;
            }


            Log.I("Wait for LDPlayer window to appear");
            if (WaitUntil(() => GetLDPlayerWindow() != IntPtr.Zero, delegate { }, 30_000, 1_000))
            {
                hwnd = GetLDPlayerWindow();
                GetRenderHwnd(hwnd);
                Log.I($"LDPlayer appeared: HWND: {hwnd}, Render: {renderHwnd}");

                if(hwnd != IntPtr.Zero && renderHwnd != IntPtr.Zero)
                {

                    if(WaitUntil(() => WinAPI.IsWindowVisible(renderHwnd), delegate { }, 300_000, 1_000))
                    {
                        Log.I("LDPlayer loaded");
                        Wait(3_000);
                        SetPos(hwnd);
                        Wait(3_000);
                        return true;
                    }
                    else
                    {
                        Log.E("LDPlayer didn't load");
                    }
                }
                else
                {
                    Log.E("Couldn't get render hwnd");
                }
            }
            else
            {
                Log.E("LDPlayer window did not appear");
            }
            return false;
        }
        private void SetPos(IntPtr hwnd)
        {
            Utils.SetDefaultNoxState(hwnd);
            WinAPI.RestoreWindow(hwnd);
            WinAPI.SetWindowPos(hwnd, hwnd, 0, 0, Cst.WINDOW_WIDTH + 1, Cst.WINDOW_HEIGHT + 1, WinAPI.SWP_NOZORDER);
            Utils.SetDefaultNoxState(hwnd);
        }

        public void LDConsoleCloseGC()
        {
            Log.I("Close GC");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = ldConsolePath,
                Arguments = $"killapp --name \"{windowName}\" --packagename \"com.raongames.growcastle\"",

                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process killGCProcess = new Process
            {
                StartInfo = psi
            };

            killGCProcess.Start();
            killGCProcess.WaitForExit();
        }

        public void LDConsoleOpenGC()
        {
            Log.I("Open GC");
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = ldConsolePath,
                Arguments = $"runapp --name \"{windowName}\" --packagename \"com.raongames.growcastle\"",

                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process startGCProcess = new Process
            {
                StartInfo = psi
            };

            startGCProcess.Start();
            startGCProcess.WaitForExit();

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
