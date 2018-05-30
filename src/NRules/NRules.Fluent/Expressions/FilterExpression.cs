using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class FilterExpression : IFilterExpression
    {
        private readonly RuleBuilder _builder;

        public FilterExpression(RuleBuilder builder)
        {
            _builder = builder;
        }

        public IFilterExpression OnChange(params Expression<Func<object>>[] keySelectors)
        {
            var filters = _builder.Filters();
            foreach (var keySelector in keySelectors)
            {
                var expression = keySelector.DslExpression(filters.Declarations);
                filters.Filter(FilterType.KeyChange, expression);
            }

            return this;
        }

        public IFilterExpression OnChange<T>(Expression<Func<T>> keySelector, Expression<Func<IEqualityComparer<T>>> comparer) where T : class
        {
            var keySelectorInternalized = InternalizeComparer(keySelector, comparer);
            return OnChange(keySelectorInternalized);
        }

        public IFilterExpression Where(params Expression<Func<bool>>[] predicates)
        {
            var filters = _builder.Filters();
            foreach (var predicate in predicates)
            {
                var expression = predicate.DslExpression(filters.Declarations);
                filters.Filter(FilterType.Predicate, expression);
            }

            return this;
        }

        private Expression<Func<object>> InternalizeComparer<T>(Expression<Func<T>> keySelector, Expression<Func<IEqualityComparer<T>>> comparer) where T : class
        {
            var keyExpr = Expression.Invoke(keySelector);
            var compExpr = Expression.Invoke(comparer);
            var ctorInfo = typeof(EqualityWrapper<T>).GetTypeInfo().DeclaredConstructors.First();
            return Expression.Lambda<Func<object>>(Expression.Convert(Expression.New(ctorInfo, keyExpr, compExpr), typeof(object)));
        }
    }

    internal class EqualityWrapper<T> : IEquatable<EqualityWrapper<T>>
    {
        public EqualityWrapper(T value, IEqualityComparer<T> comparer)
        {
            Value = value;
            Comparer = comparer;
        }

        public T Value { get; }
        public IEqualityComparer<T> Comparer { get; }

        public bool Equals(EqualityWrapper<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Comparer.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EqualityWrapper<T>) obj);
        }

        public override int GetHashCode() { return Comparer.GetHashCode(Value); }
    }
}