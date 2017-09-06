using System.Collections.Generic;
using System.Linq;
using NRules.Rete;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    /// <summary>
    /// Represents a match of all rule's conditions.
    /// </summary>
    public interface IActivation
    {
        /// <summary>
        /// Rule that got activated.
        /// </summary>
        IRuleDefinition Rule { get; }

        /// <summary>
        /// Facts matched by the rule.
        /// </summary>
        IEnumerable<IFactMatch> Facts { get; }
    }

    internal class Activation : IActivation
    {
        internal Activation(ICompiledRule compiledRule, Tuple tuple, IndexMap tupleFactMap)
        {
            CompiledRule = compiledRule;
            Tuple = tuple;
            TupleFactMap = tupleFactMap;
        }

        public IRuleDefinition Rule => CompiledRule.Definition;
        public IEnumerable<IFactMatch> Facts => GetMatchedFacts();

        public ICompiledRule CompiledRule { get; }
        public Tuple Tuple { get; }
        public IndexMap TupleFactMap { get; }

        private FactMatch[] GetMatchedFacts()
        {
            var matches = CompiledRule.Declarations.Select(x => new FactMatch(x)).ToArray();
            int index = Tuple.Count - 1;
            foreach (var fact in Tuple.Facts)
            {
                int factIndex = TupleFactMap[index];
                var factMatch = matches[factIndex];
                factMatch.SetFact(fact);
                index--;
            }
            return matches;
        }
    }
}