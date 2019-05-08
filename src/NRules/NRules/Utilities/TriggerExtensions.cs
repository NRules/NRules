using NRules.RuleModel;

namespace NRules.Utilities
{
    internal static class TriggerExtensions
    {
        public static bool Matches(this MatchTrigger matchTrigger, ActionTrigger actionTrigger)
        {
            return ((int)actionTrigger & (int)matchTrigger) != 0;
        }
    }
}
