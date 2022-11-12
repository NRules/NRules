﻿using NRules.Rete;

namespace NRules.Utilities;

internal interface IArguments
{
    object?[] GetValues();
}

internal class LhsExpressionArguments : IArguments
{
    private readonly IArgumentMap _argumentMap;
    private readonly Rete.Tuple? _tuple;
    private readonly Fact? _fact;

    public LhsExpressionArguments(IArgumentMap argumentMap, Rete.Tuple? tuple, Fact? fact)
    {
        _argumentMap = argumentMap;
        _tuple = tuple;
        _fact = fact;
    }

    public object?[] GetValues()
    {
        var args = new object?[_argumentMap.Count];

        if (_tuple != null)
        {
            var index = _tuple.Count - 1;
            foreach (var fact in _tuple.Facts)
            {
                var mappedIndex = _argumentMap.FactMap[index];
                if (mappedIndex >= 0)
                    args[mappedIndex] = fact.Object;

                index--;
            }
        }

        if (_fact is not null)
        {
            var mappedIndex = _argumentMap.FactMap[_argumentMap.Count - 1];
            if (mappedIndex >= 0)
                args[mappedIndex] = _fact.Object;
        }

        return args;
    }
}

internal class ActivationExpressionArguments : IArguments
{
    private readonly IArgumentMap _argumentMap;
    private readonly Activation _activation;

    public ActivationExpressionArguments(IArgumentMap argumentMap, Activation activation)
    {
        _argumentMap = argumentMap;
        _activation = activation;
    }

    public object?[] GetValues()
    {
        var args = new object?[_argumentMap.Count];

        var index = _activation.Tuple.Count - 1;
        foreach (var fact in _activation.Tuple.Facts)
        {
            var mappedIndex = _argumentMap.FactMap[index];
            if (mappedIndex >= 0)
                args[mappedIndex] = fact.Object;

            index--;
        }

        return args;
    }
}
