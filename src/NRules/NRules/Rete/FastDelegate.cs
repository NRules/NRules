using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Rete
{
    internal static class FastDelegate
    {
        public static TDelegate Create<TDelegate>(LambdaExpression expression)
        {
            var wrapper = Wrap<TDelegate>(expression);
            var fastDelegate = wrapper.Compile();
            return fastDelegate;
        }

        private static Expression<TDelegate> Wrap<TDelegate>(LambdaExpression expression)
        {
            var parameter = Expression.Parameter(typeof(object[]));
            var arguments = new List<Expression>();
            for (int i = 0; i < expression.Parameters.Count; i++)
            {
                var parameterValue = Expression.ArrayIndex(parameter, Expression.Constant(i));
                var argument = Expression.Convert(parameterValue, expression.Parameters[i].Type);
                arguments.Add(argument);
            }
            var body = Expression.Invoke(expression, arguments);
            var wrapper = Expression.Lambda<TDelegate>(body, parameter);
            
            return wrapper;
        }
    }
}
