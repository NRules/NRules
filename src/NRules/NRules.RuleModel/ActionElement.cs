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
        private readonly List<Declaration> _references;

        /// <summary>
        /// Expression that represents the rule action.
        /// </summary>
        public LambdaExpression Expression { get; }

        /// <summary>
        /// List of declarations referenced by the action expression.
        /// </summary>
        public IEnumerable<Declaration> References => _references;

        internal ActionElement(IEnumerable<Declaration> declarations, IEnumerable<Declaration> references, LambdaExpression expression)
            : base(declarations)
        {
            _references = new List<Declaration>(references);
            Expression = expression;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitAction(context, this);
        }
    }
}