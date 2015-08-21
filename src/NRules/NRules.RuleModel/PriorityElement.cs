using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule priority expression.
    /// </summary>
    [DebuggerDisplay("{Expression.ToString()}")]
    public class PriorityElement : RuleElement
    {
        private readonly List<Declaration> _references;
        private readonly LambdaExpression _expression;

        /// <summary>
        /// Expression that calculates rule's priority.
        /// </summary>
        public LambdaExpression Expression
        {
            get { return _expression; }
        }

        /// <summary>
        /// List of declarations the priority expression references.
        /// </summary>
        public IEnumerable<Declaration> References
        {
            get { return _references; }
        }

        public PriorityElement(IEnumerable<Declaration> declarations, IEnumerable<Declaration> references, LambdaExpression expression) 
            : base(declarations)
        {
            _references = new List<Declaration>(references);
            _expression = expression;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitPriority(context, this);
        }
    }
}