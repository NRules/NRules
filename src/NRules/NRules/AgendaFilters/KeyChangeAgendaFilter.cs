using System.Collections.Generic;
using System.Linq;

namespace NRules.AgendaFilters
{
    internal class KeyChangeAgendaFilter : IAgendaFilter
    {
        private const string KeyName = "ChangeKeys";
        private readonly List<IActivationExpression> _keySelectors;

        public KeyChangeAgendaFilter(IEnumerable<IActivationExpression> keySelectors)
        {
            _keySelectors = new List<IActivationExpression>(keySelectors);
        }

        public bool Accept(Activation activation)
        {
            var oldKeys = activation.GetState<List<object>>(KeyName);
            var newKeys = _keySelectors.Select(selector => selector.Invoke(activation)).ToList();
            bool accept = true;

            if (oldKeys != null)
            {
                accept = false;
                for (int i = 0; i < oldKeys.Count; i++)
                {
                    if (!Equals(oldKeys[i], newKeys[i]))
                    {
                        accept = true;
                        break;
                    }
                }
            }

            activation.SetState(KeyName, newKeys);
            return accept;
        }
    }
}