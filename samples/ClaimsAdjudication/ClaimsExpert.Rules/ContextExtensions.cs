using NRules.RuleModel;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules
{
    public static class ContextExtensions
    {
        public static void Info(this IContext context, Claim claim, string message)
        {
            InsertAlert(context, 1, claim, message);
        }

        public static void Warning(this IContext context, Claim claim, string message)
        {
            InsertAlert(context, 2, claim, message);
        }

        public static void Error(this IContext context, Claim claim, string message)
        {
            InsertAlert(context, 3, claim, message);
        }

        private static void InsertAlert(IContext context, int severity, Claim claim, string message)
        {
            var alert = new ClaimAlert { Severity = severity, Claim = claim, RuleName = context.Rule.Name, Message = message };
            context.Insert(alert);
        }
    }
}