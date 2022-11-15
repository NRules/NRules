using System;
using System.Collections.Generic;

namespace NRules.Aggregators
{
    public interface IAggregatorRegistry
    {
        Type this[string name] { get; }

        void RegisterFactory(string name, Type factoryType);
    }

    /// <summary>
    /// Registry of custom aggregator factories.
    /// </summary>
    public class AggregatorRegistry : IAggregatorRegistry
    {
        private readonly Dictionary<string, Type> _factoryMap = new();

        internal AggregatorRegistry()
        {
        }

        /// <summary>
        /// Looks up custom aggregator factory type by the aggregator name.
        /// </summary>
        /// <param name="name">Name of the custom aggregator.</param>
        /// <returns>Custom aggregator type or <c>null</c> if a given aggregator type is not registered.</returns>
        public Type this[string name]
        {
            get
            {
                _factoryMap.TryGetValue(name, out var factoryType);
                return factoryType;
            }
        }

        /// <summary>
        /// Registers a custom aggregator factory type, so that rules that use it can be successfully compiled.
        /// </summary>
        /// <param name="name">Name of the custom aggregator.</param>
        /// <param name="factoryType">Custom aggregator factory type.</param>
        public void RegisterFactory(string name, Type factoryType)
        {
            _factoryMap.Add(name, factoryType);
        }
    }
}