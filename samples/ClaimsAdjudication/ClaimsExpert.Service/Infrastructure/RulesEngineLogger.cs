using Common.Logging;
using NRules.Diagnostics;

namespace NRules.Samples.ClaimsExpert.Service.Infrastructure
{
    internal class RulesEngineLogger
    {
        private static readonly ILog Log = LogManager.GetLogger<RulesEngineLogger>();

        public RulesEngineLogger(ISessionFactory sessionFactory)
        {
            sessionFactory.Events.RuleFiredEvent += OnRuleFired;
            sessionFactory.Events.ConditionFailedEvent += OnConditionFailedEvent;
            sessionFactory.Events.ActionFailedEvent += OnActionFailedEvent;
        }

        private void OnRuleFired(object sender, AgendaEventArgs args)
        {
            Log.InfoFormat("Rule fired. Rule={0}", args.Rule.Name);
        }

        private void OnConditionFailedEvent(object sender, ConditionErrorEventArgs args)
        {
            Log.ErrorFormat("Condition evaluation failed. Condition={0}, Message={1}", args.Condition, args.Exception);
        }

        private void OnActionFailedEvent(object sender, ActionErrorEventArgs args)
        {
            Log.ErrorFormat("Action evaluation failed. Action={0}, Message={1}", args.Action, args.Exception);
        }
    }
}