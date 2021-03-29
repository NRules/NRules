using System;
using System.Linq;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using NRules.Diagnostics;
using NRules.Rete;
using NRules.RuleModel.Builders;
using NRules.Utilities;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Benchmark.Expressions
{
    [BenchmarkCategory("Micro", "Expressions")]
    public class BenchmarkLhsExpression : BenchmarkBase
    {
        private readonly NodeInfo _nodeInfo;
        private readonly ILhsExpression<bool> _lhsExpression;
        private readonly ILhsTupleExpression<bool> _lhsTupleExpression;
        private readonly ILhsFactExpression<bool> _lhsFactExpression;
        private readonly Tuple _tuple;
        private readonly Fact _fact;

        public BenchmarkLhsExpression()
        {
            _nodeInfo = new NodeInfo();
            var ruleExpressionCompiler = new RuleExpressionCompiler();
            Expression<Func<string, int, decimal, bool>> betaExpression = (s, i, d) => s.Length == i;
            var betaElement = Element.Condition(betaExpression);
            _lhsExpression = ruleExpressionCompiler.CompileLhsTupleFactExpression<bool>(betaElement, betaElement.Imports.ToList());
            _lhsTupleExpression = ruleExpressionCompiler.CompileLhsTupleExpression<bool>(betaElement, betaElement.Imports.ToList());
            _tuple = ToTuple("abcd", 4, 1.0m);

            Expression<Func<string, bool>> alphaExpression = s => s.Length == 1;
            var alphaElement = Element.Condition(alphaExpression);
            _lhsFactExpression = ruleExpressionCompiler.CompileLhsFactExpression<bool>(alphaElement);
            _fact = new Fact("abcd");
        }

        [Benchmark]
        public bool EvaluateTupleFactExpression() => _lhsExpression.Invoke(Context, _nodeInfo, _tuple.LeftTuple, _tuple.RightFact);
  
        [Benchmark]
        public bool EvaluateTupleExpression() => _lhsTupleExpression.Invoke(Context, _nodeInfo, _tuple); 
        
        [Benchmark]
        public bool EvaluateFactExpression() => _lhsFactExpression.Invoke(Context, _nodeInfo, _fact);
    }
}
