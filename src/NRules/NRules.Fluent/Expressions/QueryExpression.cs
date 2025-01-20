using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions;

internal class QueryExpression : IQuery, IQueryBuilder
{
    private readonly ParameterExpression _symbol;
    private readonly SymbolStack _symbolStack;
    private readonly GroupBuilder _groupBuilder;
    private Func<string?, Type?, BuildResult>? _buildAction;

    public QueryExpression(ParameterExpression symbol, SymbolStack symbolStack, GroupBuilder groupBuilder)
    {
        _symbol = symbol;
        _symbolStack = symbolStack;
        _groupBuilder = groupBuilder;
        Builder = this;
    }

    public IQueryBuilder Builder { get; }

    public void FactQuery<TSource>(Expression<Func<TSource, bool>>[] conditions)
        where TSource : notnull
    {
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(TSource), name);
            patternBuilder.DslConditions(_symbolStack.Scope, conditions);
            _symbolStack.Scope.Add(patternBuilder.Declaration);
            return new BuildResult(patternBuilder);
        };
    }

    public void From<TSource>(Expression<Func<TSource>> source)
        where TSource : notnull
    {
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(TSource), name);

            var bindingBuilder = patternBuilder.Binding();
            bindingBuilder.DslBindingExpression(_symbolStack.Scope, source);

            _symbolStack.Scope.Add(patternBuilder.Declaration);
            return new BuildResult(patternBuilder);
        };
    }

    public void Where<TSource>(Expression<Func<TSource, bool>>[] predicates)
        where TSource : notnull
    {
        var previousBuildAction = _buildAction ?? throw new ArgumentNullException(nameof(_buildAction));
        _buildAction = (name, type) =>
        {
            var result = previousBuildAction(name, type);
            var patternBuilder = result.Pattern ?? throw new ArgumentException("Query source pattern is not specified");
            patternBuilder.DslConditions(_symbolStack.Scope, predicates);
            return result;
        };
    }

    public void Select<TSource, TResult>(Expression<Func<TSource, TResult>> selector)
        where TSource : notnull
    {
        var previousBuildAction = _buildAction ?? throw new ArgumentNullException(nameof(_buildAction));
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(TResult), name);

            BuildResult result;
            using (_symbolStack.Frame())
            {
                var aggregateBuilder = patternBuilder.Aggregate();
                var previousResult = previousBuildAction(null, null);
                var sourceBuilder = previousResult.Pattern ?? throw new ArgumentException("Query source pattern is not specified");
                var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope, selector);
                aggregateBuilder.Project(selectorExpression);
                aggregateBuilder.Pattern(sourceBuilder);
                result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
            }

            _symbolStack.Scope.Add(patternBuilder.Declaration);
            return result;
        };
    }

    public void SelectMany<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector)
        where TSource : notnull
    {
        var previousBuildAction = _buildAction ?? throw new ArgumentNullException(nameof(_buildAction));
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(TResult), name);

            BuildResult result;
            using (_symbolStack.Frame())
            {
                var aggregateBuilder = patternBuilder.Aggregate();
                var previousResult = previousBuildAction(null, null);
                var sourceBuilder = previousResult.Pattern ?? throw new ArgumentException("Query source pattern is not specified");
                var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope, selector);
                aggregateBuilder.Flatten(selectorExpression);
                aggregateBuilder.Pattern(sourceBuilder);
                result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
            }

            _symbolStack.Scope.Add(patternBuilder.Declaration);
            return result;
        };
    }

    public void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        where TSource : notnull
        where TKey : notnull
        where TElement : notnull
    {
        var previousBuildAction = _buildAction ?? throw new ArgumentNullException(nameof(_buildAction));
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(IGrouping<TKey, TElement>), name);

            BuildResult result;
            using (_symbolStack.Frame())
            {
                var aggregateBuilder = patternBuilder.Aggregate();
                var previousResult = previousBuildAction(null, null);
                var sourceBuilder = previousResult.Pattern ?? throw new ArgumentException("Query source pattern is not specified");
                var keySelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope, keySelector);
                var elementSelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope, elementSelector);
                aggregateBuilder.GroupBy(keySelectorExpression, elementSelectorExpression);
                aggregateBuilder.Pattern(sourceBuilder);
                result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
            }

            _symbolStack.Scope.Add(patternBuilder.Declaration);
            return result;
        };
    }

    public void Collect<TSource>()
        where TSource : notnull
    {
        var previousBuildAction = _buildAction ?? throw new ArgumentNullException(nameof(_buildAction));
        _buildAction = (name, type) =>
        {
            var patternBuilder = new PatternBuilder(type ?? typeof(IEnumerable<TSource>), name);

            BuildResult result;
            using (_symbolStack.Frame())
            {
                var aggregateBuilder = patternBuilder.Aggregate();
                var previousResult = previousBuildAction(null, null);
                var sourceBuilder = previousResult.Pattern ?? throw new ArgumentException("Query source pattern is not specified");
                aggregateBuilder.Collect();
                aggregateBuilder.Pattern(sourceBuilder);

                result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
            }

            _symbolStack.Scope.Add(patternBuilder.Declaration);
            return result;
        };
    }

    public void ToLookup<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        where TSource : notnull
        where TElement : notnull
    {
        var previousBuildAction = _buildAction ?? throw new ArgumentNullException(nameof(_buildAction));
        _buildAction = (name, _) =>
        {
            var result = previousBuildAction(name, typeof(IKeyedLookup<TKey, TElement>));
            var sourceBuilder = result.Source ?? throw new ArgumentException("Query source is not specified");
            var keySelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope, keySelector);
            var elementSelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope, elementSelector);
            var aggregateBuilder = result.Aggregate ?? throw new ArgumentException("Query aggregate is not specified");
            aggregateBuilder.ToLookup(keySelectorExpression, elementSelectorExpression);
            return result;
        };
    }

    public void OrderBy<TSource, TKey>(Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
        where TSource : notnull
    {
        var previousBuildAction = _buildAction ?? throw new ArgumentNullException(nameof(_buildAction));
        _buildAction = (name, type) =>
        {
            var result = previousBuildAction(name, type);
            var sourceBuilder = result.Source ?? throw new ArgumentException("Query source is not specified");
            var keySelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope, keySelector);
            var aggregateBuilder = result.Aggregate ?? throw new ArgumentException("Query aggregate is not specified");
            aggregateBuilder.OrderBy(keySelectorExpression, sortDirection);
            return result;
        };
    }

    public void Aggregate<TSource, TResult>(string aggregateName, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions)
        where TSource : notnull
    {
        Aggregate<TSource, TResult>(aggregateName, expressions, null);
    }

    public void Aggregate<TSource, TResult>(string aggregateName, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions, Type? customFactoryType)
        where TSource : notnull
    {
        var previousBuildAction = _buildAction ?? throw new ArgumentNullException(nameof(_buildAction));
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(TResult), name);

            BuildResult result;
            using (_symbolStack.Frame())
            {
                var aggregateBuilder = patternBuilder.Aggregate();
                var previousResult = previousBuildAction(null, null);
                var sourceBuilder = previousResult.Pattern ?? throw new ArgumentException("Query source pattern is not specified");

                var rewrittenExpressionCollection = new List<KeyValuePair<string, LambdaExpression>>();
                foreach (var item in expressions)
                {
                    var expression = sourceBuilder.DslPatternExpression(_symbolStack.Scope, item.Value);
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
        if (_buildAction == null) throw new ArgumentException("Query build action is not specified");
        var patternBuilder = _buildAction(_symbol.Name, null).Pattern ?? throw new ArgumentException("Query source pattern is not specified");
        _groupBuilder.Pattern(patternBuilder);
        return patternBuilder;
    }
    
    private sealed class BuildResult(PatternBuilder pattern)
    {
        public BuildResult(PatternBuilder pattern, AggregateBuilder aggregate, PatternBuilder source)
            : this(pattern)
        {
            Aggregate = aggregate;
            Source = source;
        }

        public PatternBuilder? Pattern { get; } = pattern;
        public AggregateBuilder? Aggregate { get; }
        public PatternBuilder? Source { get; }
    }
}