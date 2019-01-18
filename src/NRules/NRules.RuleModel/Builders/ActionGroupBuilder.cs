using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a group of rule actions.
    /// </summary>
    public class ActionGroupBuilder : RuleRightElementBuilder, IBuilder<ActionGroupElement>
    {
        private readonly List<ActionElement> _actions = new List<ActionElement>();

        private const ActionTrigger DefaultTrigger = ActionTrigger.Activated | ActionTrigger.Reactivated;

        internal ActionGroupBuilder()
        {
        }

        /// <summary>
        /// Adds a rule action to the group.
        /// The action will be executed on new and updated rule activations.
        /// </summary>
        /// <param name="expression">Rule action expression.
        /// The first parameter of the action expression must be <see cref="IContext"/>.
        /// Names and types of the rest of the expression parameters must match the names and types defined in the pattern declarations.</param>
        public void Action(LambdaExpression expression)
        {
            Action(expression, DefaultTrigger);
        }

        /// <summary>
        /// Adds a rule action to the group.
        /// </summary>
        /// <param name="expression">Rule action expression.
        /// The first parameter of the action expression must be <see cref="IContext"/>.
        /// Names and types of the rest of the expression parameters must match the names and types defined in the pattern declarations.</param>
        /// <param name="actionTrigger">Activation events that trigger the action.</param>
        public void Action(LambdaExpression expression, ActionTrigger actionTrigger)
        {
            if (expression.Parameters.Count == 0 ||
                expression.Parameters.First().Type != typeof(IContext))
            {
                throw new ArgumentException(
                    $"Action expression must have {typeof(IContext)} as its first parameter");
            }

            if (actionTrigger == ActionTrigger.None)
            {
                throw new ArgumentException("Action trigger not specified");
            }

            var actionElement = new ActionElement(expression, actionTrigger);
            _actions.Add(actionElement);
        }

        ActionGroupElement IBuilder<ActionGroupElement>.Build()
        {
            var insertActions = _actions.Where(x => x.ActionTrigger.HasFlag(ActionTrigger.Activated)).ToList();
            if (insertActions.Count == 0)
            {
                throw new ArgumentException($"Rule must have at least one match action");
            }

            var actionGroup = new ActionGroupElement(_actions);
            return actionGroup;
        }
    }
}