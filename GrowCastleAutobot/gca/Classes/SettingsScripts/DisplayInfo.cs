using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca.Classes.SettingsScripts
{
    public class DisplayInfo
    {
        public int Index { get; set; }
        public string Name { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Primary { get; set; }
        public string DisplayName => $"{Name}: {Width}x{Height}" + (Primary ? " (Primary)" : "");
        public override string ToString() => DisplayName;
    }
}
