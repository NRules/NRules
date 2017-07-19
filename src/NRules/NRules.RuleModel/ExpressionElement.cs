using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that defines an expression.
    /// </summary>
    public abstract class ExpressionElement : RuleElement
    {
        private readonly List<Declaration> _references;

        internal ExpressionElement(IEnumerable<Declaration> declarations, IEnumerable<Declaration> references, LambdaExpression expression)
            : base(declarations)
        {
            _references = new List<Declaration>(references);
            Expression = expression;
        }

        /// <summary>
        /// Expression.
        /// </summary>
        public LambdaExpression Expression { get; }

        /// <summary>
        /// List of declarations the expression references.
        /// </summary>
        public IEnumerable<Declaration> References => _references;
    }
}