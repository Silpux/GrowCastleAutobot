using gca_clicker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca_clicker.Structs
{
    public struct ActionBetweenBattle
    {

        public TimeSpan TimeToWait { get; set; }
        public OnlineActions OnlineActions { get; set; }

        public bool OnlineActionsBeforeWait { get; set; }
        public bool OnlineActionsAfterWait { get; set; }

    }
}
