using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a group of rule actions.
    /// </summary>
    public class ActionGroupBuilder : RuleElementBuilder, IBuilder<ActionGroupElement>
    {
        private readonly List<ActionElement> _actions = new List<ActionElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionGroupBuilder"/>.
        /// </summary>
        public ActionGroupBuilder()
        {
        }

        /// <summary>
        /// Adds a rule action to the group element.
        /// The action will be executed on new and updated rule activations.
        /// </summary>
        /// <param name="expression">Rule action expression.
        /// The first parameter of the action expression must be <see cref="IContext"/>.
        /// Names and types of the rest of the expression parameters must match the names and types defined in the pattern declarations.</param>
        public void Action(LambdaExpression expression)
        {
            Action(expression, ActionElement.DefaultTrigger);
        }

        /// <summary>
        /// Adds a rule action to the group element.
        /// </summary>
        /// <param name="expression">Rule action expression.
        /// The first parameter of the action expression must be <see cref="IContext"/>.
        /// Names and types of the rest of the expression parameters must match the names and types defined in the pattern declarations.</param>
        /// <param name="actionTrigger">Activation events that trigger the action.</param>
        public void Action(LambdaExpression expression, ActionTrigger actionTrigger)
        {
            var actionElement = Element.Action(expression, actionTrigger);
            _actions.Add(actionElement);
        }

        ActionGroupElement IBuilder<ActionGroupElement>.Build()
        {
            var actionGroup = Element.ActionGroup(_actions);
            return actionGroup;
        }
    }
}