using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class QueryExpression : IQuery, IQueryBuilder
    {
        private readonly GroupBuilder _groupBuilder;
        private Func<IPatternContainerBuilder, string, PatternBuilder> _buildAction;

        public QueryExpression(GroupBuilder groupBuilder)
        {
            _groupBuilder = groupBuilder;
            Builder = this;
        }

        public IQueryBuilder Builder { get; }

        public void FactQuery<TFact>(Expression<Func<TFact>> alias, Expression<Func<TFact, bool>>[] conditions)
        {
            var symbol = alias.ToParameterExpression();
            var patternBuilder = _groupBuilder.Pattern(symbol.Type, symbol.Name);
            patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
        }

        public void FactQuery<TSource>(Expression<Func<TSource, bool>>[] conditions)
        {
            _buildAction = (b, n) =>
            {
                var patternBuilder = b.Pattern(typeof(TSource), n);
                patternBuilder.DslConditions(_groupBuilder.Declarations, conditions);
                return patternBuilder;
            };
        }

        public void Query<TResult>(Expression<Func<TResult>> alias, Func<IQuery, IQuery<TResult>> queryAction)
        {
            var symbol = alias.ToParameterExpression();
            var patternBuilder = _groupBuilder.Pattern(typeof(TResult), symbol.Name);
            var groupBuilder = patternBuilder.Group(GroupType.And);
            var queryBuilder = new QueryExpression(groupBuilder);
            queryAction(queryBuilder);
            queryBuilder.Build(null);
        }

        public void Query<TSource>(Func<IQuery, IQuery<TSource>> queryAction)
        {
            _buildAction = (b, n) =>
            {
                var patternBuilder = b.Pattern(typeof(TSource), n);
                var groupBuilder = patternBuilder.Group(GroupType.And);
                var queryBuilder = new QueryExpression(groupBuilder);
                queryAction(queryBuilder);
                queryBuilder.Build(null);
                return patternBuilder;
            };
        }

        public void From<TSource>(Expression<Func<TSource>> source)
        {
            _buildAction = (b, n) =>
            {
                var patternBuilder = b.Pattern(typeof(TSource), n);
                var bindingBuilder = patternBuilder.Binding();
                bindingBuilder.DslBindingExpression(_groupBuilder.Declarations, source);
                return patternBuilder;
            };
        }

        public void Where<TSource>(Expression<Func<TSource, bool>>[] predicates)
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var patternBuilder = previousBuildAction(b, n);
                patternBuilder.DslConditions(_groupBuilder.Declarations, predicates);
                return patternBuilder;
            };
        }

        public void Select<TSource, TResult>(Expression<Func<TSource, TResult>> selector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var aggregatePatternBuilder = b.Pattern(typeof(TResult), n);
                var aggregateBuilder = aggregatePatternBuilder.Aggregate();
                var sourceBuilder = previousBuildAction(aggregateBuilder, null);
                var selectorExpression = sourceBuilder.DslPatternExpression(_groupBuilder.Declarations, selector);
                aggregateBuilder.Project(selectorExpression);
                return aggregatePatternBuilder;
            };
        }

        public void SelectMany<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var aggregatePatternBuilder = b.Pattern(typeof(TResult), n);
                var aggregateBuilder = aggregatePatternBuilder.Aggregate();
                var sourceBuilder = previousBuildAction(aggregateBuilder, null);
                var selectorExpression = sourceBuilder.DslPatternExpression(_groupBuilder.Declarations, selector);
                aggregateBuilder.Flatten(selectorExpression);
                return aggregatePatternBuilder;
            };
        }

        public void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var aggregatePatternBuilder = b.Pattern(typeof(IGrouping<TKey, TElement>), n);
                var aggregateBuilder = aggregatePatternBuilder.Aggregate();
                var sourceBuilder = previousBuildAction(aggregateBuilder, null);
                var keySelectorExpression = sourceBuilder.DslPatternExpression(_groupBuilder.Declarations, keySelector);
                var elementSelectorExpression = sourceBuilder.DslPatternExpression(_groupBuilder.Declarations, elementSelector);
                aggregateBuilder.GroupBy(keySelectorExpression, elementSelectorExpression);
                return aggregatePatternBuilder;
            };
        }

        public void Aggregate<TSource, TResult>(string name, IDictionary<string, LambdaExpression> expressionMap)
        {
            Aggregate<TSource, TResult>(name, expressionMap, null);
        }

        public void Aggregate<TSource, TResult>(string name, IDictionary<string, LambdaExpression> expressionMap, Type customFactoryType)
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var aggregatePatternBuilder = b.Pattern(typeof(TResult), n);
                var aggregateBuilder = aggregatePatternBuilder.Aggregate();
                var sourceBuilder = previousBuildAction(aggregateBuilder, null);

                var rewrittenExpressionMap = new Dictionary<string, LambdaExpression>();
                foreach (var expression in expressionMap)
                {
                    var lambda = sourceBuilder.DslPatternExpression(_groupBuilder.Declarations, expression.Value);
                    rewrittenExpressionMap[expression.Key] = lambda;
                }

                aggregateBuilder.Aggregator(name, rewrittenExpressionMap, customFactoryType);
                return aggregatePatternBuilder;
            };
        }

        public void Collect<TSource>()
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var aggregatePatternBuilder = b.Pattern(typeof(IEnumerable<TSource>), n);
                var aggregateBuilder = aggregatePatternBuilder.Aggregate();
                previousBuildAction(aggregateBuilder, null);
                aggregateBuilder.Collect();
                return aggregatePatternBuilder;
            };
        }

        public void Build(string symbolName)
        {
            _buildAction(_groupBuilder, symbolName);
        }
    }
}