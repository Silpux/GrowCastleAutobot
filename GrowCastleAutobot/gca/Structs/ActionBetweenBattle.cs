using gca.Enums;

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
