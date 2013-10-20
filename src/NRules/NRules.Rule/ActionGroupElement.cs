using System.Collections.Generic;

namespace NRules.Rule
{
    public class ActionGroupElement
    {
        private readonly List<ActionElement> _actions;

        internal ActionGroupElement(IEnumerable<ActionElement> actions)
        {
            _actions = new List<ActionElement>(actions);
        }

        public IEnumerable<ActionElement> Actions { get { return _actions; } }
    }
}