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
        public IMatch Match => Activation;
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
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (fact == null)
                throw new ArgumentNullException(nameof(fact));
            
            var keyedFact = new KeyValuePair<object, object>(key, fact);
            InsertAllLinked(new[] {keyedFact});
        }

        public void InsertAllLinked(IEnumerable<KeyValuePair<object, object>> keyedFacts)
        {
            _session.InsertLinked(Activation, keyedFacts);
        }

        public void UpdateLinked(object key, object fact)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (fact == null)
                throw new ArgumentNullException(nameof(fact));

            var keyedFact = new KeyValuePair<object, object>(key, fact);
            UpdateAllLinked(new[] {keyedFact});
        }

        public void UpdateAllLinked(IEnumerable<KeyValuePair<object, object>> keyedFacts)
        {
            _session.UpdateLinked(Activation, keyedFacts);
        }

        public void RetractLinked(object key, object fact)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (fact == null)
                throw new ArgumentNullException(nameof(fact));

            var keyedFact = new KeyValuePair<object, object>(key, fact);
            RetractAllLinked(new[] {keyedFact});
        }

        public void RetractAllLinked(IEnumerable<KeyValuePair<object, object>> keyedFacts)
        {
            _session.RetractLinked(Activation, keyedFacts);
        }

        public object Resolve(Type serviceType)
        {
            var resolutionContext = new ResolutionContext(_session, Rule);
            var service = _session.DependencyResolver.Resolve(resolutionContext, serviceType);
            return service;
        }
    }
}
