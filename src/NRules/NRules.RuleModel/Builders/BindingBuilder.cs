using System;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a binding expression element.
    /// </summary>
    public class BindingBuilder : RuleElementBuilder, IBuilder<BindingElement>
    {
        private readonly Type _valueType;
        private LambdaExpression _expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingBuilder"/>.
        /// </summary>
        /// <param name="valueType">Type of the binding expression result.</param>
        public BindingBuilder(Type valueType)
        {
            _valueType = valueType;
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
            var element = Element.Binding(_valueType, _expression);
            return element;
        }
    }
}