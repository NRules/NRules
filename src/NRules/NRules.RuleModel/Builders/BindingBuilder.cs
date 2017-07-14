using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a binding element that associates a declaration with a calculated expression.
    /// </summary>
    public class BindingBuilder : RuleLeftElementBuilder, IBuilder<BindingElement>
    {
        private readonly Declaration _declaration;
        private LambdaExpression _expression;

        internal BindingBuilder(SymbolTable scope, Declaration declaration) : base(scope)
        {
            _declaration = declaration;
        }

        /// <summary>
        /// Adds a calculated expression to the binding element.
        /// </summary>
        /// <param name="expression">Expression to bind.</param>
        public void BindingExpression(LambdaExpression expression)
        {
            _expression = expression;
        }

        BindingElement IBuilder<BindingElement>.Build()
        {
            if (_expression == null)
                throw new ArgumentException($"BINDING element requires a binding expression. Name={_declaration.Name}, Type={_declaration.Type}");

            IEnumerable<Declaration> references = _expression.Parameters.Select(p => Scope.Lookup(p.Name, p.Type));
            var element = new BindingElement(_declaration, Scope.VisibleDeclarations, references, _expression);
            _declaration.Target = element;
            return element;
        }
    }
}