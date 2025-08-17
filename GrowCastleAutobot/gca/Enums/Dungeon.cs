namespace gca.Enums
{
    [Flags]
    public enum Dungeon
    {
        None = 0,

        GreenDragon = 1,
        BlackDragon = 2,
        RedDragon = 4,
        Sin = 8,
        LegendaryDragon = 16,
        BoneDragon = 32,

        BeginnerDungeon = 64,
        IntermediateDungeon = 128,
        ExpertDungeon = 256,

        Dragons = GreenDragon | BlackDragon | RedDragon | Sin | LegendaryDragon | BoneDragon,
        Dungeons = BeginnerDungeon | IntermediateDungeon | ExpertDungeon,

        Any = Dragons | Dungeons,

    }
}
