using System;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a binding expression element.
    /// </summary>
    public class BindingBuilder : RuleElementBuilder, IBuilder<BindingElement>
    {
        private Type _resultType;
        private LambdaExpression _expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingBuilder"/>.
        /// </summary>
        public BindingBuilder()
        {
        }

        /// <summary>
        /// Sets type of the result produced by the binding expression.
        /// If not provided, this is set to the return type of the binding expression.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        public void ResultType(Type resultType)
        {
            _resultType = resultType;
        }

        /// <summary>
        /// Sets a calculated expression on the binding element.
        /// </summary>
        /// <param name="expression">Expression to bind.</param>
        public void BindingExpression(LambdaExpression expression)
        {
            _expression = expression;
        }

        BindingElement IBuilder<BindingElement>.Build()
        {
            var resultType = _resultType ?? _expression?.ReturnType;
            var element = Element.Binding(resultType, _expression);
            return element;
        }
    }
}