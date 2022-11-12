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
    {
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(TSource), name);
            patternBuilder.DslConditions(_symbolStack.Scope.Declarations, conditions);
            var result = new BuildResult(patternBuilder);
            _symbolStack.Scope.Add(result.Pattern.Declaration);
            return result;
        };
    }

    public void From<TSource>(Expression<Func<TSource>> source)
    {
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(TSource), name);

            var bindingBuilder = patternBuilder.Binding();
            bindingBuilder.DslBindingExpression(_symbolStack.Scope.Declarations, source);
            var result = new BuildResult(patternBuilder);
            _symbolStack.Scope.Add(result.Pattern.Declaration);
            return result;
        };
    }

    public void Where<TSource>(Expression<Func<TSource, bool>>[] predicates)
    {
        if (_buildAction is null)
        {
            throw new InvalidOperationException("Call From* first");
        }

        var previousBuildAction = _buildAction;
        _buildAction = (name, type) =>
        {
            var result = previousBuildAction(name, type);
            result.Pattern.DslConditions(_symbolStack.Scope.Declarations, predicates);
            return result;
        };
    }

    public void Select<TSource, TResult>(Expression<Func<TSource, TResult>> selector)
    {
        if (_buildAction is null)
        {
            throw new InvalidOperationException("Call From* first");
        }

        var previousBuildAction = _buildAction;
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(TResult), name);

            BuildResult result;
            using (_symbolStack.Frame())
            {
                var aggregateBuilder = patternBuilder.Aggregate();
                var previousResult = previousBuildAction(null, null);
                var sourceBuilder = previousResult.Pattern;
                var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, selector);
                aggregateBuilder.Project(selectorExpression);
                aggregateBuilder.Pattern(sourceBuilder);
                result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
            }

            _symbolStack.Scope.Add(result.Pattern.Declaration);
            return result;
        };
    }

    public void SelectMany<TSource, TResult>(Expression<Func<TSource, IEnumerable<TResult>>> selector)
    {
        if (_buildAction is null)
        {
            throw new InvalidOperationException("Call From* first");
        }

        var previousBuildAction = _buildAction;
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(TResult), name);

            BuildResult result;
            using (_symbolStack.Frame())
            {
                var aggregateBuilder = patternBuilder.Aggregate();
                var previousResult = previousBuildAction(null, null);
                var sourceBuilder = previousResult.Pattern;
                var selectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, selector);
                aggregateBuilder.Flatten(selectorExpression);
                aggregateBuilder.Pattern(sourceBuilder);
                result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
            }

            _symbolStack.Scope.Add(result.Pattern.Declaration);
            return result;
        };
    }

    public void GroupBy<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
    {
        if (_buildAction is null)
        {
            throw new InvalidOperationException("Call From* first");
        }

        var previousBuildAction = _buildAction;
        _buildAction = (name, _) =>
        {
            var sourceBuilder = previousBuildAction(null, null).Pattern;
            var result = BuildGroupBy(name, sourceBuilder, keySelector, elementSelector);
            _symbolStack.Scope.Add(result.Pattern.Declaration);
            return result;
        };
    }

    private BuildResult BuildGroupBy<TSource, TKey, TElement>(string? name, PatternBuilder sourceBuilder, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
    {
        var patternBuilder = new PatternBuilder(typeof(IGrouping<TKey, TElement>), name);
        using var _ = _symbolStack.Frame();

        var keySelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, keySelector);
        var elementSelectorExpression = sourceBuilder.DslPatternExpression(_symbolStack.Scope.Declarations, elementSelector);

        var aggregateBuilder = patternBuilder.Aggregate();
        aggregateBuilder.GroupBy(keySelectorExpression, elementSelectorExpression);
        aggregateBuilder.Pattern(sourceBuilder);
        return new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
    }

    public void Collect<TSource>()
    {
        if (_buildAction is null)
        {
            throw new InvalidOperationException("Call From* first");
        }

        var previousBuildAction = _buildAction;
        _buildAction = (name, type) =>
        {
            var patternBuilder = new PatternBuilder(type ?? typeof(IEnumerable<TSource>), name);

            BuildResult result;
            using (_symbolStack.Frame())
            {
                var aggregateBuilder = patternBuilder.Aggregate();
                var previousResult = previousBuildAction(null, null);
                var sourceBuilder = previousResult.Pattern;
                aggregateBuilder.Collect();
                aggregateBuilder.Pattern(sourceBuilder);

                result = new BuildResult(patternBuilder, aggregateBuilder, sourceBuilder);
            }

            _symbolStack.Scope.Add(result.Pattern.Declaration);
            return result;
        };
    }

    public void ToLookup<TSource, TKey, TElement>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
    {
        if (_buildAction is null)
        {
            throw new InvalidOperationException("Call From* first");
        }

        var previousBuildAction = _buildAction;
        _buildAction = (name, _) =>
        {
            var result = previousBuildAction(name, typeof(IKeyedLookup<TKey, TElement>));
            if (result.Source is { } source && result.Aggregate is { } aggregate)
            {
                var keySelectorExpression = source.DslPatternExpression(_symbolStack.Scope.Declarations, keySelector);
                var elementSelectorExpression = source.DslPatternExpression(_symbolStack.Scope.Declarations, elementSelector);
                aggregate.ToLookup(keySelectorExpression, elementSelectorExpression);
                return result;
            }
            throw new InvalidOperationException();
        };
    }

    public void OrderBy<TSource, TKey>(Expression<Func<TSource, TKey>> keySelector, SortDirection sortDirection)
    {
        if (_buildAction is null)
        {
            throw new InvalidOperationException("Call From* first");
        }

        var previousBuildAction = _buildAction;
        _buildAction = (name, type) =>
        {
            var result = previousBuildAction(name, type);
            if (result.Source is { } source && result.Aggregate is { } aggregate)
            {
                var keySelectorExpression = source.DslPatternExpression(_symbolStack.Scope.Declarations, keySelector);
                aggregate.OrderBy(keySelectorExpression, sortDirection);
                return result;
            }
            throw new InvalidOperationException();
        };
    }

    public void Aggregate<TSource, TResult>(string aggregateName, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions)
    {
        Aggregate<TSource, TResult>(aggregateName, expressions, null);
    }

    public void Aggregate<TSource, TResult>(string aggregateName, IEnumerable<KeyValuePair<string, LambdaExpression>> expressions, Type? customFactoryType)
    {
        if (_buildAction is null)
        {
            throw new InvalidOperationException("Call From* first");
        }

        var previousBuildAction = _buildAction;
        _buildAction = (name, _) =>
        {
            var patternBuilder = new PatternBuilder(typeof(TResult), name);

            BuildResult result;
            using (_symbolStack.Frame())
            {
                var aggregateBuilder = patternBuilder.Aggregate();
                var previousResult = previousBuildAction(null, null);
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

            _symbolStack.Scope.Add(result.Pattern.Declaration);
            return result;
        };
    }

    public PatternBuilder Build()
    {
        if (_buildAction is null)
        {
            throw new InvalidOperationException("Call From* first");
        }

        var patternBuilder = _buildAction(_symbol.Name, null);
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
        public AggregateBuilder? Aggregate { get; }
        public PatternBuilder? Source { get; }
    }
}