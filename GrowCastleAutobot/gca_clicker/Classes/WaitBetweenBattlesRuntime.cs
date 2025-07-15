using gca_clicker.Classes.SettingsScripts;
using gca_clicker.Clicker;
using gca_clicker.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Classes
{
    public class WaitBetweenBattlesRuntime
    {

        private TimeSpan triggerMin;
        private TimeSpan triggerMax;

        private TimeSpan currentDuration;

        private TimeSpan waitMin;
        private TimeSpan waitMax;

        private bool isElapsed = false;
        private bool isActive = false;

        private bool isSuspended = false;

        public bool IsActive => isActive;
        public bool IsElapsed => isElapsed;

        private Thread workerThread = null!;
        private CancellationTokenSource cts = null!;
        private ManualResetEventSlim pauseEvent = new(false);

        private Stopwatch stopwatch = new();

        WaitBetweenBattlesUserControl userControl;

        public WaitBetweenBattlesRuntime(WaitBetweenBattlesSetting setting)
        {

            triggerMin = TimeSpan.FromSeconds(setting.TriggerMin);
            triggerMax = TimeSpan.FromSeconds(setting.TriggerMax);

            waitMin = TimeSpan.FromSeconds(setting.WaitMin);
            waitMax = TimeSpan.FromSeconds(setting.WaitMax);

            userControl = setting.UserControl;

            Init();
        }

        private void FinishCurrentThread()
        {
            if (workerThread != null && workerThread.IsAlive)
            {
                cts.Cancel();
                pauseEvent.Set();
            }
            isActive = false;
            isElapsed = false;
        }

        public void Dispose()
        {
            FinishCurrentThread();
            userControl.ResetUIQueued();
            workerThread = null!;
        }

        private void Init()
        {
            if(workerThread != null && workerThread.IsAlive)
            {
                throw new InvalidOperationException("Previous thread is not finished!");
            }

            pauseEvent.Set();
            cts = new CancellationTokenSource();
            currentDuration = Utils.GetRandomTimeSpan(triggerMin, triggerMax);
            stopwatch.Reset();
            workerThread = new Thread(() => Run(cts.Token));
            workerThread.IsBackground = true;
            isActive = false;
            isElapsed = false;

        }

        private void ResetThread()
        {
            FinishCurrentThread();
            Init();
        }

        public void Start()
        {
            if (isActive)
            {
                throw new InvalidOperationException("Thread is avtive! Cannot start");
            }
            isElapsed = false;
            isActive = true;
            isSuspended = false;
            UpdateUI();
            workerThread.Start();
            pauseEvent.Set();
            stopwatch.Start();
        }

        public void Suspend()
        {
            pauseEvent.Reset();
            stopwatch.Stop();
            isSuspended = true;
            UpdateUI();
        }

        public void Resume()
        {
            pauseEvent.Set();
            stopwatch.Start();
            isSuspended = false;
            UpdateUI();
        }

        public void Reset()
        {
            ResetThread();
            userControl.ResetUI();
        }



        public void UpdateUI()
        {
            if (isElapsed)
            {
                userControl.SetElapsedUI();
            }
            else if (isSuspended)
            {
                userControl.SetSuspendedUI();
            }
            else
            {
                userControl.SetRunningUI();
            }
        }

        public bool GetActions(out ActionBetweenBattle actions)
        {
            actions = new ActionBetweenBattle();

            if (!isElapsed)
            {
                return false;
            }

            actions.TimeToWait = Utils.GetRandomTimeSpan(waitMin, waitMax);

            return true;
        }

        public void SetWaitingTimeLeft(TimeSpan time)
        {
            userControl.SetWaitingTimeLeft(time);
        }

        private void Run(CancellationToken token)
        {
            isElapsed = false;
            isSuspended = false;
            while (stopwatch.Elapsed < currentDuration)
            {
                pauseEvent.Wait();

                TimeSpan remaining = currentDuration - stopwatch.Elapsed;
                userControl.SetTriggerTimeLeft(remaining);

                Thread.Sleep(5);

                if (token.IsCancellationRequested)
                {
                    return;
                }

            }

            isElapsed = true;
            stopwatch.Stop();
            userControl.SetElapsedUI();
            isActive = false;
        }

        public void ConfirmWait()
        {
            userControl.SetActiveWaitUI();
        }
        public void IgnoreWait()
        {
            userControl.SetIgnoredWaitUI();
        }

    }
}
