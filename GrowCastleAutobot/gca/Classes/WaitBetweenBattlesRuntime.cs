using gca.Classes.SettingsScripts;
using gca.Enums;
using gca.Structs;
using System.Diagnostics;

namespace gca.Classes
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

        private bool openGuild;
        private double openGuildChance;

        private bool openGuildsTop;
        private double openGuildsTopChance;

        private bool openGuildsChat;
        private double openGuildsChatChance;

        private bool openRandomProfileFromGuild;
        private double openRandomProfileFromGuildChance;

        private bool openTop;
        private double openTopChance;

        private bool openTopSeason;
        private double openTopSeasonChance;

        private bool openHellTopSeason;
        private double openHellTopSeasonChance;

        private bool openHellTopSeasonMy;
        private double openHellTopSeasonMyChance;

        private bool openTopWavesOverall;
        private double openTopWavesOverallChance;

        private bool openTopWavesOverallMy;
        private double openTopWavesOverallMyChance;

        private bool craftStones;
        private double craftStonesChance;

        private bool doSave;
        private double doSaveChance;

        private bool onlineActionsBeforeWait;
        private bool onlineActionsAfterWait;

        private bool isSuspended = false;

        public bool IsActive => isActive;
        public bool IsElapsed => isElapsed;

        private bool ignoreWait;

        private Thread workerThread = null!;
        private CancellationTokenSource cts = null!;
        private ManualResetEventSlim pauseEvent = new(false);

        private Stopwatch stopwatch = new();

        private WaitBetweenBattlesUserControl userControl;
        public WaitBetweenBattlesUserControl UserControl => userControl;

        public WaitBetweenBattlesRuntime(WaitBetweenBattlesSetting setting)
        {

            triggerMin = TimeSpan.FromSeconds(setting.TriggerMin);
            triggerMax = TimeSpan.FromSeconds(setting.TriggerMax);

            waitMin = TimeSpan.FromSeconds(setting.WaitMin);
            waitMax = TimeSpan.FromSeconds(setting.WaitMax);

            userControl = setting.UserControl;

            ignoreWait = userControl.IgnoreWait;

            openGuild = setting.OpenGuild;
            openGuildChance = (double)setting.OpenGuildChance / 100;

            openGuildsTop = setting.OpenGuildsTop;
            openGuildsTopChance = (double)setting.OpenGuildsTopChance / 100;

            openGuildsChat = setting.OpenGuildsChat;
            openGuildsChatChance = (double)setting.OpenGuildsChatChance / 100;

            openRandomProfileFromGuild = setting.OpenRandomProfileInGuild;
            openRandomProfileFromGuildChance = (double)setting.OpenRandomProfileInGuildChance / 100;

            openTop = setting.OpenTop;
            openTopChance = (double)setting.OpenTopChance / 100;

            openHellTopSeason = setting.OpenTopHellSeason;
            openHellTopSeasonChance = (double)setting.OpenTopHellSeasonChance / 100;

            openHellTopSeasonMy = setting.OpenTopHellSeasonMy;
            openHellTopSeasonMyChance = (double)setting.OpenTopHellSeasonMyChance / 100;

            openTopWavesOverall = setting.OpenTopWavesOverall;
            openTopWavesOverallChance = (double)setting.OpenTopWavesOverallChance / 100;

            openTopWavesOverallMy = setting.OpenTopWavesOverallMy;
            openTopWavesOverallMyChance = (double)setting.OpenTopWavesOverallMyChance / 100;

            craftStones = setting.CraftStones;
            craftStonesChance = (double)setting.CraftStonesChance / 100;

            doSave = setting.DoSave;
            doSaveChance = (double)setting.DoSaveChance / 100;

            onlineActionsBeforeWait = setting.BeforeWait;
            onlineActionsAfterWait = setting.AfterWait;

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
            if (workerThread != null && workerThread.IsAlive)
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
            if (workerThread != null)
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

        public ActionBetweenBattle GetActions()
        {
            ActionBetweenBattle actions = new ActionBetweenBattle();

            if (ignoreWait)
            {
                actions.TimeToWait = TimeSpan.Zero;
            }
            else
            {
                actions.TimeToWait = Utils.GetRandomTimeSpan(waitMin, waitMax);
            }

            OnlineActions onlineActions = OnlineActions.None;

            Random rand = new();

            if (openGuild && rand.NextDouble() < openGuildChance)
            {
                onlineActions |= OnlineActions.OpenGuild;

                if (openGuildsTop && rand.NextDouble() < openGuildChance)
                {
                    onlineActions |= OnlineActions.OpenGuildsTop;
                }
                if (openGuildsChat && rand.NextDouble() < openGuildChance)
                {
                    onlineActions |= OnlineActions.OpenGuildChat;
                }
                if (openRandomProfileFromGuild && rand.NextDouble() < openRandomProfileFromGuildChance)
                {
                    onlineActions |= OnlineActions.OpenRandomProfileFromMyGuild;
                }

            }

            if (openTop && rand.NextDouble() < openTopChance)
            {
                onlineActions |= OnlineActions.OpenTop;

                if (openTopSeason && rand.NextDouble() < openTopSeasonChance)
                {
                    onlineActions |= OnlineActions.OpenTopSeason;
                }
                if (openHellTopSeason && rand.NextDouble() < openHellTopSeasonChance)
                {
                    onlineActions |= OnlineActions.OpenTopHellSeason;
                }
                if (openHellTopSeasonMy && rand.NextDouble() < openHellTopSeasonMyChance)
                {
                    onlineActions |= OnlineActions.OpenTopHellSeasonMy;
                }
                if (openTopWavesOverall && rand.NextDouble() < openTopWavesOverallChance)
                {
                    onlineActions |= OnlineActions.OpenTopWavesOverall;
                }
                if (openTopWavesOverallMy && rand.NextDouble() < openTopWavesOverallMyChance)
                {
                    onlineActions |= OnlineActions.OpenTopWavesMy;
                }
            }

            if (craftStones && rand.NextDouble() < craftStonesChance)
            {
                onlineActions |= OnlineActions.CraftStones;
            }

            if (doSave && rand.NextDouble() < doSaveChance)
            {
                onlineActions |= OnlineActions.DoSave;
            }

            actions.OnlineActions = onlineActions;

            actions.OnlineActionsBeforeWait = onlineActionsBeforeWait;
            actions.OnlineActionsAfterWait = onlineActionsAfterWait;

            return actions;
        }

        private void Run(CancellationToken token)
        {
            isElapsed = false;
            isSuspended = false;
            while (stopwatch.Elapsed < currentDuration)
            {
                pauseEvent.Wait();

                TimeSpan remaining = currentDuration - stopwatch.Elapsed;

                double percent = 1 - stopwatch.ElapsedMilliseconds / currentDuration.TotalMilliseconds;

                userControl.SetTriggerTimeLeft(remaining, percent);

                Thread.Sleep(37);

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
