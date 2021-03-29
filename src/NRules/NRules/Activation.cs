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
    /// <seealso cref="IMatch"/>
    /// <seealso cref="IFactMatch"/>
    [DebuggerDisplay("{Rule.Name} FactCount={Tuple.Count}")]
    public class Activation : IMatch
    {
        internal Activation(ICompiledRule compiledRule, Tuple tuple)
        {
            CompiledRule = compiledRule;
            Tuple = tuple;
        }

        /// <summary>
        /// Rule that got activated.
        /// </summary>
        public IRuleDefinition Rule => CompiledRule.Definition;

        /// <summary>
        /// Facts matched by the rule.
        /// </summary>
        public IEnumerable<IFactMatch> Facts => GetMatchedFacts();

        /// <summary>
        /// Event that triggered the match.
        /// </summary>
        public MatchTrigger Trigger { get; private set; }

        internal ICompiledRule CompiledRule { get; }
        internal Tuple Tuple { get; }

        internal bool IsEnqueued { get; set; }
        internal bool HasFired { get; set; }

        internal void OnInsert()
        {
            Trigger = MatchTrigger.Created;
        }

        internal void OnUpdate()
        {
            Trigger = HasFired ? MatchTrigger.Updated : MatchTrigger.Created;
        }

        internal void OnRemove()
        {
            Trigger = HasFired ? MatchTrigger.Removed : MatchTrigger.None;
        }

        internal void OnSelect()
        {
            HasFired = Trigger != MatchTrigger.Removed;
        }

        internal void Clear()
        {
            HasFired = false;
            Trigger = MatchTrigger.None;
        }

        private FactMatch[] GetMatchedFacts()
        {
            var matches = CompiledRule.Declarations.Select(x => new FactMatch(x)).ToArray();
            int index = Tuple.Count - 1;
            var enumerator = Tuple.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int factIndex = CompiledRule.FactMap[index];
                var factMatch = matches[factIndex];
                factMatch.SetFact(enumerator.Current);
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
            public IFactSource Source { get; private set; }

            public void SetFact(Fact fact)
            {
                Type = fact.FactType;
                Value = fact.Object;
                Source = fact.Source;
            }
        }
    }
}