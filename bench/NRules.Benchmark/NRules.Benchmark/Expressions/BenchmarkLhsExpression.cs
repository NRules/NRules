using System;
using System.Linq;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using NRules.Rete;
using NRules.RuleModel.Builders;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Benchmark.Expressions
{
    public class BenchmarkLhsExpression : BenchmarkBase
    {
        private readonly NodeDebugInfo _nodeInfo;
        private readonly ILhsExpression<bool> _lhsExpression;
        private readonly Tuple _tuple;

        public BenchmarkLhsExpression()
        {
            _nodeInfo = new NodeDebugInfo();
            Expression<Func<string, int, decimal, bool>> expression = (s, i, d) => s.Length == i;
            var element = Element.Condition(expression);
            _lhsExpression = ExpressionCompiler.CompileLhsExpression<bool>(element, element.Imports.ToList());

            _tuple = ToTuple("abcd", 4, 1.0m);
        }

        [Benchmark]
        public bool EvaluateExpression() => _lhsExpression.Invoke(Context, _nodeInfo, _tuple.LeftTuple, _tuple.RightFact);
    }
}
