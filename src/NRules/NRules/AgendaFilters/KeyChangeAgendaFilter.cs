using NRules.Utilities;

namespace NRules.AgendaFilters;

internal class KeyChangeAgendaFilter : IStatefulAgendaFilter
{
    private readonly List<IActivationExpression<object>> _keySelectors;
    private readonly Dictionary<Activation, ChangeKeys> _changeKeys = new();

    public KeyChangeAgendaFilter(IEnumerable<IActivationExpression<object>> keySelectors)
    {
        _keySelectors = new List<IActivationExpression<object>>(keySelectors);
    }

    public bool Accept(AgendaContext context, Activation activation)
    {
        var initial = false;
        var keys = _changeKeys.GetOrAdd(activation, _ =>
        {
            initial = true;
            return new ChangeKeys(_keySelectors.Count);
        });

        for (var i = 0; i < _keySelectors.Count; i++)
        {
            keys.New[i] = _keySelectors[i].Invoke(context, activation);
        }

        var accept = true;
        if (!initial)
        {
            accept = false;
            for (var i = 0; i < keys.Current.Length; i++)
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
        _changeKeys.GetValueOrDefault(activation).UpdateCurrent();
    }

    public void Remove(AgendaContext context, Activation activation)
    {
        _changeKeys.Remove(activation);
    }

    private readonly struct ChangeKeys
    {
        public ChangeKeys(int size)
        {
            Current = new object[size];
            New = new object[size];
        }

        public object[] Current { get; } = Array.Empty<object>();
        public object[] New { get; } = Array.Empty<object>();

        public void UpdateCurrent()
        {
            Array.Copy(New, Current, Current.Length);
        }
    }
}