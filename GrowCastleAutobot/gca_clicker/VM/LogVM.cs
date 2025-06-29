using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.VM
{
    public class LogViewModel : INotifyPropertyChanged
    {
        private string lastLogLines = null!;
        public string LastLogLines
        {
            get => lastLogLines;
            set
            {
                lastLogLines = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void UpdateLastLogs(IEnumerable<string> lastLogLines)
        {
            LastLogLines = string.Join("\n", lastLogLines);
        }

        public void OnPropertyChanged([CallerMemberName] string caller = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
    }
}
