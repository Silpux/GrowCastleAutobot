namespace gca.Classes.SettingsScripts
{
    public class BuildSettings
    {
        public bool this[int i]
        {
            get => SlotsToPress[i];
            set => SlotsToPress[i] = value;
        }
        public bool[] SlotsToPress { get; set; } = new bool[15];
        public int[] PressOrder { get; set; } = new int[15] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
        public List<int> SingleClickSlots { get; set; } = new();
        public int PwSlot { get; set; } = -1;
        public int SmithSlot { get; set; } = -1;
        public int OrcBandSlot { get; set; } = -1;
        public int MiliitaryFSlot { get; set; } = -1;
        public int ChronoSlot { get; set; } = -1;

    }
}
