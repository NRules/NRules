using System;
using System.Collections.Generic;
using NRules.Extensibility;
using NRules.RuleModel;

namespace NRules
{
    internal interface IActionContext : IContext
    {
        ICompiledRule CompiledRule { get; }
        Activation Activation { get; }
        bool IsHalted { get; }
    }

    internal class ActionContext : IActionContext
    {
        private readonly ISession _session;
        private readonly Activation _activation;
        private bool _isHalted;

        public ActionContext(ISession session, Activation activation)
        {
            _session = session;
            _activation = activation;
            _isHalted = false;
        }

        public IRuleDefinition Rule { get { return CompiledRule.Definition; } }
        public IEnumerable<IFactMatch> Facts { get { return _activation.Facts; } }

        public ICompiledRule CompiledRule { get { return _activation.CompiledRule; } }
        public Activation Activation { get { return _activation; } }
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

        public TService Resolve<TService>()
        {
            var service = Resolve(typeof (TService));
            return (TService) service;
        }

        public object Resolve(Type serviceType)
        {
            var resolutionContext = new ResolutionContext(_session, Rule);
            var service = _session.DependencyResolver.Resolve(resolutionContext, serviceType);
            return service;
        }

        public void Halt()
        {
            _isHalted = true;
        }
    }
}
