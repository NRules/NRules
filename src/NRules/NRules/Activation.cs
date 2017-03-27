using System;
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

    internal class Activation : IActivation, IEquatable<Activation>
    {
        private readonly ICompiledRule _compiledRule;
        private readonly Tuple _tuple;
        private readonly IndexMap _tupleFactMap;
        private readonly Lazy<FactMatch[]> _matchedFacts;

        internal Activation(ICompiledRule compiledRule, Tuple tuple, IndexMap tupleFactMap)
        {
            _compiledRule = compiledRule;
            _tuple = tuple;
            _tupleFactMap = tupleFactMap;
            _matchedFacts = new Lazy<FactMatch[]>(CreateMatchedFacts);
        }

        public IRuleDefinition Rule
        {
            get { return _compiledRule.Definition; }
        }

        public IEnumerable<IFactMatch> Facts
        {
            get { return _matchedFacts.Value; }
        }

        public ICompiledRule CompiledRule
        {
            get { return _compiledRule; }
        }

        public Tuple Tuple
        {
            get { return _tuple; }
        }

        public IndexMap TupleFactMap
        {
            get { return _tupleFactMap; }
        }

        public bool Equals(Activation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.CompiledRule, CompiledRule) && Equals(other.Tuple, Tuple);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Activation)) return false;
            return Equals((Activation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CompiledRule.GetHashCode()*397) ^ Tuple.GetHashCode();
            }
        }

        private FactMatch[] CreateMatchedFacts()
        {
            var matches = _compiledRule.Declarations.Select(x => new FactMatch(x)).ToArray();
            int index = _tuple.Count - 1;
            foreach (var fact in _tuple.Facts)
            {
                int factIndex = _tupleFactMap[index];
                var factMatch = matches[factIndex];
                factMatch.SetFact(fact);
                index--;
            }
            return matches;
        }
    }
}