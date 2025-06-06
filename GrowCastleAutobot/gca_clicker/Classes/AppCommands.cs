using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace gca_clicker.Classes
{
    public static class AppCommands
    {
        public static readonly RoutedUICommand Save = new("Save", "Save", typeof(AppCommands));
        public static readonly RoutedUICommand Open = new("Open", "Open", typeof(AppCommands));
    }
}
