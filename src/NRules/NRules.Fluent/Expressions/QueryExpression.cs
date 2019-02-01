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
        private readonly ParameterExpression _symbol;
        private readonly SymbolStack _symbolStack;
        private readonly GroupBuilder _groupBuilder;
        private Func<string, PatternBuilder> _buildAction;

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
                return patternBuilder;
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
                return patternBuilder;
            };
        }

        public void Where<TSource>(Expression<Func<TSource, bool>>[] predicates)
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = previousBuildAction(name);
                patternBuilder.DslConditions(_symbolStack.Scope.Declarations, predicates);
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
                    var sourceBuilder = previousBuildAction(null);
                    var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, selector);
                    aggregateBuilder.Project(selectorExpression);
                    aggregateBuilder.Pattern(sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
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
                    var sourceBuilder = previousBuildAction(null);
                    var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, selector);
                    aggregateBuilder.Flatten(selectorExpression);
                    aggregateBuilder.Pattern(sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
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
                    var sourceBuilder = previousBuildAction(null);
                    var keySelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, keySelector);
                    var elementSelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, elementSelector);
                    aggregateBuilder.GroupBy(keySelectorExpression, elementSelectorExpression);
                    aggregateBuilder.Pattern(sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
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
                    var sourceBuilder = previousBuildAction(null);

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
                return patternBuilder;
            };
        }

        public void Collect<TSource>()
        {
            var previousBuildAction = _buildAction;
            _buildAction = name =>
            {
                var patternBuilder = new PatternBuilder(typeof(IEnumerable<TSource>), name);

                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var sourceBuilder = previousBuildAction(null);
                    aggregateBuilder.Collect();
                    aggregateBuilder.Pattern(sourceBuilder);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
            };
        }

        public PatternBuilder Build()
        {
            var patternBuilder = _buildAction(_symbol.Name);
            _groupBuilder.Pattern(patternBuilder);
            return patternBuilder;
        }
    }
}