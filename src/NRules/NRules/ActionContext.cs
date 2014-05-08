using System;
using NRules.RuleModel;

namespace NRules
{
    internal class ActionContext : IContext
    {
        private readonly ISession _session;

        public ActionContext(ISession session)
        {
            _session = session;
            IsHalted = false;
        }

        public bool IsHalted { get; set; }

        public void Insert(object fact)
        {
            _session.Insert(fact);
        }

        public void Update(object fact)
        {
            _session.Update(fact);
        }

        public void Update<T>(T fact, Action<T> updateAction)
        {
            updateAction(fact);
            _session.Update(fact);
        }

        public void Retract(object fact)
        {
            _session.Retract(fact);
        }

        public void Halt()
        {
            IsHalted = true;
        }
    }
}