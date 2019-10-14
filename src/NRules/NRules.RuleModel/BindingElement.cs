using System;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that represents results of an expression.
    /// </summary>
    public class BindingElement : ExpressionElement
    {
        internal BindingElement(Type resultType, LambdaExpression expression) 
            : base(expression)
        {
            ResultType = resultType;
        }

        /// <inheritdoc cref="RuleElement.ElementType"/>
        public override ElementType ElementType => ElementType.Binding;
        
        /// <summary>
        /// Type of the result that this rule element yields.
        /// </summary>
        public Type ResultType { get; }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitBinding(context, this);
        }
    }
}
