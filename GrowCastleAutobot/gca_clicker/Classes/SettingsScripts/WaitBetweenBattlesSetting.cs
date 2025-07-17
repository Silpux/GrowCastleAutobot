using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace gca_clicker.Classes.SettingsScripts
{
    public class WaitBetweenBattlesSetting
    {
        
        public bool IsChecked { get; set; }

        public int TriggerMin {  get; set; }
        public int TriggerMax { get; set; }

        public int WaitMin { get; set; }
        public int WaitMax { get; set; }

        public bool OpenGuild {  get; set; }
        public int OpenGuildChance {  get; set; }

        public bool OpenGuildsTop {  get; set; }
        public int OpenGuildsTopChance { get; set; }

        public bool OpenGuildsChat {  get; set; }
        public int OpenGuildsChatChance { get; set; }

        public bool OpenRandomProfileInGuild { get; set; }
        public int OpenRandomProfileInGuildChance { get; set; }

        public bool OpenTop { get; set; }
        public int OpenTopChance { get; set; }

        public bool OpenTopSeason { get; set; }
        public int OpenTopSeasonChance { get; set; }

        public bool OpenTopHellSeason { get; set; }
        public int OpenTopHellSeasonChance { get; set; }

        public bool OpenTopHellSeasonMy { get; set; }
        public int OpenTopHellSeasonMyChance { get; set; }

        public bool OpenTopWavesOverall { get; set; }
        public int OpenTopWavesOverallChance { get; set; }

        public bool OpenTopWavesOverallMy { get; set; }
        public int OpenTopWavesOverallMyChance { get; set; }

        public bool CraftStones { get; set; }
        public int CraftStonesChance { get; set; }

        public bool DoSave { get; set; }
        public int DoSaveChance { get; set; }

        [JsonIgnore]
        public WaitBetweenBattlesUserControl UserControl { get; set; } = null!;

    }
}
