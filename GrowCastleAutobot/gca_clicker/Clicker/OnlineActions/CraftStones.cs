using gca_clicker.Classes;
using gca_clicker.Classes.Exceptions;
using gca_clicker.Clicker;
using gca_clicker.Enums;
using gca_clicker.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static gca_clicker.Classes.Utils;

namespace gca_clicker
{
    public partial class MainWindow : Window
    {

        public bool IsInTown(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }
            return P(215, 573) == Col(231, 189, 85) &&
            P(241, 574) == Col(231, 189, 85) &&
            P(286, 573) == Col(231, 189, 85) &&
            P(318, 572) == Col(231, 189, 85) &&
            P(373, 575) == Col(231, 189, 85) &&
            P(100, 132) == Cst.White &&
            P(198, 132) == Cst.White &&
            P(23, 121) == Cst.White &&
            P(1408, 160) == Cst.SkyColor;
        }

        public bool IsInForge(bool updateScreen = true)
        {
            if (updateScreen)
            {
                G();
            }

            return P(621, 178) == Col(242, 190, 35) &&
            P(611, 299) == Col(242, 190, 35) &&
            P(615, 421) == Col(242, 190, 35) &&
            P(622, 527) == Col(69, 58, 48) &&
            P(1062, 93) == Col(218, 218, 218) &&
            P(1089, 94) == Col(98, 87, 73) &&
            P(682, 93) == Col(218, 218, 218) &&
            P(630, 93) == Col(98, 87, 73);
        }

        public bool IsOnTopOfForge(bool updateScreen = true)
        {
            if (!IsInForge(updateScreen)) return false;

            return P(946, 173) == Col(68, 255, 218) &&
            P(1073, 173) == Col(68, 255, 218) &&
            P(1073, 294) == Col(244, 86, 233) &&
            P(1073, 415) == Col(244, 86, 233) &&
            P(938, 465) == Col(24, 205, 235) &&
            P(956, 778) == Col(255, 50, 50) &&
            P(1019, 787) == Col(78, 64, 50);
        }


        /// <summary>
        /// 1 to 5 positions. Returns 0 if no forge. -1 if not in town
        /// </summary>
        /// <returns></returns>
        public int FindForgePosition(bool updateScreen = true)
        {
            if (!IsInTown(updateScreen)) return -1;
            currentScreen = Colormode(5, currentScreen);

            if (P(898, 390) == Col(191, 191, 223) && P(901, 386) == Col(191, 191, 223) && P(889, 371) == Col(191, 191, 223)) return 1;
            if (P(1087, 390) == Col(191, 191, 223) && P(1094, 385) == Col(191, 191, 223) && P(1080, 371) == Col(191, 191, 223)) return 2;
            if (P(1281, 390) == Col(191, 191, 223) && P(1286, 385) == Col(191, 191, 223) && P(1272, 371) == Col(191, 191, 223)) return 3;
            if (P(1088, 641) == Col(191, 191, 223) && P(1093, 635) == Col(191, 191, 223) && P(1080, 624) == Col(191, 191, 223)) return 4;
            if (P(1280, 642) == Col(191, 191, 223) && P(1270, 620) == Col(191, 191, 223) && P(1285, 637) == Col(191, 191, 223)) return 5;

            return 0;

        }

        /// <summary>
        /// Call when in town or in gc menu
        /// </summary>
        public void SwitchTown()
        {
            Log.I($"Switch town");
            RCI(37, 120, 195, 152);
            Wait(750);
        }

        /// <summary>
        /// Call when in town or in gc menu.
        /// Will open town, forge, craft stones, then quit and back to gc menu.
        /// </summary>
        /// <exception cref="OnlineActionsException"></exception>
        public void CraftStones()
        {

            Log.I($"{nameof(CraftStones)}");

            if (CheckGCMenu())
            {
                SwitchTown();
            }
            else if (!IsInTown())
            {
                throw new OnlineActionsException($"{nameof(CraftStones)}: was not in town and not in gc menu");
            }

            int forgePos = FindForgePosition();
            Log.I($"Forge position: {forgePos}");

            switch (forgePos)
            {
                case -1:
                    throw new OnlineActionsException($"{nameof(CraftStones)}: was not in town");
                case 0:
                    Log.E($"{nameof(CraftStones)}: forge was not found");
                    SwitchTown();
                    return;
                case 1:
                    RCI(875, 427, 921, 472);
                    break;
                case 2:
                    RCI(1061, 419, 1112, 471);
                    break;
                case 3:
                    RCI(1248, 418, 1300, 472);
                    break;
                case 4:
                    RCI(1061, 670, 1109, 720);
                    break;
                case 5:
                    RCI(1249, 670, 1299, 718);
                    break;
            }
            Wait(500);

            WaitUntil(() => IsInForge(), delegate { }, 3000, 50);

            if (!IsInForge())
            {
                throw new OnlineActionsException($"{nameof(CraftStones)}: forge was not opened");
            }
            Log.I("Forge opened");

            Wait(rand.Next(300, 800));

            if (!IsOnTopOfForge())
            {
                Log.I("Wheel on top of forge");
                for (int i = 0; i < 3; i++)
                {
                    Mouse_Wheel(1111, 444, 150);
                    Wait(300);
                }
                
                WaitUntil(() => IsOnTopOfForge(), delegate { }, 5000, 50);

                Wait(100);

                if (!IsOnTopOfForge())
                {
                    Log.T($"{nameof(CraftStones)}: Couldn't scroll to top of forge");
                    throw new OnlineActionsException($"{nameof(CraftStones)}: Couldn't scroll to top of forge");
                }
            }
            Log.I("Craft A click");

            RCI(1286, 175, 1378, 224); // craft A stones
            Wait(300);
            WaitUntil(() => IsInForge(), delegate { }, 20_000, 50);

            if (!IsInForge())
            {
                Log.T($"{nameof(CraftStones)}: forge gone after crafting A stones");
                throw new OnlineActionsException($"{nameof(CraftStones)}: forge gone after crafting A stones");
            }
            else
            {
                Log.I("crafted");
            }

            Wait(rand.Next(1000, 4000));

            Log.I("Craft S click");
            RCI(1278, 414, 1379, 467); // craft S stones
            Wait(300);
            WaitUntil(() => IsInForge(), delegate { }, 20_000, 50);

            if (!IsInForge())
            {
                Log.T($"{nameof(CraftStones)}: forge gone after crafting S stones");
                throw new OnlineActionsException($"{nameof(CraftStones)}: forge gone after crafting S stones");
            }
            else
            {
                Log.I("crafted");
            }

            Wait(rand.Next(500, 1500));

            Log.I("Quit forge");
            QuitForge();

            Log.I("Switch back to gc menu");
            SwitchTown();

        }

        public void QuitForge()
        {
            if (!IsInForge())
            {
                Log.T($"{nameof(QuitForge)}: forge was not opened");
                throw new OnlineActionsException($"{nameof(QuitForge)}: forge was not opened");
            }

            WaitUntilDeferred(() => IsInTown(), () => RClick(500, 500), 1600, 500);

            if (!IsInTown())
            {
                Log.T($"{nameof(QuitForge)}: couldn't exit from forge");
                throw new OnlineActionsException($"{nameof(QuitForge)}: couldn't exit from forge");
            }
        }

        public void PerformCraftStonesActions(OnlineActions actions)
        {
            if ((actions & OnlineActions.CraftStones) != 0)
            {
                CraftStones();
            }
        }

    }
}
