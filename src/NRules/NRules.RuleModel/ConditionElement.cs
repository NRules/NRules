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
        private readonly List<Declaration> _references;

        /// <summary>
        /// Expression that represents a boolean condition.
        /// </summary>
        public LambdaExpression Expression { get; }

        /// <summary>
        /// List of declarations the condition expression references.
        /// </summary>
        public IEnumerable<Declaration> References => _references;

        internal ConditionElement(IEnumerable<Declaration> declarations, IEnumerable<Declaration> references, LambdaExpression expression)
            : base(declarations)
        {
            _references = new List<Declaration>(references);
            Expression = expression;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitCondition(context, this);
        }
    }
}