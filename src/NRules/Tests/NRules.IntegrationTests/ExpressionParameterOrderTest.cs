using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class ExpressionParameterOrderTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_MatchGroup1_FiresOnce()
        {
            //Arrange
            var f0 = new FactType0();
            var f1 = new FactType1();
            var f2 = new FactType2();
            Session.InsertAll(new object[]{f0, f1, f2});

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_MatchGroup2_FiresOnce()
        {
            //Arrange
            var f0 = new FactType0();
            var f3 = new FactType3();
            Session.InsertAll(new object[]{f0, f3});

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType0
        {
        }

        public class FactType1
        {
        }

        public class FactType2
        {
        }

        public class FactType3
        {
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType0 f0 = null;
                FactType1 f1 = null;
                FactType2 f2 = null;
                FactType3 f3 = null;

                When()
                    .Or(x => x
                        .And(xx => xx
                            .Match(() => f0)
                            .Match(() => f1)
                            .Match(() => f2)
                            .Having(() => Condition(f0, f1, f2))
                            .Having(() => Condition(f2, f3, f0)))
                        .And(xx => xx
                            .Match(() => f3)
                            .Match(() => f0)
                            .Having(() => Condition(f0, f3))
                            .Having(() => Condition(f3, f0)))
                    );

                Then()
                    .Do(ctx => Action(f2, f0, f3));
            }

            private bool Condition(FactType0 f0, FactType1 f1, FactType2 f2)
            {
                return true;
            }

            private bool Condition(FactType2 f2, FactType3 f3, FactType0 f0)
            {
                return true;
            }

            private bool Condition(FactType0 f0, FactType3 f3)
            {
                return true;
            }

            private bool Condition(FactType3 f3, FactType0 f0)
            {
                return true;
            }

            private void Action(FactType2 f2, FactType0 f0, FactType3 f3)
            {
            }
        }
    }
}