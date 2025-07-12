using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Classes
{
    public class WaitBetweenBattleSetting
    {
        
        public bool IsChecked { get; set; }

        public int TriggerMin {  get; set; }
        public int TriggerMax { get; set; }

        public int WaitMin {  get; set; }
        public int WaitMax { get; set; }


    }
}
