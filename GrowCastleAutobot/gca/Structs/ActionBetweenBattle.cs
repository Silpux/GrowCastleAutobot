using gca.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gca.Structs
{
    public struct ActionBetweenBattle
    {

        public TimeSpan TimeToWait { get; set; }
        public OnlineActions OnlineActions { get; set; }

        public bool OnlineActionsBeforeWait { get; set; }
        public bool OnlineActionsAfterWait { get; set; }

    }
}
