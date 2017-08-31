using System.Collections.Generic;
using System.Linq;

namespace NRules
{
    internal class KeyChangeActivationFilter : IActivationFilter
    {
        private readonly List<IActivationExpression> _keySelectors;
        private readonly Dictionary<Activation, List<object>> _keys = new Dictionary<Activation, List<object>>();

        public KeyChangeActivationFilter(IEnumerable<IActivationExpression> keySelectors)
        {
            _keySelectors = new List<IActivationExpression>(keySelectors);
        }

        public bool Accept(Activation activation)
        {
            List<object> oldKeys;
            _keys.TryGetValue(activation, out oldKeys);

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

            _keys[activation] = newKeys;
            return accept;
        }

        public void Remove(Activation activation)
        {
            _keys.Remove(activation);
        }
    }
}