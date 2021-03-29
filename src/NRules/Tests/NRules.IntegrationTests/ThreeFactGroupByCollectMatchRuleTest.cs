using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class ThreeFactGroupByCollectMatchRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_AllPatternsMatch_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1{Key = "key1", Join = "join1"};
            var fact2 = new FactType2{Join = "join1"};
            var fact3 = new FactType3{Join = "join1"};
            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Fact]
        public void Fire_AllPatternsMatchThenFirstFactRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1{Key = "key1", Join = "join1"};
            var fact2 = new FactType2{Join = "join1"};
            var fact3 = new FactType3{Join = "join1"};
            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Retract(fact1);
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }
        
        [Fact]
        public void Fire_AllPatternsMatchThenSecondFactRetracted_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1{Key = "key1", Join = "join1"};
            var fact2 = new FactType2{Join = "join1"};
            var fact3 = new FactType3{Join = "join1"};
            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Retract(fact2);
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_AllPatternsMatchThenThirdFactRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1{Key = "key1", Join = "join1"};
            var fact2 = new FactType2{Join = "join1"};
            var fact3 = new FactType3{Join = "join1"};
            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Retract(fact3);
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType1
        {
            public string Key { get; set; }
            public string Join { get; set; }
        }

        public class FactType2
        {
            public string Join { get; set; }
        }

        public class FactType3
        {
            public string Join { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                IGrouping<string, FactType1> group = default;
                IEnumerable<FactType2> facts2 = default;
                FactType3 fact3 = default;

                When()
                    .Query(() => group, x => x
                        .Match<FactType1>()
                        .GroupBy(f => f.Key))
                    .Query(() => facts2, q => q
                        .Match<FactType2>(f => f.Join == group.First().Join)
                        .Collect())
                    .Match<FactType3>(() => fact3, f => f.Join == group.First().Join);
                
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}