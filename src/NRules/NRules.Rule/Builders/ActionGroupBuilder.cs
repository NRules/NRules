using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Rule.Builders
{
    public class ActionGroupBuilder : RuleElementBuilder, IBuilder<ActionGroupElement>
    {
        private readonly List<ActionElement> _actions = new List<ActionElement>();

        internal ActionGroupBuilder(SymbolTable scope) : base(scope)
        {
        }

        public void Action(LambdaExpression action)
        {
            var actionElement = new ActionElement(action);
            _actions.Add(actionElement);
        }

        public ActionGroupElement Build()
        {
            var actionGroup = new ActionGroupElement(_actions);
            return actionGroup;
        }
    }
}