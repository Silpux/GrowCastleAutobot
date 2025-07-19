using gca_clicker.Classes;
using gca_clicker.Classes.Exceptions;
using gca_clicker.Clicker;
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

        public bool IsInGuild()
        {
            Getscreen();
            return Pxl(256, 461) == Col(168, 43, 42);
        }

        public bool IsInTop()
        {
            Getscreen();

            return Pxl(1022, 91) == Col(218, 218, 218) &&
            Pxl(1084, 91) == Col(218, 218, 218) &&
            Pxl(1110, 92) == Col(98, 87, 73) &&
            Pxl(995, 127) == Col(98, 87, 73) &&
            Pxl(1239, 91) == Col(242, 190, 35) &&
            Pxl(1374, 109) == Col(235, 170, 23);
        }

        public bool IsInTown()
        {
            Getscreen();
            return Pxl(215, 573) == Col(231, 189, 85) &&
            Pxl(241, 574) == Col(231, 189, 85) &&
            Pxl(286, 573) == Col(231, 189, 85) &&
            Pxl(318, 572) == Col(231, 189, 85) &&
            Pxl(373, 575) == Col(231, 189, 85) &&
            Pxl(100, 132) == Cst.White &&
            Pxl(198, 132) == Cst.White &&
            Pxl(23, 121) == Cst.White &&
            Pxl(1408, 160) == Cst.SkyColor;
        }

        public bool IsInForge()
        {
            Getscreen();

            return Pxl(621, 178) == Col(242, 190, 35) &&
            Pxl(611, 299) == Col(242, 190, 35) &&
            Pxl(615, 421) == Col(242, 190, 35) &&
            Pxl(622, 527) == Col(69, 58, 48) &&
            Pxl(1062, 93) == Col(218, 218, 218) &&
            Pxl(1089, 94) == Col(98, 87, 73) &&
            Pxl(682, 93) == Col(218, 218, 218) &&
            Pxl(630, 93) == Col(98, 87, 73);
        }

        public bool IsInPlayerProfile()
        {
            Getscreen();
            return Pxl(405, 643) == Col(98, 87, 73) &&
            Pxl(1079, 639) == Col(98, 87, 73) &&
            Pxl(401, 278) == Col(75, 62, 52) &&
            Pxl(667, 316) == Col(236, 192, 49) &&
            Pxl(938, 316) == Col(52, 251, 61);
        }

        public int FindForgePosition()
        {
            if (!IsInTown()) return -1;
            currentScreen = Colormode(5, currentScreen);

            if (Pxl(898, 390) == Col(191, 191, 223) && Pxl(901, 386) == Col(191, 191, 223) && Pxl(889, 371) == Col(191, 191, 223)) return 1;
            if (Pxl(1087, 390) == Col(191, 191, 223) && Pxl(1094, 385) == Col(191, 191, 223) && Pxl(1080, 371) == Col(191, 191, 223)) return 2;
            if (Pxl(1281, 390) == Col(191, 191, 223) && Pxl(1286, 385) == Col(191, 191, 223) && Pxl(1272, 371) == Col(191, 191, 223)) return 3;
            if (Pxl(1088, 641) == Col(191, 191, 223) && Pxl(1093, 635) == Col(191, 191, 223) && Pxl(1080, 624) == Col(191, 191, 223)) return 4;
            if (Pxl(1280, 642) == Col(191, 191, 223) && Pxl(1270, 620) == Col(191, 191, 223) && Pxl(1285, 637) == Col(191, 191, 223)) return 5;

            return 0;

        }

        public void OpenGuild()
        {

            Log.I("Open guild");
            RandomClickIn(1355, 411, 1422, 469);
            Wait(500);

            WaitUntil(() => IsInGuild() || CheckSky(), Getscreen, 20_000, 50);

            if (CheckSky())
            {
                Log.I("Can't open guild");
                throw new OnlineActionsException("Couldn't open guild");
            }

        }

        public void CloseGuild()
        {
            if (!IsInGuild())
            {
                throw new OnlineActionsException($"Called {nameof(CloseGuild)}, but guild wasn't open");
            }

            RClick(500, 500);
            Wait(100);
        }

        public void OpenGuildChat()
        {
            if (!IsInGuild())
            {
                throw new OnlineActionsException($"{nameof(OpenGuildChat)} called outside of guild");
            }

            RandomClickIn(257, 553, 288, 603);
            Wait(500);

            WaitUntil(IsInGuild, Getscreen, 20_000, 50);

        }

        public void OpenGuildsTop()
        {
            if (!IsInGuild())
            {
                throw new OnlineActionsException($"{nameof(OpenGuildsTop)} called outside of guild");
            }

            RandomClickIn(257, 670, 289, 715);
            Wait(500);

            WaitUntil(IsInGuild, Getscreen, 20_000, 50);

        }

        /// <summary>
        /// Call when list of guild players is open
        /// </summary>
        public void CheckRandomProfileInGuild()
        {

            for(int i = 0; i < 3; i++)
            {
                Mouse_Wheel(738, 584, 150);
                Wait(200);
            }
            Wait(300);

            //RandomClickIn(364, 403, 1118, 698);

            Wait(300);

            WaitUntil(() => IsInPlayerProfile() || IsInGuild(), delegate { }, 20_000, 50);

            if (IsInPlayerProfile())
            {
                Wait(rand.Next(1000, 3000));
                RClick(500, 500);
            }

        }

    }
}
