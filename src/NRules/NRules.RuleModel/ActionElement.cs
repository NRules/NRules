using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Activation events that trigger the actions.
    /// </summary>
    [Flags]
    public enum ActionTrigger
    {
        /// <summary>
        /// Action is not triggered.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Action is triggered when activation is created.
        /// </summary>
        Activated = 0x1,

        /// <summary>
        /// Action is triggered when activation is updated.
        /// </summary>
        Reactivated = 0x2,

        /// <summary>
        /// Action is triggered when activation is removed.
        /// </summary>
        Deactivated = 0x4,
    }

    /// <summary>
    /// Action executed by the engine when the rule fires.
    /// </summary>
    [DebuggerDisplay("{Expression.ToString()}")]
    public class ActionElement : ExpressionElement
    {
        /// <summary>
        /// Default value for action trigger.
        /// </summary>
        public const ActionTrigger DefaultTrigger = ActionTrigger.Activated | ActionTrigger.Reactivated;
        
        internal ActionElement(LambdaExpression expression, ActionTrigger actionTrigger)
            : base(expression, expression.Parameters.Skip(1))
        {
            ActionTrigger = actionTrigger;
        }

        /// <summary>
        /// Activation events that trigger this action.
        /// </summary>
        public ActionTrigger ActionTrigger { get; }
        
        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitAction(context, this);
        }
    }
}