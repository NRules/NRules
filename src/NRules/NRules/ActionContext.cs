using System.Collections.Generic;
using NRules.Extensibility;
using NRules.RuleModel;

namespace NRules
{
    internal interface IActionContext : IContext
    {
        ICompiledRule CompiledRule { get; }
        IActivation Activation { get; }
        bool IsHalted { get; }
    }

    internal class ActionContext : IActionContext
    {
        private readonly ISession _session;
        private readonly ICompiledRule _compiledRule;
        private readonly IActivation _activation;
        private bool _isHalted;

        public ActionContext(ISession session, ICompiledRule compiledRule, IActivation activation)
        {
            _session = session;
            _compiledRule = compiledRule;
            _activation = activation;
            _isHalted = false;
        }

        public IRuleDefinition Rule { get { return _compiledRule.Definition; } }
        public IEnumerable<IFactMatch> Facts { get { return _activation.Facts; } }

        public ICompiledRule CompiledRule { get { return _compiledRule; } }
        public IActivation Activation { get { return _activation; } }
        public bool IsHalted { get { return _isHalted; } }

        public void Insert(object fact)
        {
            _session.Insert(fact);
        }

        public void InsertAll(IEnumerable<object> facts)
        {
            _session.InsertAll(facts);
        }

        public bool TryInsert(object fact)
        {
            return _session.TryInsert(fact);
        }

        public void Update(object fact)
        {
            _session.Update(fact);
        }

        public void UpdateAll(IEnumerable<object> facts)
        {
            _session.UpdateAll(facts);
        }

        public bool TryUpdate(object fact)
        {
            return _session.TryUpdate(fact);
        }

        public void Retract(object fact)
        {
            _session.Retract(fact);
        }

        public void RetractAll(IEnumerable<object> facts)
        {
            _session.RetractAll(facts);
        }

        public bool TryRetract(object fact)
        {
            return _session.TryRetract(fact);
        }

        public TService Resove<TService>()
        {
            var resolutionContext = new ResolutionContext(_session, _compiledRule.Definition);
            var service = _session.DependencyResolver.Resolve(resolutionContext, typeof (TService));
            return (TService) service;
        }

        public void Halt()
        {
            _isHalted = true;
        }
    }
}
