using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using NRules.Aggregators;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class CustomFirstAggregatorTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_NoMatchingFacts_DoesNotFire()
    {
        //Arrange - Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_TwoGroupsOfMatchingFacts_FiresTwiceWithFirstValidFactInEachGroup()
    {
        //Arrange
        var fact1 = new FactType { GroupProperty = "1", TestProperty = "Valid Value 1" };
        var fact2 = new FactType { GroupProperty = "1", TestProperty = "Valid Value 2" };
        var fact3 = new FactType { GroupProperty = "2", TestProperty = "Invalid Value 3" };
        var fact4 = new FactType { GroupProperty = "2", TestProperty = "Valid Value 4" };

        var facts = new[] { fact1, fact2, fact3, fact4 };
        Session.InsertAll(facts);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact<FactType>(f => f.TestProperty == "Valid Value 1"));
            s.Rule().Fired(Matched.Fact<FactType>(f => f.TestProperty == "Valid Value 4"));
        });
    }

    [Fact]
    public void Fire_OneGroupsOfMatchingFactsThenFirstFactRetracted_FiresOnceWithSecondFact()
    {
        //Arrange
        var fact1 = new FactType { GroupProperty = "1", TestProperty = "Valid Value 1" };
        var fact2 = new FactType { GroupProperty = "1", TestProperty = "Valid Value 2" };
        var fact3 = new FactType { GroupProperty = "1", TestProperty = "Valid Value 3" };

        var facts = new[] { fact1, fact2, fact3 };
        Session.InsertAll(facts);
        Session.Retract(fact1);

        //Act
        Session.Fire();

        //Assert
        Verify(s =>
        {
            s.Rule().Fired(Matched.Fact<FactType>(f => f.TestProperty == "Valid Value 2"));
        });
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule>();
    }

    public class FactType
    {
        [NotNull]
        public string? GroupProperty { get; set; }
        [NotNull]
        public string? TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            FactType first = null!;

            When()
                .Query(() => first, q => q
                    .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                    .GroupBy(x => x.GroupProperty)
                    .First());
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}

public static class FirstQueryExtensions
{
    public static IQuery<TSource> First<TSource>(this IQuery<IEnumerable<TSource>> source)
    {
        var expressions = new List<KeyValuePair<string, LambdaExpression>>();
        source.Builder.Aggregate<IEnumerable<TSource>, TSource>("First", expressions, typeof(CustomFirstAggregatorFactory));
        return new QueryExpression<TSource>(source.Builder);
    }
}

internal class CustomFirstAggregatorFactory : IAggregatorFactory
{
    private Func<IAggregator> _factory = null!;
    
    public void Compile(AggregateElement element, IReadOnlyCollection<IAggregateExpression> compiledExpressions)
    {
        var elementType = element.ResultType;
        var aggregatorType = typeof(CustomFirstAggregator<>).MakeGenericType(elementType);
        var factoryExpression = Expression.Lambda<Func<IAggregator>>(Expression.New(aggregatorType));
        _factory = factoryExpression.Compile();
    }

    public IAggregator Create()
    {
        return _factory();
    }
}

public class CustomFirstAggregator<TElement> : IAggregator
{
    private readonly Dictionary<object, TElement> _firstElements = new();

    public IReadOnlyCollection<AggregationResult> Add(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        var results = new List<AggregationResult>();
        foreach (var fact in facts)
        {
            var collection = (IEnumerable<TElement>)fact.Value!;
            foreach (var value in collection)
            {
                _firstElements[fact] = value;
                results.Add(AggregationResult.Added(value!, Enumerable.Repeat(fact, 1)));
                break;
            }
        }
        return results;
    }

    public IReadOnlyCollection<AggregationResult> Modify(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        var results = new List<AggregationResult>();
        foreach (var fact in facts)
        {
            var collection = (IEnumerable<TElement>)fact.Value!;
            foreach (var value in collection)
            {
                if (_firstElements.TryGetValue(fact, out var oldFirst))
                {
                    if (Equals(oldFirst, value))
                    {
                        results.Add(AggregationResult.Modified(value!, value!, Enumerable.Repeat(fact, 1)));
                    }
                    else
                    {
                        results.Add(AggregationResult.Removed(oldFirst!));
                        results.Add(AggregationResult.Added(value!, Enumerable.Repeat(fact, 1)));
                    }
                }
                else
                {
                    results.Add(AggregationResult.Added(value!, Enumerable.Repeat(fact, 1)));
                }
                _firstElements[fact] = value;
                break;
            }
        }
        return results;
    }

    public IReadOnlyCollection<AggregationResult> Remove(AggregationContext context, ITuple tuple, IReadOnlyCollection<IFact> facts)
    {
        var results = new List<AggregationResult>();
        foreach (var fact in facts)
        {
            if (_firstElements.TryGetValue(fact, out var oldFirst))
            {
                results.Add(AggregationResult.Removed(oldFirst!));
                _firstElements.Remove(fact);
            }
        }
        return results;
    }
}