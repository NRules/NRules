using System;
using System.Collections.Generic;
using NRules.Utilities;

namespace NRules.Aggregators;

/// <summary>
/// Registry of custom aggregator factories.
/// </summary>
public class AggregatorRegistry
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
    internal Type? this[string name]
    {
        get
        {
            return _factoryMap.GetValueOrDefault(name);
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