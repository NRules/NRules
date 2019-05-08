using System;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class ForwardChainingTransitiveTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_FactInserted_EachRuleFires()
        {
            //Arrange
            var order = new FactType {Value = "Value1"};

            Session.Insert(order);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce<FactToCalc1Rule>();
            AssertFiredOnce<Calc1ToCalc2Rule>();
            AssertFiredOnce<Calc1Calc2ToCalc3Rule>();
        }

        [Fact]
        public void Fire_FactInsertedThenUpdated_EachRuleFiresTwice()
        {
            //Arrange
            var order = new FactType {Value = "Value1"};

            Session.Insert(order);
            Session.Fire();

            order.Value = "Value2";
            Session.Update(order);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice<FactToCalc1Rule>();
            AssertFiredTwice<Calc1ToCalc2Rule>();
            AssertFiredTwice<Calc1Calc2ToCalc3Rule>();
        }
        
        protected override void SetUpRules()
        {
            SetUpRule<FactToCalc1Rule>();
            SetUpRule<Calc1ToCalc2Rule>();
            SetUpRule<Calc1Calc2ToCalc3Rule>();
        }

        public class FactType
        {
            public string Value { get; set; }
        }

        public class Calc1
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class Calc2
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class Calc3 : IEquatable<Calc3>
        {
            public bool Equals(Calc3 other)
            {
                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Calc3) obj);
            }

            public override int GetHashCode()
            {
                return 1;
            }
        }

        public class FactToCalc1Rule : Rule
        {
            public override void Define()
            {
                FactType o = null;

                When()
                    .Match(() => o);

                Filter()
                    .OnChange(() => o.Value);

                Then()
                    .Yield(_ => new Calc1 {Key = o.Value});
            }
        }

        public class Calc1ToCalc2Rule : Rule
        {
            public override void Define()
            {
                Calc1 calc = null;

                When()
                    .Match(() => calc);

                Filter()
                    .OnChange(() => calc);

                Then()
                    .Yield(_ => new Calc2{Key = calc.Key});
            }
        }

        public class Calc1Calc2ToCalc3Rule : Rule
        {
            public override void Define()
            {
                Calc1 calc1 = null;
                Calc2 calc2 = null;

                When()
                    .Match(() => calc1)
                    .Match(() => calc2, c => c.Key.Equals(calc1.Key));

                Then()
                    .Yield(_ => new Calc3());
            }
        }
    }
}