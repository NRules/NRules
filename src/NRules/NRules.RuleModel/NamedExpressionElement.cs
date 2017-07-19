using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Expression with a name used by an aggregator.
    /// </summary>
    public class NamedExpressionElement : ExpressionElement
    {
        internal NamedExpressionElement(string name, IEnumerable<Declaration> declarations, IEnumerable<Declaration> references, LambdaExpression expression) 
            : base(declarations, references, expression)
        {
            Name = name;
        }

        /// <summary>
        /// Expression name.
        /// </summary>
        public string Name { get; }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitNamedExpression(context, this);
        }
    }
}