using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions
{
    internal class ActionRewriter : ExpressionRewriter
    {
        public ActionRewriter(IEnumerable<Declaration> declarations)
            : base(declarations)
        {
        }

        protected override void InitParameters(LambdaExpression expression)
        {
            Parameters.Add(expression.Parameters.First());
        }
    }
}