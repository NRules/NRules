using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.Aggregators;
using NRules.Fluent.Dsl;
using NRules.Fluent.Expressions;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests.CustomAggregator
{
    public class OneFactTwoExpressionAggregatorTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_NoMatchingFacts_DoesNotFire()
        {
            //Arrange - Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndOneForAnother_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

            var facts = new[] {fact1, fact2, fact3};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnother_FiresTwiceWithTwoFactsInEachGroup()
        {
            //Arrange
            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
            var fact4 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>(0).Count());
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>(1).Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneRetracted_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
            var fact4 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            Session.Retract(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToInvalid_FiresOnceWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
            var fact4 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact4.TestProperty = "Invalid Value";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndTwoForAnotherOneUpdatedToFirstGroup_FiresOnceWithThreeFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
            var fact4 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact4.GroupProperty = 1;
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            var actual = GetFiredFact<IGrouping<long, string>>().Count();
            Assert.Equal(3, actual);
        }

        [Fact]
        public void Fire_TwoFactsForOneGroupAndOneForAnotherAndOneInvalidTheInvalidUpdatedToSecondGroup_FiresTwiceWithTwoFactsInEachGroup()
        {
            //Arrange
            var fact1 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact2 = new FactType {GroupProperty = 1, TestProperty = "Valid Value"};
            var fact3 = new FactType {GroupProperty = 2, TestProperty = "Valid Value"};
            var fact4 = new FactType {GroupProperty = 2, TestProperty = "Invalid Value"};

            var facts = new[] {fact1, fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact4.TestProperty = "Valid Value";
            Session.Update(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>(0).Count());
            Assert.Equal(2, GetFiredFact<IGrouping<long, string>>(1).Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public long GroupProperty { get; set; }
            public string TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                IEnumerable<string> group = null;

                When()
                    .Query(() => group, x => x
                        .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                        .MimicGroupBy(f => f.GroupProperty, f => f.TestProperty)
                        .Where(g => g.Count() > 1));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }

    public static class GroupByQuery
    {
        public static IQuery<IGrouping<TKey, TElement>> MimicGroupBy<TSource, TKey, TElement>(this IQuery<TSource> source, Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TElement>> elementSelector)
        {
            var expressionMap = new Dictionary<string, LambdaExpression>
            {
                {"KeySelector", keySelector},
                {"ElementSelector", elementSelector}
            };
            source.Builder.Aggregate<TSource, IGrouping<TKey, TElement>>("GroupByMimicry", expressionMap, typeof(MimicGroupByAggregateFactory));
            return new QueryExpression<IGrouping<TKey, TElement>>(source.Builder);
        }
    }

    internal class MimicGroupByAggregateFactory : IAggregatorFactory
    {
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IDictionary<string, IAggregateExpression> compiledExpressions)
        {
            var keySelector = element.ExpressionMap["KeySelector"];
            var elementSelector = element.ExpressionMap["ElementSelector"];

            var sourceType = element.Source.ValueType;
            var keyType = keySelector.Expression.ReturnType;
            var elementType = elementSelector.Expression.ReturnType;
            Type aggregatorType = typeof(MimicGroupByAggregator<,,>).MakeGenericType(sourceType, keyType, elementType);

            var compiledKeySelector = compiledExpressions["KeySelector"];
            var compiledElementSelector = compiledExpressions["ElementSelector"];
            var ctor = aggregatorType.GetTypeInfo().DeclaredConstructors.Single();
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(ctor, Expression.Constant(compiledKeySelector), Expression.Constant(compiledElementSelector)));
            _factory = factoryExpression.Compile();
        }

        public IAggregator Create()
        {
            return _factory();
        }
    }

    public class MimicGroupByAggregator<TSource, TKey, TElement> : IAggregator
    {
        private readonly IAggregateExpression _keySelector;
        private readonly IAggregateExpression _elementSelector;

        private readonly Dictionary<object, TKey> _sourceToKey = new Dictionary<object, TKey>();
        private readonly Dictionary<object, TElement> _sourceToElement = new Dictionary<object, TElement>();

        private readonly DefaultKeyMap<TKey, Grouping> _groups = new DefaultKeyMap<TKey, Grouping>();

        public MimicGroupByAggregator(IAggregateExpression keySelector, IAggregateExpression elementSelector)
        {
            _keySelector = keySelector;
            _elementSelector = elementSelector;
        }

        public IEnumerable<AggregationResult> Add(ITuple tuple, IEnumerable<IFact> facts)
        {
            var keys = new List<TKey>();
            var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var key = (TKey)_keySelector.Invoke(tuple, fact);
                var element = (TElement)_elementSelector.Invoke(tuple, fact);
                _sourceToKey[source] = key;
                _sourceToElement[source] = element;
                var result = Add(key, element);
                if (!resultLookup.ContainsKey(key))
                {
                    keys.Add(key);
                    resultLookup[key] = result;
                }
            }
            var results = GetResults(keys, resultLookup);
            return results;
        }

        public IEnumerable<AggregationResult> Modify(ITuple tuple, IEnumerable<IFact> facts)
        {
            var keys = new List<TKey>();
            var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var key = (TKey)_keySelector.Invoke(tuple, fact);
                var element = (TElement)_elementSelector.Invoke(tuple, fact);
                var oldKey = _sourceToKey[source];
                var oldElement = _sourceToElement[source];
                _sourceToKey[source] = key;
                _sourceToElement[source] = element;

                if (Equals(key, oldKey))
                {
                    var result = Modify(key, oldElement, element);
                    if (!resultLookup.ContainsKey(key))
                    {
                        keys.Add(key);
                        resultLookup[key] = result;
                    }
                }
                else
                {
                    var result1 = Remove(oldKey, oldElement);
                    if (!resultLookup.ContainsKey(oldKey))
                    {
                        keys.Add(oldKey);
                    }
                    resultLookup[oldKey] = result1;

                    var result2 = Add(key, element);
                    AggregationResult previousResult;
                    if (!resultLookup.TryGetValue(key, out previousResult))
                    {
                        keys.Add(key);
                        resultLookup[key] = result2;
                    }
                    else if (previousResult.Action == AggregationAction.Removed ||
                             result2.Action == AggregationAction.Added)
                    {
                        resultLookup[key] = AggregationResult.Modified(previousResult.Aggregate);
                    }
                }
            }
            var results = GetResults(keys, resultLookup);
            return results;
        }

        public IEnumerable<AggregationResult> Remove(ITuple tuple, IEnumerable<IFact> facts)
        {
            var keys = new List<TKey>();
            var resultLookup = new DefaultKeyMap<TKey, AggregationResult>();
            foreach (var fact in facts)
            {
                var source = (TSource)fact.Value;
                var oldKey = _sourceToKey[source];
                var oldElement = _sourceToElement[source];
                _sourceToKey.Remove(source);
                _sourceToElement.Remove(source);
                var result = Remove(oldKey, oldElement);
                if (!resultLookup.ContainsKey(oldKey))
                {
                    keys.Add(oldKey);
                }
                resultLookup[oldKey] = result;
            }
            var results = GetResults(keys, resultLookup);
            return results;
        }

        private AggregationResult Add(TKey key, TElement element)
        {
            Grouping group;
            if (!_groups.TryGetValue(key, out group))
            {
                group = new Grouping(key);
                _groups[key] = group;

                group.Add(element);
                return AggregationResult.Added(group);
            }

            group.Add(element);
            return AggregationResult.Modified(group);
        }

        private AggregationResult Modify(TKey key, TElement oldElement, TElement element)
        {
            var group = _groups[key];
            if (Equals(oldElement, element))
            {
                group.Modify(element);
            }
            else
            {
                group.Remove(oldElement);
                group.Add(element);
            }
            return AggregationResult.Modified(group);
        }

        private AggregationResult Remove(TKey key, TElement element)
        {
            var group = _groups[key];
            group.Remove(element);
            if (group.Count == 0)
            {
                _groups.Remove(key);
                return AggregationResult.Removed(group);
            }
            return AggregationResult.Modified(group);
        }

        private static IEnumerable<AggregationResult> GetResults(IEnumerable<TKey> keys, DefaultKeyMap<TKey, AggregationResult> lookup)
        {
            var results = new List<AggregationResult>();
            foreach (var key in keys)
            {
                var result = lookup[key];
                results.Add(result);
            }
            return results;
        }

        public IEnumerable<object> Aggregates => _groups.Values;

        private class Grouping : FactCollection<TElement>, IGrouping<TKey, TElement>
        {
            public Grouping(TKey key)
            {
                Key = key;
            }

            public TKey Key { get; }
        }
    }
}