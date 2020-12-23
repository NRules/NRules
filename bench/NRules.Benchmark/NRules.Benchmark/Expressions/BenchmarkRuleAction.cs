using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using BenchmarkDotNet.Attributes;
using NRules.Rete;
using NRules.RuleModel;
using NRules.RuleModel.Builders;
using NRules.Utilities;

namespace NRules.Benchmark.Expressions
{
    [BenchmarkCategory("Micro", "Expressions")]
    public class BenchmarkRuleAction : BenchmarkBase
    {
        private readonly IActionContext _actionContext;
        private readonly IRuleAction _ruleAction;

        public BenchmarkRuleAction()
        {
            Expression<Action<IContext, string, int, decimal>> expression = (c, s, i, d) => PerformAction(c, s, i, d);
            var element = Element.Action(expression);
            var map = IndexMap.CreateMap(element.Imports, element.Imports);
            _ruleAction = ExpressionCompiler.CompileAction(element, element.Imports.ToList(), new List<DependencyElement>(), map);

            var compiledRule = new CompiledRule(null, element.Imports, new []{_ruleAction}, null, map);
            var tuple = ToTuple("abcd", 4, 1.0m);
            var activation = new Activation(compiledRule, tuple);
            _actionContext = new ActionContext(Context.Session, activation, CancellationToken.None);
        }

        [Benchmark]
        public void EvaluateExpression()
        {
            _ruleAction.Invoke(Context, _actionContext);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void PerformAction(IContext context, string value1, int value2, decimal value3)
        {
        }
    }
}
