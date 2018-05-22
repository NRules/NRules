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

        public bool Accept(AgendaContext context, Activation activation)
        {
            var keys = activation.GetState<ChangeKeys>(KeyName);
            if (keys == null)
            {
                keys = new ChangeKeys();
                activation.SetState(KeyName, keys);
                activation.OnRuleFiring += OnRuleFiring;
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

        private void OnRuleFiring(object sender, ActivationEventArgs args)
        {
            var keys = args.Activation.GetState<ChangeKeys>(KeyName);
            if (keys != null)
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