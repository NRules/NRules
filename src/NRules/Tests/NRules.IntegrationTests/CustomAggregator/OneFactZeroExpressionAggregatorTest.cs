using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Aggregators;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests.CustomAggregator
{
    public class OneFactZeroExpressionAggregatorTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_NoMatchingFacts_FiresOnceWithEmptyCollection()
        {
            //Arrange - Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(0, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsAndOneInvalid_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new FactType { TestProperty = "Valid Value 1" };
            var fact2 = new FactType { TestProperty = "Valid Value 2" };
            var fact3 = new FactType { TestProperty = "Invalid Value 3" };

            var facts = new[] { fact1, fact2, fact3 };
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneUpdated_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new FactType { TestProperty = "Valid Value 1" };
            var fact2 = new FactType { TestProperty = "Valid Value 2" };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneRetracted_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new FactType { TestProperty = "Valid Value 1" };
            var fact2 = new FactType { TestProperty = "Valid Value 2" };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(1, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedTwoRetracted_FiresOnceWithEmptyCollection()
        {
            //Arrange
            var fact1 = new FactType { TestProperty = "Valid Value 1" };
            var fact2 = new FactType { TestProperty = "Valid Value 2" };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);
            Session.Retract(fact1);
            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(0, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneUpdatedToInvalid_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new FactType { TestProperty = "Valid Value 1" };
            var fact2 = new FactType { TestProperty = "Valid Value 2" };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);

            fact2.TestProperty = "Invalid Value";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(1, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_OneMatchingFactsAndOneInvalidInsertedTheInvalidUpdatedToValid_FiresOnceWithTwoFactInCollection()
        {
            //Arrange
            var fact1 = new FactType { TestProperty = "Valid Value 1" };
            var fact2 = new FactType { TestProperty = "Invalid Value" };

            var facts = new[] { fact1, fact2 };
            Session.InsertAll(facts);

            fact2.TestProperty = "Valid Value 2";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public string TestProperty { get; set; }
        }

        public class FactProjection : IEquatable<FactProjection>
        {
            public FactProjection(FactType fact)
            {
                Value = fact.TestProperty;
            }

            public string Value { get; }

            public bool Equals(FactProjection other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Value, other.Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((FactProjection)obj);
            }

            public override int GetHashCode()
            {
                return (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                IEnumerable<FactType> collection = null;

                When()
                    .Query(() => collection, x => x
                        .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                        .MimicCollect());
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }

    public static class CollectQuery
    {
        public static IQuery<IEnumerable<TSource>> MimicCollect<TSource>(this IQuery<TSource> source)
        {
            var expressionMap = new Dictionary<string, LambdaExpression>();
            source.Builder.Aggregate<TSource, IEnumerable<TSource>>("CollectMimicry", expressionMap, typeof(MimicCollectAggregateFactory));
            return new QueryExpression<IEnumerable<TSource>>(source.Builder);
        }
    }

    internal class MimicCollectAggregateFactory : IAggregatorFactory
    {
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IDictionary<string, IAggregateExpression> compiledExpressions)
        {
            var sourceType = element.Source.ValueType;
            var aggregatorType = typeof(MimicCollectAggregator<>).MakeGenericType(sourceType);
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(Expression.New(aggregatorType));
            _factory = factoryExpression.Compile();
        }

        public IAggregator Create()
        {
            return _factory();
        }
    }

    public class MimicCollectAggregator<TElement> : IAggregator
    {
        private readonly FactCollection<TElement> _items = new FactCollection<TElement>();
        private readonly object[] _container;
        private bool _created = false;

        public MimicCollectAggregator()
        {
            _container = new object[] { _items };
        }

        public IEnumerable<AggregationResult> Add(ITuple tuple, IEnumerable<IFact> facts)
        {
            AddFacts(facts);
            if (!_created)
            {
                _created = true;
                return new[] { AggregationResult.Added(_items) };
            }
            return new[] { AggregationResult.Modified(_items) };
        }

        public IEnumerable<AggregationResult> Modify(ITuple tuple, IEnumerable<IFact> facts)
        {
            ModifyFacts(facts);
            return new[] { AggregationResult.Modified(_items) };
        }

        public IEnumerable<AggregationResult> Remove(ITuple tuple, IEnumerable<IFact> facts)
        {
            RemoveFacts(facts);
            return new[] { AggregationResult.Modified(_items) };
        }

        private void AddFacts(IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                var item = (TElement)fact.Value;
                _items.Add(item);
            }
        }

        private void ModifyFacts(IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                var item = (TElement)fact.Value;
                _items.Modify(item);
            }
        }

        private void RemoveFacts(IEnumerable<IFact> facts)
        {
            foreach (var fact in facts)
            {
                var item = (TElement)fact.Value;
                _items.Remove(item);
            }
        }

        public IEnumerable<object> Aggregates => _container;
    }
}