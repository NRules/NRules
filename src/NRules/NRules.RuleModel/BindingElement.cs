using System;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that represents results of an expression.
    /// </summary>
    public class BindingElement : PatternSourceElement
    {
        internal BindingElement(Type resultType, LambdaExpression expression) 
            : base(resultType)
        {
            Expression = expression;

            var imports = expression.Parameters.Select(p => p.ToDeclaration());
            AddImports(imports);
        }
        
        /// <summary>
        /// Binding expression.
        /// </summary>
        public LambdaExpression Expression { get; }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitBinding(context, this);
        }
    }
}
