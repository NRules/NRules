using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;

namespace NRules.Fluent
{
    internal abstract class ExpressionBuilderDecorator : ILeftHandSide
    {
        protected ExpressionBuilder Builder { get; private set; }

        protected ExpressionBuilderDecorator(ExpressionBuilder builder)
        {
            Builder = builder;
        }

        public ILeftHandSide Match<T>(Expression<Func<T>> alias, params Expression<Func<T, bool>>[] conditions)
        {
            return Builder.Match(alias, conditions);
        }

        public ILeftHandSide Match<T>(Expression<Func<T, bool>> condition, params Expression<Func<T, bool>>[] conditions)
        {
            return Builder.Match(condition, conditions);
        }

        public ILeftHandSide Match<T>()
        {
            return Builder.Match<T>();
        }

        public ICollectPattern<IEnumerable<T>> Collect<T>(Expression<Func<IEnumerable<T>>> alias, params Expression<Func<T, bool>>[] itemConditions)
        {
            return Builder.Collect(alias, itemConditions);
        }

        public ILeftHandSide Exists<T>(params Expression<Func<T, bool>>[] conditions)
        {
            return Builder.Exists(conditions);
        }

        public ILeftHandSide Not<T>(params Expression<Func<T, bool>>[] conditions)
        {
            return Builder.Not(conditions);
        }

        public ILeftHandSide All<T>(Expression<Func<T, bool>> baseCondition, params Expression<Func<T, bool>>[] conditions)
        {
            return Builder.All(baseCondition, conditions);
        }

        public ILeftHandSide All<T>(Expression<Func<T, bool>> condition)
        {
            return Builder.All(condition);
        }

        public ILeftHandSide And(Action<ILeftHandSide> builder)
        {
            return Builder.And(builder);
        }

        public ILeftHandSide Or(Action<ILeftHandSide> builder)
        {
            return Builder.Or(builder);
        }
    }
}