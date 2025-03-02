using NRules.Rete;

namespace NRules.Utilities;

internal interface IArguments
{
    object?[] GetValues();
}

internal class LhsExpressionArguments(IArgumentMap argumentMap, Tuple? tuple, Fact? fact) : IArguments
{
    public object?[] GetValues()
    {
        var args = new object?[argumentMap.Count];

        if (tuple != null)
        {
            var index = tuple.Count - 1;
            var enumerable = tuple.GetEnumerator();
            while (enumerable.MoveNext())
            {
                var mappedIndex = argumentMap.FactMap[index];
                if (mappedIndex >= 0)
                    args[mappedIndex] = enumerable.Current.Object;

                index--;
            }
        }

        if (fact != null)
        {
            var mappedIndex = argumentMap.FactMap[argumentMap.Count - 1];
            if (mappedIndex >= 0)
                args[mappedIndex] = fact.Object;
        }

        return args;
    }
}

internal class ActivationExpressionArguments(IArgumentMap argumentMap, Activation activation) : IArguments
{
    public object?[] GetValues()
    {
        var args = new object?[argumentMap.Count];

        var index = activation.Tuple.Count - 1;
        var enumerable = activation.Tuple.GetEnumerator();
        while (enumerable.MoveNext())
        {
            var mappedIndex = argumentMap.FactMap[index];
            if (mappedIndex >= 0)
                args[mappedIndex] = enumerable.Current.Object;

            index--;
        }

        return args;
    }
}
