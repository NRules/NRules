using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NRules.Rete;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    /// <summary>
    /// Represents a match of all rule's conditions.
    /// </summary>
    [DebuggerDisplay("{Rule.Name} FactCount={Tuple.Count}")]
    public class Activation : IMatch
    {
        private Dictionary<object, object> _stateMap;
        
        internal Activation(ICompiledRule compiledRule, Tuple tuple, IndexMap factMap)
        {
            CompiledRule = compiledRule;
            Tuple = tuple;
            FactMap = factMap;
        }

        /// <summary>
        /// Rule that got activated.
        /// </summary>
        public IRuleDefinition Rule => CompiledRule.Definition;

        /// <summary>
        /// Facts matched by the rule.
        /// </summary>
        public IEnumerable<IFactMatch> Facts => GetMatchedFacts();

        internal ICompiledRule CompiledRule { get; }
        internal Tuple Tuple { get; }
        internal IndexMap FactMap { get; }

        internal T GetState<T>(object key)
        {
            if (_stateMap != null && _stateMap.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default(T);
        }

        internal void SetState(object key, object value)
        {
            if (_stateMap == null) _stateMap = new Dictionary<object, object>();
            _stateMap[key] = value;
        }

        private FactMatch[] GetMatchedFacts()
        {
            var matches = CompiledRule.Declarations.Select(x => new FactMatch(x)).ToArray();
            int index = Tuple.Count - 1;
            foreach (var fact in Tuple.Facts)
            {
                int factIndex = FactMap[index];
                var factMatch = matches[factIndex];
                factMatch.SetFact(fact);
                index--;
            }
            return matches;
        }

        private class FactMatch : IFactMatch
        {
            public FactMatch(Declaration declaration)
            {
                Declaration = declaration;
            }

            public Declaration Declaration { get; }
            public Type Type { get; private set; }
            public object Value { get; private set; }

            public void SetFact(Fact fact)
            {
                Type = fact.FactType.AsType();
                Value = fact.Object;
            }
        }
    }
}