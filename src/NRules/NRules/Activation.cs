using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Diagnostics;
using NRules.Rete;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    /// <summary>
    /// Rule activation, which represents a match of all rule's conditions.
    /// </summary>
    public interface IActivation
    {
        /// <summary>
        /// Rule related to the event.
        /// </summary>
        IRuleDefinition Rule { get; }

        /// <summary>
        /// Tuple related to the event.
        /// </summary>
        IEnumerable<FactInfo> Facts { get; }
    }

    internal class Activation : IActivation, IEquatable<Activation>
    {
        private readonly ICompiledRule _compiledRule;
        private readonly Tuple _tuple;
        private readonly IndexMap _tupleFactMap;

        internal Activation(ICompiledRule compiledRule, Tuple tuple, IndexMap tupleFactMap)
        {
            _compiledRule = compiledRule;
            _tuple = tuple;
            _tupleFactMap = tupleFactMap;
        }

        public IRuleDefinition Rule
        {
            get { return _compiledRule.Definition; }
        }

        public IEnumerable<FactInfo> Facts
        {
            get { return _tuple.OrderedFacts.Select(f => new FactInfo(f)).ToArray(); }
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
    }
}