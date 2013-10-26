using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Rule.Builders
{
    /// <summary>
    /// Builder to compose a rule pattern.
    /// </summary>
    public class PatternBuilder : RuleElementBuilder, IBuilder<PatternElement>
    {
        private readonly List<ConditionElement> _conditions = new List<ConditionElement>();
        private IBuilder<PatternSourceElement> _sourceBuilder;

        internal PatternBuilder(SymbolTable scope, Declaration declaration) : base(scope)
        {
            Declaration = declaration;
        }

        /// <summary>
        /// Adds a condition expression to the pattern.
        /// </summary>
        /// <param name="expression">Condition expression.</param>
        public void Condition(LambdaExpression expression)
        {
            IEnumerable<Declaration> declarations = expression.Parameters.Select(p => Scope.Lookup(p.Name, p.Type));
            var condition = new ConditionElement(declarations, expression);
            _conditions.Add(condition);
        }

        /// <summary>
        /// Pattern declaration.
        /// </summary>
        public Declaration Declaration { get; private set; }

        /// <summary>
        /// Creates an aggregate builder that builds the source of the pattern.
        /// </summary>
        /// <returns>Aggregate builder.</returns>
        public AggregateBuilder SourceAggregate()
        {
            if (_sourceBuilder != null)
            {
                throw new InvalidOperationException("Pattern can only have a single source");
            }

            SymbolTable scope = Scope.New();

            var builder = new AggregateBuilder(Declaration.Type, scope);
            _sourceBuilder = builder;

            return builder;
        }

        PatternElement IBuilder<PatternElement>.Build()
        {
            PatternElement patternElement;
            if (_sourceBuilder != null)
            {
                var source = _sourceBuilder.Build();
                patternElement = new PatternElement(Declaration, _conditions, source);
            }
            else
            {
                patternElement = new PatternElement(Declaration, _conditions);
            }
            Declaration.Target = patternElement;
            return patternElement;
        }
    }
}