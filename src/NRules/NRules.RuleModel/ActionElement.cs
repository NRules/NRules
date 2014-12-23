using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Action executed by the engine when the rule fires.
    /// </summary>
    [DebuggerDisplay("{Expression.ToString()}")]
    public class ActionElement : RuleElement
    {
        private readonly List<Declaration> _declarations;
        private readonly LambdaExpression _expression;

        /// <summary>
        /// Expression that represents the rule action.
        /// </summary>
        public LambdaExpression Expression
        {
            get { return _expression; }
        }

        /// <summary>
        /// List of declarations referenced by the action expression.
        /// </summary>
        public IEnumerable<Declaration> Declarations
        {
            get { return _declarations; }
        }

        internal ActionElement(IEnumerable<Declaration> declarations, LambdaExpression expression)
        {
            _declarations = new List<Declaration>(declarations);
            _expression = expression;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitAction(context, this);
        }
    }
}