using gca.Classes;
using gca.Classes.Exceptions;
using gca.Enums;
using gca.Script.Tests;
using System.Windows;
using System.Windows.Forms;

namespace gca
{
    public partial class Autobot
    {
        public void PerformTestMode(TestMode testMode)
        {

            switch (testMode)
            {
                case TestMode.MouseMovement1:
                    foreach (var p in TestFunctions.GetCirclePointsClockwise(773, 470, 300))
                    {
                        OnInfoLabelUpdate?.Invoke(p.ToString());
                        SetCursor(p.x, p.y);
                        Wait(1);
                    }
                    break;
                case TestMode.MouseMovement2:
                    foreach (var p in TestFunctions.GetRectangleBorderClockwise(305, 96, 265, 406))
                    {
                        OnInfoLabelUpdate?.Invoke(p.ToString());
                        SetCursor(p.x, p.y);
                        Wait(1);
                    }
                    break;
                case TestMode.MouseMovement3:
                    foreach (var p in TestFunctions.GetSpiral(773, 470, 3000))
                    {
                        OnInfoLabelUpdate?.Invoke(p.ToString());
                        SetCursor(p.x, p.y);
                        Wait(1);
                    }
                    break;
                case TestMode.MouseMove:
                    int x1 = testMouseMoveX1, y1 = testMouseMoveY1, x2 = testMouseMoveX2, y2 = testMouseMoveY2;
                    SetCursor(x1, y1);
                    previousMousePosition.x = x1;
                    previousMousePosition.y = y1;
                    Move(x2, y2);
                    break;
                case TestMode.CrystalsCount:

                    bool darkMode = countCrystalsTestDarkMode;
                    if (!darkMode && !CheckSky())
                    {
                        OnCrystalsCountTestLabelUpdate?.Invoke("Go to gc menu");
                        break;
                    }
                    else if (darkMode && CheckSky())
                    {
                        OnCrystalsCountTestLabelUpdate?.Invoke("Open hero window");
                        break;
                    }

                    int crystals = CountCrystals(!darkMode, out bool hasOranges);

                    string result = $"{(hasOranges ? "Has oranges" : "No oranges")} Result: {crystals}";

                    OnCrystalsCountTestLabelUpdate?.Invoke(result);

                    break;
                case TestMode.Restart:

                    Restart();
                    OnRestartTestLabelUpdate?.Invoke("Restart done");

                    break;
                case TestMode.Reset:

                    Reset();
                    OnRestartTestLabelUpdate?.Invoke("Reset done");

                    break;
                case TestMode.Cleanup:

                    MakeCleanup();
                    OnRestartTestLabelUpdate?.Invoke("Clenup done");

                    break;
                case TestMode.UpgradeHero:

                    if (!CheckGCMenu())
                    {
                        OnUpgradeTestLabelUpdate?.Invoke("Not in gc menu");
                        break;
                    }

                    UpgradeHero();
                    OnUpgradeTestLabelUpdate?.Invoke("Finished hero upgrading");

                    break;
                case TestMode.UpgradeCastle:

                    if (!CheckGCMenu())
                    {
                        OnUpgradeTestLabelUpdate?.Invoke("Not in gc menu");
                        break;
                    }

                    UpgradeTower();

                    OnUpgradeTestLabelUpdate?.Invoke("Finished castle upgrading");

                    break;
                case TestMode.OnlineActions:

                    if (!CheckGCMenu())
                    {
                        OnOnlineActionsTestLabelUpdate?.Invoke("Not in gc menu");
                        break;
                    }

                    OnlineActions onlineActions = OnlineActions.None;

                    if (onlineActionsTest_OpenGuildTest)
                    {
                        onlineActions |= OnlineActions.OpenGuild;
                    }
                    if (onlineActionsTest_OpenRandomProfileFromGuildTest)
                    {
                        onlineActions |= OnlineActions.OpenRandomProfileFromMyGuild;
                    }
                    if (onlineActionsTest_OpenGuildsChatTest)
                    {
                        onlineActions |= OnlineActions.OpenGuildChat;
                    }
                    if (onlineActionsTest_OpenGuildsTopTest)
                    {
                        onlineActions |= OnlineActions.OpenGuildsTop;
                    }
                    if (onlineActionsTest_OpenTopTest)
                    {
                        onlineActions |= OnlineActions.OpenTop;
                    }
                    if (onlineActionsTest_OpenTopSeasonTest)
                    {
                        onlineActions |= OnlineActions.OpenTopSeason;
                    }
                    if (onlineActionsTest_OpenHellSeasonMyTest)
                    {
                        onlineActions |= OnlineActions.OpenTopHellSeasonMy;
                    }
                    if (onlineActionsTest_OpenHellSeasonTest)
                    {
                        onlineActions |= OnlineActions.OpenTopHellSeason;
                    }
                    if (onlineActionsTest_OpenWavesTopMyTest)
                    {
                        onlineActions |= OnlineActions.OpenTopWavesMy;
                    }
                    if (onlineActionsTest_OpenWavesTopTest)
                    {
                        onlineActions |= OnlineActions.OpenTopWavesOverall;
                    }
                    if (onlineActionsTest_CraftStonesTest)
                    {
                        onlineActions |= OnlineActions.CraftStones;
                    }
                    if (onlineActionsTest_DoSaveTest)
                    {
                        onlineActions |= OnlineActions.DoSave;
                    }
                    try
                    {

                        OnOnlineActionsTestLabelUpdate?.Invoke("Doing guild actions");
                        PerformGuildActions(onlineActions);
                        OnOnlineActionsTestLabelUpdate?.Invoke("Doing top actions");
                        PerformTopActions(onlineActions);
                        OnOnlineActionsTestLabelUpdate?.Invoke("Craft stones");
                        PerformCraftStonesActions(onlineActions);
                        OnOnlineActionsTestLabelUpdate?.Invoke("Do save");
                        PerformSaveActions(onlineActions);
                    }
                    catch (OnlineActionsException e)
                    {
                        string message = $"Error happened during online actions: {e.Info}";
                        Log.F(message);
                        OnOnlineActionsTestLabelUpdate?.Invoke(message);
                        break;
                    }

                    OnOnlineActionsTestLabelUpdate?.Invoke("Finished online actions");

                    break;
                case TestMode.ShowGameStatus:

                    OnGameStatusLabelUpdate?.Invoke(string.Empty);
                    List<string> status = new List<string>();
                    G();

                    if (CheckSky(false))
                    {
                        if (CheckEmptyGame(false))
                        {
                            status.Add("Empty acc");
                        }
                        else if (CheckGCMenu(false))
                        {
                            status.Add("In gc menu");
                            if (IsAdForCoinsOnScreen(false))
                            {
                                status.Add("Has ad for coins");
                            }
                            if (IsHellButtonsOpened())
                            {
                                status.Add("Hell buttons open");
                            }
                            else if (IsReplayButtonsOpened())
                            {
                                status.Add("Replay buttons open");
                            }
                        }
                        else if (IsInTown(false))
                        {
                            int forgePos = FindForgePosition(false);
                            status.Add($"Is in town.\nForge position: {forgePos}");
                        }
                        else
                        {
                            if (IsInBattle(false))
                            {
                                status.Add("Is in battle");
                            }
                            if (IsMidWave(false))
                            {
                                status.Add("Is mid wave");
                            }
                            if (IsSkipPanelOnScreen(false))
                            {
                                status.Add("Skip panel on screen");
                            }
                        }
                    }
                    else
                    {
                        if (HasPausePanel(false))
                        {
                            status.Add("Paused");
                            if (HasExitAfterBattlePanel(false))
                            {
                                status.Add("Has Exit after battle button");
                            }
                        }
                        else if (HasExitPanel(false))
                        {
                            status.Add("Exit panel");
                        }
                        else if (IsInForge(false))
                        {
                            status.Add("Is in forge");
                            if (IsOnTopOfForge(false))
                            {
                                status.Add("On top on forge");
                            }
                            else
                            {
                                status.Add("Scrolled down in forge");
                            }
                        }
                        else if (IsLoseABPanelOnScreen(false))
                        {
                            status.Add("Lose AB panel");
                        }
                        else if (CaptchaOnScreen(false))
                        {
                            status.Add("Captcha");
                        }
                        else if (IsHeroPanelOnScreen(false))
                        {
                            status.Add("Hero opened");
                        }
                        else if (IsChooseClassPanelOnScreen(false))
                        {
                            status.Add("Choose class opened");
                        }
                        else if (IsInShop(false))
                        {
                            status.Add("Is in shop");
                        }
                        else if (IsItemOnScreen(false))
                        {
                            status.Add("Item");
                            ItemGrade grade = GetItemGrade();
                            if (grade == ItemGrade.NoItem)
                            {
                                status.Add("Item detected, then not");
                            }
                            else if (grade != ItemGrade.NoItem && grade != ItemGrade.None)
                            {
                                status.Add($"Grade: {grade}");
                            }
                            else if (IsRuneOnScreen(false))
                            {
                                status.Add($"Rune");
                            }
                            else
                            {
                                status.Add($"Couldn't identify item");
                            }

                        }
                        else if (IsInTop(false))
                        {
                            status.Add($"Is in top");
                            TopSection section = GetCurrentTopSection();
                            bool globalTop = IsTopGlobalOpen();
                            status.Add($"Top section {section}.\nGlobal: {globalTop}");
                        }
                        else if (IsInGuild(false))
                        {
                            status.Add($"In guild");
                        }
                        else if (IsSaveGamePanelOpened(false))
                        {
                            status.Add($"Save game");
                        }
                        else if (IsInPlayerProfile(false))
                        {
                            status.Add($"Player profile");
                        }
                        else if (IsInNoxMainMenu(false))
                        {
                            status.Add($"Nox main menu");
                        }
                        else
                        {
                            status.Add($"Couldn't identify");
                        }
                    }

                    if (IsPopupOnScreen(false))
                    {
                        status.Add($"Has popup");
                    }

                    OnGameStatusLabelUpdate?.Invoke(string.Join("\n", status));

                    break;
                case TestMode.SolveCaptcha:
                    if (!CaptchaOnScreen(false))
                    {
                        OnCaptchaTestLabelUpdate?.Invoke("Need to open captcha");
                        return;
                    }
                    SolveCaptcha(true);

                    OnCaptchaTestLabelUpdate?.Invoke("Finished");
                    break;
            }

        }

    }
}
