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
        private bool isSuspended = false;

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
            RestartThread();
        }

        private void RestartThread()
        {
            Debug.WriteLine("RestartThread");
            if (workerThread != null && workerThread.IsAlive)
            {
                cts.Cancel();
                pauseEvent.Set();
                workerThread.Join();
            }
            cts = new CancellationTokenSource();

            workerThread = new Thread(() => Run(cts.Token));
            workerThread.IsBackground = true;
            isElapsed = false;
            isSuspended = true;
            currentDuration = Utils.GetRandomTimeSpan(triggerMin, triggerMax);
            Debug.WriteLine($"Trigger min: {triggerMin}");
            Debug.WriteLine($"Trigger max: {triggerMax}");
            Debug.WriteLine($"Current duration: {currentDuration}");
            pauseEvent.Reset();
            stopwatch.Reset();

            workerThread.Start();
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
            RestartThread();
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

            Debug.WriteLine($"Wait min: {waitMin}");
            Debug.WriteLine($"Wait max: {waitMax}");

            actions.TimeToWait = Utils.GetRandomTimeSpan(waitMin, waitMax);
            Debug.WriteLine($"Result: {actions.TimeToWait}");

            return true;
        }


        private void Run(CancellationToken token)
        {
            Debug.WriteLine("Start thread");
            while (stopwatch.Elapsed < currentDuration)
            {
                pauseEvent.Wait();

                if (token.IsCancellationRequested) return;

                TimeSpan remaining = currentDuration - stopwatch.Elapsed;

                Debug.WriteLine($"Remaining: {remaining}");

                userControl.SetTimeLeft(remaining);

                Thread.Sleep(100);
            }

            Debug.WriteLine("Elapsed");
            isElapsed = true;
            userControl.SetElapsedUI();
        }

    }
}
