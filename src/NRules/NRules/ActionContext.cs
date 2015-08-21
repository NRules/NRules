using NRules.RuleModel;

namespace NRules
{
    internal class ActionContext : IContext
    {
        private readonly ICompiledRule _compiledRule;
        private readonly ISession _session;
        private bool _isHalted;

        public ActionContext(ICompiledRule compiledRule, ISession session)
        {
            _compiledRule = compiledRule;
            _session = session;
            _isHalted = false;
        }

        public ICompiledRule CompiledRule { get { return _compiledRule; } }
        public IRuleDefinition Rule { get { return _compiledRule.Definition; } }
        public bool IsHalted { get { return _isHalted; } }

        public void Insert(object fact)
        {
            _session.Insert(fact);
        }

        public bool TryInsert(object fact)
        {
            return _session.TryInsert(fact);
        }

        public void Update(object fact)
        {
            _session.Update(fact);
        }

        public bool TryUpdate(object fact)
        {
            return _session.TryUpdate(fact);
        }

        public void Retract(object fact)
        {
            _session.Retract(fact);
        }

        public bool TryRetract(object fact)
        {
            return _session.TryRetract(fact);
        }

        public void Halt()
        {
            _isHalted = true;
        }
    }
}