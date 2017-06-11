using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a rule pattern.
    /// </summary>
    public class PatternBuilder : RuleLeftElementBuilder, IBuilder<PatternElement>
    {
        private readonly List<ConditionElement> _conditions = new List<ConditionElement>();
        private PatternSourceElementBuilder _sourceBuilder;

        internal PatternBuilder(SymbolTable scope, Declaration declaration) : base(scope)
        {
            Declaration = declaration;
        }

        /// <summary>
        /// Builder for the source of this element.
        /// </summary>
        public PatternSourceElementBuilder SourceBuilder => _sourceBuilder;

        /// <summary>
        /// Adds a condition expression to the pattern.
        /// </summary>
        /// <param name="expression">Condition expression.
        /// Names and types of the expression parameters must match the names and types defined in the pattern declarations.</param>
        public void Condition(LambdaExpression expression)
        {
            IEnumerable<Declaration> references = expression.Parameters.Select(p => Scope.Lookup(p.Name, p.Type));
            var condition = new ConditionElement(Scope.Declarations, references, expression);
            _conditions.Add(condition);
        }

        /// <summary>
        /// Pattern declaration.
        /// </summary>
        public Declaration Declaration { get; }

        /// <summary>
        /// Creates an aggregate builder that builds the source of the pattern.
        /// </summary>
        /// <returns>Aggregate builder.</returns>
        public AggregateBuilder Aggregate()
        {
            AssertSingleSource();
            var builder = new AggregateBuilder(Declaration.Type, Scope);
            _sourceBuilder = builder;
            return builder;
        }

        PatternElement IBuilder<PatternElement>.Build()
        {
            Validate();
            PatternElement patternElement;
            if (_sourceBuilder != null)
            {
                var builder = (IBuilder<PatternSourceElement>)_sourceBuilder;
                var source = builder.Build();
                patternElement = new PatternElement(Declaration, Scope.VisibleDeclarations, _conditions, source);
            }
            else
            {
                patternElement = new PatternElement(Declaration, Scope.VisibleDeclarations, _conditions);
            }
            Declaration.Target = patternElement;
            return patternElement;
        }

        private void AssertSingleSource()
        {
            if (_sourceBuilder != null)
            {
                throw new InvalidOperationException("Pattern element can only have a single source");
            }
        }

        private void Validate()
        {
            foreach (var condition in _conditions)
            {
                var dependencyDeclarations = condition.References.Where(x => x.Target is DependencyElement).ToArray();
                if (dependencyDeclarations.Any())
                {
                    var names = string.Join(",", dependencyDeclarations.Select(x => x.Name));
                    var message = $"Pattern element cannot reference injected dependency. Condition={condition.Expression}, Dependency={names}";
                    throw new InvalidOperationException(message);
                }

                if (condition.Expression.ReturnType != typeof(bool))
                {
                    var message = $"Pattern condition must return a Boolean result. Condition={condition.Expression}";
                    throw new ArgumentException(message);
                }
            }
        }
    }
}