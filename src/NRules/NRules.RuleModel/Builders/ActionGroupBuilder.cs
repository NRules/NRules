using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a group of rule actions.
    /// </summary>
    public class ActionGroupBuilder : RuleElementBuilder, IBuilder<ActionGroupElement>
    {
        private readonly List<ActionElement> _actions = new List<ActionElement>();

        internal ActionGroupBuilder(SymbolTable scope) : base(scope)
        {
        }

        /// <summary>
        /// Adds a rule action to the group.
        /// </summary>
        /// <param name="expression">Rule action expression.
        /// The first parameter of the action expression must be <see cref="IContext"/>.
        /// Names and types of the rest of the expression parameters must match the names and types defined in the pattern declarations.</param>
        public void Action(LambdaExpression expression)
        {
            if (expression.Parameters.Count == 0 ||
                expression.Parameters.First().Type != typeof(IContext))
            {
                throw new ArgumentException(
                    string.Format("Action expression must have {0} as its first parameter", typeof(IContext)));
            }
            IEnumerable<ParameterExpression> parameters = expression.Parameters.Skip(1);
            IEnumerable<Declaration> references = parameters.Select(p => Scope.Lookup(p.Name, p.Type));
            var actionElement = new ActionElement(Scope.Declarations, references, expression);
            _actions.Add(actionElement);
        }

        ActionGroupElement IBuilder<ActionGroupElement>.Build()
        {
            var actionGroup = new ActionGroupElement(Scope.VisibleDeclarations, _actions);
            return actionGroup;
        }
    }
}