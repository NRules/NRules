using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Rule.Builders
{
    public class ActionGroupBuilder : RuleElementBuilder, IBuilder<ActionGroupElement>
    {
        private readonly List<ActionElement> _actions = new List<ActionElement>();

        internal ActionGroupBuilder(SymbolTable scope) : base(scope)
        {
        }

        public void Action(LambdaExpression expression)
        {
            IEnumerable<Declaration> declarations = expression.Parameters.Skip(1).Select(p => Scope.Lookup(p.Name, p.Type));
            var actionElement = new ActionElement(declarations, expression);
            _actions.Add(actionElement);
        }

        public ActionGroupElement Build()
        {
            var actionGroup = new ActionGroupElement(_actions);
            return actionGroup;
        }
    }
}