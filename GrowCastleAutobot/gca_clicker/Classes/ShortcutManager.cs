using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace gca_clicker.Classes
{
    public static class ShortcutManager
    {
        public static Dictionary<string, KeyGesture> Shortcuts { get; set; } = new()
        {
            { "Save", new KeyGesture(Key.S, ModifierKeys.Control) },
            { "Open", new KeyGesture(Key.O, ModifierKeys.Control) }
        };
    }
}
