using System;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.Utilities;

namespace NRules
{
    internal interface IActivationCondition
    {
        bool Invoke(Activation activation);
    }

    internal class ActivationCondition : IActivationCondition
    {
        private readonly LambdaExpression _expression;
        private readonly FastDelegate<Func<object[], bool>> _compiledExpression;
        private readonly IndexMap _factIndexMap;

        public ActivationCondition(LambdaExpression expression, FastDelegate<Func<object[], bool>> compiledExpression, IndexMap factIndexMap)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
            _factIndexMap = factIndexMap;
        }

        public bool Invoke(Activation activation)
        {
            var tuple = activation.Tuple;
            var tupleFactMap = activation.TupleFactMap;

            var args = new object[_compiledExpression.ArrayArgumentCount];

            int index = tuple.Count - 1;
            var factIndexMap = _factIndexMap;
            foreach (var fact in tuple.Facts)
            {
                var mappedIndex = factIndexMap[tupleFactMap[index]];
                IndexMap.SetElementAt(args, mappedIndex, fact.Object);
                index--;
            }

            return _compiledExpression.Delegate.Invoke(args);
        }
    }
}
