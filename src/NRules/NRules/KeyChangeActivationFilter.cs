using System.Collections.Generic;
using System.Linq;
using NRules.Extensibility;

namespace NRules
{
    internal class KeyChangeActivationFilter : IActivationFilter
    {
        private readonly List<IActivationExpression> _keySelectors;
        private List<object> _keys = new List<object>();

        public KeyChangeActivationFilter(IEnumerable<IActivationExpression> keySelectors)
        {
            _keySelectors = new List<IActivationExpression>(keySelectors);
        }

        public bool Accept(Activation activation)
        {
            var newKeys = _keySelectors.Select(selector => selector.Invoke(activation)).ToList();
            bool accept = true;

            if (_keys.Count == newKeys.Count)
            {
                accept = false;
                for (int i = 0; i < _keys.Count; i++)
                {
                    if (!Equals(_keys[i], newKeys[i]))
                    {
                        accept = true;
                        break;
                    }
                }
            }

            _keys = newKeys;
            return accept;
        }
    }
}