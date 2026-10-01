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
        AncientDragon = 64,

        BeginnerDungeon = 128,
        IntermediateDungeon = 256,
        ExpertDungeon = 512,

        Dragons = GreenDragon | BlackDragon | RedDragon | Sin | LegendaryDragon | BoneDragon | AncientDragon,
        Dungeons = BeginnerDungeon | IntermediateDungeon | ExpertDungeon,

        Any = Dragons | Dungeons,

    }
}
