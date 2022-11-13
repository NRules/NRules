using NRules.Utilities;

namespace NRules.AgendaFilters;

internal class KeyChangeAgendaFilter : IStatefulAgendaFilter
{
    private readonly IReadOnlyCollection<IActivationExpression<object>> _keySelectors;
    private readonly Dictionary<Activation, ChangeKeys> _changeKeys = new();

    public KeyChangeAgendaFilter(IReadOnlyCollection<IActivationExpression<object>> keySelectors)
    {
        _keySelectors = keySelectors;
    }

    public bool Accept(AgendaContext context, Activation activation)
    {
        var initial = false;
        var keys = _changeKeys.GetOrAdd(activation, _ =>
        {
            initial = true;
            return new ChangeKeys(_keySelectors.Count);
        });

        SelectNewKeys(keys, context, activation);

        if (initial)
        {
            return true;
        }

        for (var i = 0; i < keys.Current.Length; i++)
        {
            if (!Equals(keys.Current[i], keys.New[i]))
            {
                return true;
            }
        }

        return false;

        void SelectNewKeys(ChangeKeys keys, AgendaContext context, Activation activation)
        {
            var i = 0;
            foreach (var selector in _keySelectors)
            {
                keys.New[i++] = selector.Invoke(context, activation);
            }
        }
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