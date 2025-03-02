using System;
using System.Collections.Generic;

namespace NRules.AgendaFilters;

internal class KeyChangeAgendaFilter(IReadOnlyList<IActivationExpression<object>> keySelectors) 
    : IStatefulAgendaFilter
{
    private readonly Dictionary<Activation, ChangeKeys> _changeKeys = new();

    public bool Accept(AgendaContext context, Activation activation)
    {
        bool initial = false;
        if (!_changeKeys.TryGetValue(activation, out var keys))
        {
            initial = true;
            keys = new ChangeKeys(keySelectors.Count);
            _changeKeys[activation] = keys;
        }

        for (int i = 0; i < keySelectors.Count; i++)
        {
            keys.New[i] = keySelectors[i].Invoke(context, activation);
        }
        bool accept = true;

        if (!initial)
        {
            accept = false;
            for (int i = 0; i < keys.Current.Length; i++)
            {
                if (!Equals(keys.Current[i], keys.New[i]))
                {
                    accept = true;
                    break;
                }
            }
        }

        return accept;
    }

    public void Select(AgendaContext context, Activation activation)
    {
        if (_changeKeys.TryGetValue(activation, out var keys))
        {
            Array.Copy(keys.New, keys.Current, keys.Current.Length);
        }
    }

    public void Remove(AgendaContext context, Activation activation)
    {
        _changeKeys.Remove(activation);
    }

    private readonly struct ChangeKeys(int size)
    {
        public object[] Current { get; } = new object[size];
        public object[] New { get; } = new object[size];
    }
}