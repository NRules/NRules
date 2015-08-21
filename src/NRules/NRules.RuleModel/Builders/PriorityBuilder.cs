using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose priority expression element.
    /// </summary>
    public class PriorityBuilder : RuleElementBuilder, IBuilder<PriorityElement>
    {
        private LambdaExpression _expression;
        private IEnumerable<Declaration> _references;

        internal PriorityBuilder(SymbolTable scope) : base(scope)
        {
        }

        /// <summary>
        /// Sets constant priority.
        /// Default priority is 0.
        /// </summary>
        /// <param name="priority">Priority value.</param>
        public void PriorityValue(int priority)
        {
            var expression = Expression.Lambda<Func<int>>(Expression.Constant(priority));
            PriorityExpression(expression);
        }

        /// <summary>
        /// Sets priority expression.
        /// </summary>
        /// <param name="expression">Priority expression.
        /// Names and types of the expression parameters must match the names and types defined in the pattern declarations.</param>
        public void PriorityExpression(LambdaExpression expression)
        {
            if (expression.ReturnType != typeof(int))
            {
                throw new ArgumentException("Priority expression must return an integer");
            }
            _references = expression.Parameters.Select(p => Scope.Lookup(p.Name, p.Type));
            _expression = expression;
        }

        public PriorityElement Build()
        {
            if (_expression == null) PriorityValue(RuleDefinition.DefaultPriority);

            var priorityElement = new PriorityElement(Scope.VisibleDeclarations, _references, _expression);
            return priorityElement;
        }
    }
}