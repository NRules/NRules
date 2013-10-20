using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Rule.Builders
{
    public class PatternBuilder : RuleElementBuilder, IRuleElementBuilder<PatternElement>
    {
        private readonly List<ConditionElement> _conditions = new List<ConditionElement>();

        internal PatternBuilder(SymbolTable scope) : base(scope)
        {
            StartSymbolScope();
        }

        public void Condition(LambdaExpression expression)
        {
            var declarations = expression.Parameters.Select(p => Scope.Lookup(p.Name, p.Type));
            var condition = new ConditionElement(declarations, expression);
            _conditions.Add(condition);
        }

        public Declaration Declaration { get; internal set; }

        PatternElement IRuleElementBuilder<PatternElement>.Build()
        {
            var patternElement = new PatternElement(Declaration, _conditions);
            Declaration.Target = patternElement;
            return patternElement;
        }
    }
}