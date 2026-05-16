namespace gca.Enums
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

        PressDeck = 1024,
        CraftStones = 2048,
        DoSave = 4096,

        GuildActions = OpenGuild | OpenGuildsTop | OpenRandomProfileFromMyGuild | OpenGuildChat,
        TopActions = OpenTop | OpenTopSeason | OpenTopWavesMy | OpenTopWavesOverall | OpenTopHellSeasonMy | OpenTopHellSeason,

        AnyAction = OpenGuild | OpenTop | PressDeck | CraftStones | DoSave,

        All = GuildActions | TopActions | PressDeck | CraftStones | DoSave
    }
}
