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

        /// <summary>
        /// Retrieves a custom state object associated with the activation.
        /// </summary>
        /// <typeparam name="T">Type of state object.</typeparam>
        /// <param name="key">Key with which the state object is associated.</param>
        /// <returns>State object that's associated with the key or <c>default</c> value if there is no object associated with the key.</returns>
        T GetState<T>(object key);

        /// <summary>
        /// Associates a custom state object with the activation.
        /// </summary>
        /// <param name="key">State object's key.</param>
        /// <param name="value">State object's value.</param>
        void SetState(object key, object value);

        /// <summary>
        /// Removes a custom state object from the activation.
        /// </summary>
        /// <typeparam name="T">Type of state object.</typeparam>
        /// <param name="key">Key with which the state object is associated.</param>
        /// <returns>State object that was associated with the key or <c>default</c> value if there was no object associated with the key.</returns>
        T RemoveState<T>(object key);
    }

    [DebuggerDisplay("{Rule.Name} FactCount={Tuple.Count}")]
    internal class Activation : IActivation
    {
        private Dictionary<object, object> _stateMap;
        
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

        public T GetState<T>(object key)
        {
            if (_stateMap != null && _stateMap.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default(T);
        }

        public void SetState(object key, object value)
        {
            if (_stateMap == null) _stateMap = new Dictionary<object, object>();
            _stateMap[key] = value;
        }

        public T RemoveState<T>(object key)
        {
            if (_stateMap != null && _stateMap.TryGetValue(key, out var value))
            {
                var state = (T)value;
                _stateMap.Remove(key);
                return state;
            }
            return default(T);
        }

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