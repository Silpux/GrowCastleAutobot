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
        OpenTopSeasonMy = 32,
        OpenTopSeasonOverall = 64,
        OpenTopWavesMy = 128,
        OpenTopWavesOverall = 256,
        OpenTopHellMy = 512,
        OpenTopHellOverall = 1024,

        CraftStones = 2048,
        DoSave = 4096,
    }
}
