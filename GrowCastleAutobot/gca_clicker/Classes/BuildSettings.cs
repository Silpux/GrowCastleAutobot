using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Classes
{
    public class BuildSettings
    {
        public bool this[int i]
        {
            get => SlotsToPress[i];
            set => SlotsToPress[i] = value;
        }
        public bool[] SlotsToPress { get; set; } = new bool[15];
        public List<int> SingleClickSlots { get; set; } = new();
        public int PwSlot { get; set; } = -1;
        public int SmithSlot { get; set; } = -1;
        public int OrcBandSlot { get; set; } = -1;
        public int MiliitaryFSlot { get; set; } = -1;
        public int ChronoSlot { get; set; } = -1;

    }
}
