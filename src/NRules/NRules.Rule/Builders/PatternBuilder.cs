using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Rule.Builders
{
    public class PatternBuilder : RuleElementBuilder, IBuilder<PatternElement>
    {
        private readonly List<ConditionElement> _conditions = new List<ConditionElement>();

        internal PatternBuilder(SymbolTable scope, Declaration declaration) : base(scope)
        {
            Declaration = declaration;
        }

        public void Condition(LambdaExpression expression)
        {
            IEnumerable<Declaration> declarations = expression.Parameters.Select(p => Scope.Lookup(p.Name, p.Type));
            var condition = new ConditionElement(declarations, expression);
            _conditions.Add(condition);
        }

        public Declaration Declaration { get; private set; }

        PatternElement IBuilder<PatternElement>.Build()
        {
            var patternElement = new PatternElement(Declaration, _conditions);
            Declaration.Target = patternElement;
            return patternElement;
        }
    }
}