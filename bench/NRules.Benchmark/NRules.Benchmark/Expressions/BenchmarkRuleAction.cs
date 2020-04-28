using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
            _ruleAction = ExpressionCompiler.CompileAction(element, element.Imports.ToList(), new Declaration[0]);

            var compiledRule = new CompiledRule(null, element.Imports, new []{_ruleAction}, new IRuleDependency[0], null);
            var tuple = ToTuple("abcd", 4, 1.0m);
            var map = IndexMap.CreateMap(element.Imports, element.Imports);
            var activation = new Activation(compiledRule, tuple, map);
            _actionContext = new ActionContext(Context.Session, activation);
        }

        [Benchmark]
        public void EvaluateExpression()
        {
            var args = _ruleAction.GetArguments(Context, _actionContext);
            _ruleAction.Invoke(Context, _actionContext, args);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void PerformAction(IContext context, string value1, int value2, decimal value3)
        {
        }
    }
}
