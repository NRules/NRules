using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Pattern condition element.
    /// </summary>
    [DebuggerDisplay("{Expression.ToString()}")]
    public class ConditionElement : RuleElement
    {
        private readonly List<Declaration> _declarations;
        private readonly LambdaExpression _expression;

        /// <summary>
        /// Expression that represents a boolean condition.
        /// </summary>
        public LambdaExpression Expression
        {
            get { return _expression; }
        }

        /// <summary>
        /// List of declarations the condition expression references.
        /// </summary>
        public override IEnumerable<Declaration> Declarations
        {
            get { return _declarations; }
        }

        internal ConditionElement(IEnumerable<Declaration> declarations, LambdaExpression expression)
        {
            _declarations = new List<Declaration>(declarations);
            _expression = expression;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitCondition(context, this);
        }
    }
}