using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class QueryExpression : IQuery, IQueryBuilder
    {
        private readonly ParameterExpression _symbol;
        private readonly SymbolStack _symbolStack;
        private readonly GroupBuilder _groupBuilder;
        private Func<string, AggregateBuildResult> _buildAction;

        public QueryExpression(ParameterExpression symbol, SymbolStack symbolStack, GroupBuilder groupBuilder)
        {
            _symbol = symbol;
            _symbolStack = symbolStack;
            _groupBuilder = groupBuilder;
            Builder = this;
        }

        public IQueryBuilder Builder { get; }

        public void FactQuery<TSource>(Expression<Func<TSource, bool>>[] conditions)
        {
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(TSource), name);
                patternBuilder.DslConditions(_symbolStack.Scope.Declarations, conditions);
                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return new AggregateBuildResult(patternBuilder);
            };
        }

        public void From<TSource>(Expression<Func<TSource>> source)
        {
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(TSource), name);

                var bindingBuilder = patternBuilder.Binding();
                bindingBuilder.DslBindingExpression(_symbolStack.Scope.Declarations, source);

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return new AggregateBuildResult(patternBuilder);
            };
        }

        public void Where<TSource>(Expression<Func<TSource, bool>>[] predicates)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = previousBuildAction(name);
                patternBuilder.Pattern.DslConditions(_symbolStack.Scope.Declarations, predicates);
                return patternBuilder;
            };
        }

        public void Select<TSource, TResult>(Expression<Func<TSource, TResult>> selector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(TResult), name);

                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var sourceBuilder = previousBuildAction(null).Pattern;
                    var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, selector);
                    aggregateBuilder.Project(selectorExpression);
                    aggregateBuilder.Pattern(sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return new AggregateBuildResult(patternBuilder);
            };
        }

        public void SelectMany<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(TResult), name);

                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var sourceBuilder = previousBuildAction(null).Pattern;
                    var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, selector);
                    aggregateBuilder.Flatten(selectorExpression);
                    aggregateBuilder.Pattern(sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return new AggregateBuildResult(patternBuilder);
            };
        }

        public void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(IGrouping<TKey, TElement>), name);

                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var sourceBuilder = previousBuildAction(null).Pattern;
                    var keySelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, keySelector);
                    var elementSelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, elementSelector);
                    aggregateBuilder.GroupBy(keySelectorExpression, elementSelectorExpression);
                    aggregateBuilder.Pattern(sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return new AggregateBuildResult(patternBuilder);
            };
        }

        public void OrderBy<TSource, TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            OrderBy(keySelector, SortDirection.Ascending);
        }

        public void OrderByDescending<TSource, TKey>(Expression<Func<TSource, TKey>> keySelector)
        {
            OrderBy(keySelector, SortDirection.Descending);
        }

        private void OrderBy<TSource, TKey>(Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var collectBuilder = previousBuildAction(null);
                var keySelectorExpression = collectBuilder.Source.DslPatternExpression(_symbolStack.Scope.Declarations, keySelector);
                collectBuilder.Aggregate.Sort(keySelectorExpression, sortDirection);
                return collectBuilder;
            };
        }

        public void Aggregate<TSource, TResult>(string aggregateName, IDictionary<string, LambdaExpression> expressionMap)
        {
            Aggregate<TSource, TResult>(aggregateName, expressionMap, null);
        }

        public void Aggregate<TSource, TResult>(string aggregateName, IDictionary<string, LambdaExpression> expressionMap, Type customFactoryType)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(TResult), name);

                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var sourceBuilder = previousBuildAction(null).Pattern;

                    var rewrittenExpressionMap = new Dictionary<string, LambdaExpression>();
                    foreach (var item in expressionMap)
                    {
                        var expression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, item.Value);
                        rewrittenExpressionMap[item.Key] = expression;
                    }

                    aggregateBuilder.Aggregator(aggregateName, rewrittenExpressionMap, customFactoryType);
                    aggregateBuilder.Pattern(sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return new AggregateBuildResult(patternBuilder);
            };
        }

        public void Collect<TSource>()
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(IEnumerable<TSource>), name);

                AggregateBuildResult result;
                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var sourceBuilder = previousBuildAction(null).Pattern;
                    aggregateBuilder.Collect();
                    aggregateBuilder.Pattern(sourceBuilder);

                    result = new AggregateBuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return result;
            };
        }

        public PatternBuilder Build()
        {
            var patternBuilder = _buildAction(_symbol.Name);
            _groupBuilder.Pattern(patternBuilder.Pattern);
            return patternBuilder.Pattern;
        }

        private class AggregateBuildResult
        {
            public AggregateBuildResult(PatternBuilder pattern, AggregateBuilder aggregate, PatternBuilder source)
                : this(pattern)
            {
                Aggregate = aggregate;
                Source = source;
            }

            public AggregateBuildResult(PatternBuilder pattern)
            {
                Pattern = pattern;
            }

            public PatternBuilder Pattern { get; }
            public AggregateBuilder Aggregate { get; }
            public PatternBuilder Source { get; }
        }
    }
}