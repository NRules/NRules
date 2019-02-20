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
        private Func<string, BuildResult> _buildAction;

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
                return new BuildResult(patternBuilder);
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
                return new BuildResult(patternBuilder);
            };
        }

        public void Where<TSource>(Expression<Func<TSource, bool>>[] predicates)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var result = previousBuildAction(name);
                result.Pattern.DslConditions(_symbolStack.Scope.Declarations, predicates);
                return result;
            };
        }

        public void Select<TSource, TResult>(Expression<Func<TSource, TResult>> selector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(TResult), name);

                BuildResult result;
                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var previousResult = previousBuildAction(null);
                    var sourceBuilder = previousResult.Pattern;
                    var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, selector);
                    aggregateBuilder.Project(selectorExpression);
                    aggregateBuilder.Pattern(sourceBuilder);
                    result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return result;
            };
        }

        public void SelectMany<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(TResult), name);

                BuildResult result;
                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var previousResult = previousBuildAction(null);
                    var sourceBuilder = previousResult.Pattern;
                    var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, selector);
                    aggregateBuilder.Flatten(selectorExpression);
                    aggregateBuilder.Pattern(sourceBuilder);
                    result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return result;
            };
        }

        public void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(IGrouping<TKey, TElement>), name);

                BuildResult result;
                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var previousResult = previousBuildAction(null);
                    var sourceBuilder = previousResult.Pattern;
                    var keySelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, keySelector);
                    var elementSelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, elementSelector);
                    aggregateBuilder.GroupBy(keySelectorExpression, elementSelectorExpression);
                    aggregateBuilder.Pattern(sourceBuilder);
                    result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return result;
            };
        }

        public void Collect<TSource>()
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(IEnumerable<TSource>), name);

                BuildResult result;
                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var previousResult = previousBuildAction(null);
                    var sourceBuilder = previousResult.Pattern;
                    aggregateBuilder.Collect();
                    aggregateBuilder.Pattern(sourceBuilder);

                    result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return result;
            };
        }

        public void OrderBy<TSource, TKey>(Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var result = previousBuildAction(null);
                var keySelectorExpression = result.Source.DslPatternExpression(_symbolStack.Scope.Declarations, keySelector);
                result.Aggregate.OrderBy(keySelectorExpression, sortDirection);
                return result;
            };
        }

        public void Aggregate<TSource, TResult>(string aggregateName, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions)
        {
            Aggregate<TSource, TResult>(aggregateName, expressions, null);
        }

        public void Aggregate<TSource, TResult>(string aggregateName, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions, Type customFactoryType)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(TResult), name);

                BuildResult result;
                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var previousResult = previousBuildAction(null);
                    var sourceBuilder = previousResult.Pattern;

                    var rewrittenExpressionCollection = new List<KeyValuePair<string, LambdaExpression>>();
                    foreach (var item in expressions)
                    {
                        var expression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, item.Value);
                        rewrittenExpressionCollection.Add(new KeyValuePair<string, LambdaExpression>(item.Key, expression));
                    }

                    aggregateBuilder.Aggregator(aggregateName, rewrittenExpressionCollection, customFactoryType);
                    aggregateBuilder.Pattern(sourceBuilder);

                    result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
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

        private class BuildResult
        {
            public BuildResult(PatternBuilder pattern, AggregateBuilder aggregate, PatternBuilder source)
                : this(pattern)
            {
                Aggregate = aggregate;
                Source = source;
            }

            public BuildResult(PatternBuilder pattern)
            {
                Pattern = pattern;
            }

            public PatternBuilder Pattern { get; }
            public AggregateBuilder Aggregate { get; }
            public PatternBuilder Source { get; }
        }
    }
}