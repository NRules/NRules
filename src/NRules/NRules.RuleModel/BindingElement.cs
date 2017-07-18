using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that represents a binding of a calculated expression to a declaration.
    /// </summary>
    public class BindingElement : RuleLeftElement
    {
        private readonly List<Declaration> _references;

        internal BindingElement(Declaration declaration, IEnumerable<Declaration> declarations, IEnumerable<Declaration> references, LambdaExpression expression) 
            : base(declarations)
        {
            Declaration = declaration;
            ValueType = declaration.Type;
            Expression = expression;
            _references = new List<Declaration>(references);
        }

        /// <summary>
        /// Binding declaration.
        /// </summary>
        public Declaration Declaration { get; }

        /// <summary>
        /// Type of the values that the binding represents.
        /// </summary>
        public Type ValueType { get; }

        /// <summary>
        /// Binding expression.
        /// </summary>
        public LambdaExpression Expression { get; }

        /// <summary>
        /// List of declarations the binding expression references.
        /// </summary>
        public IEnumerable<Declaration> References => _references;

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitBinding(context, this);
        }
    }
}
