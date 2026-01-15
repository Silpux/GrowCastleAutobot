using gca.Classes;
using gca.Classes.Exceptions;
using gca.Script;
using gca.Enums;
using gca.Structs;
using System.Windows;

namespace gca
{
    public partial class Autobot
    {

        private bool backgroundMode;
        private nint hwnd;

        private Bitmap currentScreen = null!;

        private bool restartRequested = false;

        private void WorkerLoop()
        {

            try
            {
                G();

                if (testMode != TestMode.None)
                {
                    PerformTestMode(testMode);
                    Halt();
                }

                if (notificationOnlyMode)
                {
                    Log.I("Notification only mode is enabled");

                    bool currentCountMode = false;

                    Loop30Detector loopDetector = log30DetectionsInNotificationMode ? new() : null!;

                    while (true)
                    {
                        bool notificationReady = notifyOn30Crystals && DateTime.Now - last30CrystalsNotificationTime > notifyOn30CrystalsInterval;
                        bool audioCheckReady = playAudioOn30Crystals && DateTime.Now - last30CrystalsAudioPlayTime > playAudioOn30CrystalsInterval;

                        currentCountMode = !currentCountMode;

                        int crystalsCount = CountCrystals(currentCountMode, out _);

                        if (log30DetectionsInNotificationMode && loopDetector.Process(crystalsCount))
                        {
                            CrystalsCollectionTimeLogger.AddTimeStampWithDiff(Cst.CRYSTALS_COLLECTED_TIME_FILE_PATH);
                        }

                        if ((notificationReady || audioCheckReady) && crystalsCount >= 30)
                        {
                            if (notificationReady)
                            {
                                ShowBalloon("", "30 crystals collected");
                                last30CrystalsNotificationTime = DateTime.Now;
                            }
                            if (audioCheckReady)
                            {
                                string file = audio30crystalsIndex == 0 ? Cst.AUDIO_30_CRYSTALS_1_PATH : Cst.AUDIO_30_CRYSTALS_2_PATH;
                                OnPlayAudio?.Invoke(file, audio30crystalsIndex == 0 ? playAudio1On30CrystalsVolume : playAudio2On30CrystalsVolume);
                                last30CrystalsAudioPlayTime = DateTime.Now;
                            }
                        }
                        Wait(500);
                    }
                }

                int prevFrameStatus = 0;
                while (true)
                {
                    if (CaptchaOnScreen())
                    {
                        if (solveCaptcha)
                        {
                            SolveCaptcha(CAPTCHA_TEST_MODE);
                        }
                        else
                        {
                            StopScript();
                        }
                        G();
                    }

                    if (CheckSky(false))
                    {

                        if (CheckGCMenu(false))
                        {
                            Log.I("Gc menu detected");

                            if (CheckEmptyGame(false))
                            {
                                Wait(500);
                                if (CheckEmptyGame())
                                {
                                    Log.F("Empty game detected. Halt");
                                    if (saveScreenshotsOnError)
                                    {
                                        screenshotCache.SaveAllToFolder(Cst.SCREENSHOT_ERROR_SCREEN_CACHE_PATH);
                                    }
                                    Halt();
                                }
                            }

                            if (nextCleanupTime < DateTime.Now)
                            {

                                try
                                {
                                    if (doSaveBeforeCleanup)
                                    {
                                        DoSave();
                                    }
                                }
                                catch (OnlineActionsException e)
                                {
                                    Log.E($"Error happened while doing save before cleanup: {e.Info}");
                                }

                                MakeCleanup();
                                continue;
                            }
                            else if (doRestarts && DateTime.Now > nextRestartDt)
                            {
                                Log.I("Restart time's up. Do restart");
                                Wait(300);
                                Restart();
                                Log.I("Restart made");
                                Wait(200);
                                continue;
                            }
                            else
                            {
                                if (prevFrameStatus == 1 && dungeonFarm && dungeonToFarm.IsDungeon())
                                {
                                    ShowBattleLength();
                                }
                                CheckAdForX3();
                                WaitForAdAndWatch();
                                TryUpgradeHero();
                                TryUpgradeTower();

                                foreach (var rt in waitBetweenBattlesRuntimes)
                                {
                                    rt.Suspend();
                                }

                                if(!ignoreWaitsBetweenBattlesOnX3FromAd || DateTime.Now - x3Timer > TimeSpan.FromSeconds(1205))
                                {
                                    DoActionsBeforeReplay();
                                }

                                foreach (var rt in waitBetweenBattlesRuntimes)
                                {
                                    if (!rt.IsActive && !rt.IsElapsed)
                                    {
                                        rt.Start();
                                    }
                                    else
                                    {
                                        rt.Resume();
                                    }
                                }
                                Log.I($"Check ended");
                                OnChangeBackground?.Invoke(Cst.RunningBackground);

                                Replay();

                                prevFrameStatus = 2;
                            }
                        }
                        else
                        {

                            if (IsInTown(false))
                            {
                                SwitchTown();
                                prevFrameStatus = 0;
                                continue;
                            }

                            if (dungeonFarm)
                            {
                                ActivateHeroesDun();
                            }
                            else
                            {
                                ActivateHeroes();
                            }
                            prevFrameStatus = 1;
                        }
                    }
                    else
                    {

                        Log.I("Sky not clear. wait 4s");

                        bool quitWaiting = false;
                        if (WaitUntil(() => CheckSky() || quitWaiting, () =>
                        {
                            quitWaiting = CloseOverlap();
                        }, 4000, 10))
                        {
                            if (!quitWaiting)
                            {
                                Log.I("sky cleared. continue");
                            }
                        }
                        else if (!CheckSky())
                        {
                            Log.W("4s waited");
                            EscClickStart();
                        }

                        prevFrameStatus = 0;

                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.U($"Stop clicker thread. Time running: {RunningTime:hh\\:mm\\:ss\\.fff}");
                OnStopped?.Invoke();
            }
            catch (OnlineActionsException e)
            {
                string message = $"Stop clicker. Online action exception: {e.Info}\n\nTime running: {RunningTime:hh\\:mm\\:ss\\.fff}";
                Log.F(message);
                OnStopped?.Invoke();
                OnFailed?.Invoke(message);
            }
            catch (Exception e)
            {
                Log.F($"Unhandled exception:\n{e.Message}\n\nInner message: {e.InnerException?.Message}\n\nCall stack:\n{e.StackTrace}\n\nTime running: {RunningTime:hh\\:mm\\:ss\\.fff}");
                OnStopped?.Invoke();
                OnFailed?.Invoke($"Error happened while executing clicker:\n{e.Message}\n\nInner message: {e.InnerException?.Message}\n\nCall stack:\n{e.StackTrace}\n\nTime running: {RunningTime:hh\\:mm\\:ss\\.fff}\n\nCurrent time: {DateTime.Now:dd.MM.yyyy HH:mm:ss.fff}");
            }

            clickerThread = null!;
            SetDefaultThreadState();

            if (restartRequested && testMode == TestMode.None)
            {
                RestartThread();
            }

        }

        public void DoActionsBeforeReplay()
        {
            Log.I($"Check {waitBetweenBattlesRuntimes.Count} timers");
            for (int i = waitBetweenBattlesRuntimes.Count - 1; i >= 0; i--)
            {
                if (waitBetweenBattlesRuntimes[i].IsElapsed)
                {
                    WaitBetweenBattlesRuntime activeRT = waitBetweenBattlesRuntimes[i];

                    Log.I($"{i + 1} elapsed");
                    ActionBetweenBattle actions = activeRT.GetActions();

                    activeRT.Reset();

                    while (--i >= 0)
                    {
                        if (waitBetweenBattlesRuntimes[i].IsElapsed)
                        {
                            Log.I($"{i + 1} was elapsed. Reset and ignore wait");
                            waitBetweenBattlesRuntimes[i].Reset();
                            waitBetweenBattlesRuntimes[i].IgnoreWait();
                        }
                    }

                    OnChangeBackground?.Invoke(Cst.WaitingBackground);

                    if (actions.OnlineActionsBeforeWait)
                    {
                        Log.I("Doing online actions before wait");
                        PerformOnlineActions(actions.OnlineActions, activeRT, "Doing online actions before wait");
                    }

                    activeRT.ConfirmWait();

                    int totalMs = (int)actions.TimeToWait.TotalMilliseconds;
                    DateTime finishWaitDateTime = DateTime.Now + TimeSpan.FromMilliseconds(totalMs);

                    Log.I($"wait until {finishWaitDateTime:dd.MM.yyyy HH:mm:ss.fff}");
                    WaitUntil(() => false,
                        () =>
                        {
                            TimeSpan left = finishWaitDateTime - DateTime.Now;
                            double percent = left.TotalMilliseconds / totalMs;
                            activeRT.UserControl.SetWaitingTimeLeft(finishWaitDateTime - DateTime.Now, percent);
                        }, (int)actions.TimeToWait.TotalMilliseconds, 37);

                    actions = activeRT.GetActions();

                    if (actions.OnlineActionsAfterWait)
                    {
                        Log.I("Doing online actions after wait");
                        PerformOnlineActions(actions.OnlineActions, activeRT, "Doing online actions after wait");
                    }

                }
            }

        }

        public void PerformOnlineActions(OnlineActions actions, WaitBetweenBattlesRuntime rt, string status)
        {
            try
            {

                if ((actions & OnlineActions.AnyAction) == 0)
                {
                    Log.I($"No online actions to do");
                    return;
                }

                Wait(100);

                List<(string name, Action action)> methods = new(4);
                if ((actions & (OnlineActions.OpenGuild)) != 0)
                {
                    methods.Add(("Guild", () =>
                    {
                        PerformGuildActions(actions);
                    }
                    ));
                }

                if ((actions & (OnlineActions.OpenTop)) != 0)
                {
                    methods.Add(("Top", () =>
                    {
                        PerformTopActions(actions);
                    }
                    ));
                }

                if ((actions & (OnlineActions.CraftStones)) != 0)
                {
                    methods.Add(("Craft", () =>
                    {
                        PerformCraftStonesActions(actions);
                    }
                    ));
                }

                if ((actions & (OnlineActions.DoSave)) != 0)
                {
                    methods.Add(("Save", () =>
                    {
                        PerformSaveActions(actions);
                    }
                    ));
                }

                List<string> actionsSequence = new List<string>(6);
                int n = methods.Count;

                while (n > 1)
                {
                    n--;
                    int k = rand.Next(n + 1);
                    (string, Action) t = methods[k];
                    methods[k] = methods[n];
                    methods[n] = t;
                    actionsSequence.Insert(0, methods[n].name);
                }

                if (methods.Count > 0)
                {
                    actionsSequence.Insert(0, methods[0].name);
                }

                rt.UserControl.SetOnlineActionsUI(status, string.Join(", ", actionsSequence));

                foreach (var method in methods)
                {
                    method.action();
                }
            }
            catch (OnlineActionsException e)
            {
                Log.E($"Error on online action: {e.Info}. Will skip");
            }

        }

    }
}
