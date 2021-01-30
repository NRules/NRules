using System.Collections.Generic;
using System.Linq;

namespace NRules.AgendaFilters
{
    internal class KeyChangeAgendaFilter : IStatefulAgendaFilter
    {
        private readonly List<IActivationExpression<object>> _keySelectors;
        private readonly Dictionary<Activation, ChangeKeys> _changeKeys = new Dictionary<Activation, ChangeKeys>();

        public KeyChangeAgendaFilter(IEnumerable<IActivationExpression<object>> keySelectors)
        {
            _keySelectors = new List<IActivationExpression<object>>(keySelectors);
        }

        public bool Accept(AgendaContext context, Activation activation)
        {
            if (!_changeKeys.TryGetValue(activation, out var keys))
            {
                keys = new ChangeKeys();
                _changeKeys[activation] = keys;
            }

            keys.New = _keySelectors.Select(selector => selector.Invoke(context, activation)).ToList();
            bool accept = true;

            if (keys.Current != null)
            {
                accept = false;
                for (int i = 0; i < keys.Current.Count; i++)
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

        public void OnFiring(AgendaContext context, Activation activation)
        {
            if (_changeKeys.TryGetValue(activation, out var keys))
            {
                keys.Current = keys.New;
            }
        }

        private class ChangeKeys
        {
            public List<object> Current { get; set; }
            public List<object> New { get; set; }
        }
    }
}