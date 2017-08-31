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
        private readonly ISessionInternal _session;

        public ActionContext(ISessionInternal session, Activation activation)
        {
            _session = session;
            Activation = activation;
            IsHalted = false;
        }

        public IRuleDefinition Rule => CompiledRule.Definition;
        public IEnumerable<IFactMatch> Facts => Activation.Facts;
        public ICompiledRule CompiledRule => Activation.CompiledRule;

        public Activation Activation { get; }
        public bool IsHalted { get; private set; }

        public void Halt()
        {
            IsHalted = true;
        }

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

        public IEnumerable<object> GetLinkedKeys()
        {
            return _session.GetLinkedKeys(Activation);
        }

        public object GetLinked(object key)
        {
            return _session.GetLinked(Activation, key);
        }

        public void InsertLinked(object key, object fact)
        {
            _session.InsertLinked(Activation, key, fact);
        }

        public void UpdateLinked(object key, object fact)
        {
            _session.UpdateLinked(Activation, key, fact);
        }

        public void RetractLinked(object key, object fact)
        {
            _session.RetractLinked(Activation, key, fact);
        }

        public object Resolve(Type serviceType)
        {
            var resolutionContext = new ResolutionContext(_session, Rule);
            var service = _session.DependencyResolver.Resolve(resolutionContext, serviceType);
            return service;
        }
    }
}
