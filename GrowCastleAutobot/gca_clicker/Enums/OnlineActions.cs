using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Enums
{
    [Flags]
    public enum OnlineActions
    {
        None = 0,

        OpenGuild = 1,
        OpenGuildsTop = 2,
        OpenRandomProfileFromMyGuild = 4,
        OpenGuildChat = 8,

        OpenTop = 16,
        OpenTopSeason = 32,
        OpenTopWavesMy = 64,
        OpenTopWavesOverall = 128,
        OpenTopHellSeasonMy = 256,
        OpenTopHellSeason= 512,

        CraftStones = 1024,
        DoSave = 2048,
    }
}
