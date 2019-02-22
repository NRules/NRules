using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.Aggregators;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class CustomSelectAggregatorTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_OneMatchingFact_FiresOnce()
        {
            //Arrange
            var fact = new FactType { TestProperty = "Valid Value 1" };
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(fact.TestProperty, GetFiredFact<FactProjection>().Value);
        }

        [Fact]
        public void Fire_OneMatchingFactInsertedThenUpdated_FiresOnce()
        {
            //Arrange
            var fact = new FactType { TestProperty = "Valid Value 1" };
            Session.Insert(fact);
            Session.Update(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(fact.TestProperty, GetFiredFact<FactProjection>().Value);
        }

        [Fact]
        public void Fire_OneMatchingFactAssertedAndRetracted_DoesNotFire()
        {
            //Arrange
            var fact = new FactType { TestProperty = "Valid Value 1" };
            Session.Insert(fact);
            Session.Retract(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        protected override ISessionFactory Compile()
        {
            var compiler = new RuleCompiler();
            compiler.AggregatorRegistry.RegisterFactory("CustomSelect", typeof(CustomSelectAggregateFactory));

            var factory = compiler.Compile(Repository.GetRules());
            return factory;
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
                FactProjection projection = null;

                When()
                    .Query(() => projection, q => q
                        .Match<FactType>()
                        .CustomSelect(f => new FactProjection(f))
                        .Where(p => p.Value.StartsWith("Valid")));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }

    public static class CustomSelectQuery
    {
        public static IQuery<TResult> CustomSelect<TSource, TResult>(this IQuery<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            var expressions = new List<KeyValuePair<string, LambdaExpression>>
            {
                new KeyValuePair<string, LambdaExpression>("Selector", selector)
            };
            source.Builder.Aggregate<TSource, TResult>("CustomSelect", expressions);
            return new QueryExpression<TResult>(source.Builder);
        }
    }

    internal class CustomSelectAggregateFactory : IAggregatorFactory
    {
        private Func<IAggregator> _factory;

        public void Compile(AggregateElement element, IEnumerable<IAggregateExpression> compiledExpressions)
        {
            var selector = element.Expressions["Selector"];
            var sourceType = element.Source.ValueType;
            var resultType = selector.Expression.ReturnType;
            var aggregatorType = typeof(CustomSelectAggregator<,>).MakeGenericType(sourceType, resultType);

            var compiledSelector = compiledExpressions.FindSingle("Selector");
            var ctor = aggregatorType.GetTypeInfo().DeclaredConstructors.Single();
            var factoryExpression = Expression.Lambda<Func<IAggregator>>(
                Expression.New(ctor, Expression.Constant(compiledSelector)));
            _factory = factoryExpression.Compile();
        }

        public IAggregator Create()
        {
            return _factory();
        }
    }

    public class CustomSelectAggregator<TSource, TResult> : IAggregator
    {
        private readonly IAggregateExpression _selector;
        private readonly Dictionary<IFact, object> _sourceToValue = new Dictionary<IFact, object>();

        public CustomSelectAggregator(IAggregateExpression selector)
        {
            _selector = selector;
        }

        public IEnumerable<AggregationResult> Add(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var value = _selector.Invoke(context, tuple, fact);
                _sourceToValue[fact] = value;
                results.Add(AggregationResult.Added(value, Enumerable.Repeat(fact, 1)));
            }
            return results;
        }

        public IEnumerable<AggregationResult> Modify(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var value = _selector.Invoke(context, tuple, fact);
                var oldValue = (TResult)_sourceToValue[fact];
                _sourceToValue[fact] = value;
                results.Add(AggregationResult.Modified(value, oldValue, Enumerable.Repeat(fact, 1)));
            }
            return results;
        }

        public IEnumerable<AggregationResult> Remove(AggregationContext context, ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var oldValue = _sourceToValue[fact];
                _sourceToValue.Remove(fact);
                results.Add(AggregationResult.Removed(oldValue));
            }
            return results;
        }
    }
}