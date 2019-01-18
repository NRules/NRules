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
        private Func<IPatternContainerBuilder, string, PatternBuilder> _buildAction;

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
            _buildAction = (b, n) =>
            {
                var patternBuilder = b.Pattern(typeof(TSource), n);
                patternBuilder.DslConditions(_symbolStack.Scope.Declarations, conditions);
                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
            };
        }

        public void From<TSource>(Expression<Func<TSource>> source)
        {
            _buildAction = (b, n) =>
            {
                var patternBuilder = b.Pattern(typeof(TSource), n);

                var bindingBuilder = patternBuilder.Binding();
                bindingBuilder.DslBindingExpression(_symbolStack.Scope.Declarations, source);
                
                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
            };
        }

        public void Where<TSource>(Expression<Func<TSource, bool>>[] predicates)
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var patternBuilder = previousBuildAction(b, n);
                patternBuilder.DslConditions(_symbolStack.Scope.Declarations, predicates);
                return patternBuilder;
            };
        }

        public void Select<TSource, TResult>(Expression<Func<TSource, TResult>> selector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var patternBuilder = b.Pattern(typeof(TResult), n);

                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var sourceBuilder = previousBuildAction(aggregateBuilder, null);
                    var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, selector);
                    aggregateBuilder.Project(selectorExpression);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
            };
        }

        public void SelectMany<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var patternBuilder = b.Pattern(typeof(TResult), n);

                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var sourceBuilder = previousBuildAction(aggregateBuilder, null);
                    var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, selector);
                    aggregateBuilder.Flatten(selectorExpression);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
            };
        }

        public void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var patternBuilder = b.Pattern(typeof(IGrouping<TKey, TElement>), n);

                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var sourceBuilder = previousBuildAction(aggregateBuilder, null);
                    var keySelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, keySelector);
                    var elementSelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, elementSelector);
                    aggregateBuilder.GroupBy(keySelectorExpression, elementSelectorExpression);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
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
                var patternBuilder = b.Pattern(typeof(TResult), n);

                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    var sourceBuilder = previousBuildAction(aggregateBuilder, null);

                    var rewrittenExpressionMap = new Dictionary<string, LambdaExpression>();
                    foreach (var expression in expressionMap)
                    {
                        var lambda = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, expression.Value);
                        rewrittenExpressionMap[expression.Key] = lambda;
                    }

                    aggregateBuilder.Aggregator(name, rewrittenExpressionMap, customFactoryType);
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
            };
        }

        public void Collect<TSource>()
        {
            var previousBuildAction = _buildAction;
            _buildAction = (b, n) =>
            {
                var patternBuilder = b.Pattern(typeof(IEnumerable<TSource>), n);

                using (_symbolStack.Frame())
                {
                    var aggregateBuilder = patternBuilder.Aggregate();
                    previousBuildAction(aggregateBuilder, null);
                    aggregateBuilder.Collect();
                }

                _symbolStack.Scope.Add(patternBuilder.Declaration);
                return patternBuilder;
            };
        }

        public Declaration Build()
        {
            var patternBuilder = _buildAction(_groupBuilder, _symbol.Name);
            return patternBuilder.Declaration;
        }
    }
}