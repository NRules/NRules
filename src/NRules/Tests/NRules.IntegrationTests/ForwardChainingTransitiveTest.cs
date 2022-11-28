using System;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class ForwardChainingTransitiveTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_FactInserted_EachRuleFires()
    {
        //Arrange
        var order = new FactType { Value = "Value1" };

        Session.Insert(order);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule<FactToCalc1Rule>().FiredTimes(1);
        Verify.Rule<Calc1ToCalc2Rule>().FiredTimes(1);
        Verify.Rule<Calc1Calc2ToCalc3Rule>().FiredTimes(1);
    }

    [Fact]
    public void Fire_FactInsertedThenUpdated_EachRuleFiresTwice()
    {
        //Arrange
        var order = new FactType { Value = "Value1" };

        Session.Insert(order);
        Session.Fire();

        order.Value = "Value2";
        Session.Update(order);

        //Act
        Session.Fire();

        //Assert
        Verify.Rule<FactToCalc1Rule>().FiredTimes(2);
        Verify.Rule<Calc1ToCalc2Rule>().FiredTimes(2);
        Verify.Rule<Calc1Calc2ToCalc3Rule>().FiredTimes(2);
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<FactToCalc1Rule>();
        setup.Rule<Calc1ToCalc2Rule>();
        setup.Rule<Calc1Calc2ToCalc3Rule>();
    }

    public class FactType
    {
        public string? Value { get; set; }
    }

    public class Calc1
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }

    public class Calc2
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }

    public class Calc3 : IEquatable<Calc3>
    {
        public bool Equals(Calc3? other)
        {
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Calc3)obj);
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
            FactType? o = null;

            When()
                .Match(() => o);

            Filter()
                .OnChange(() => o!.Value!);

            Then()
                .Yield(_ => new Calc1 { Key = o!.Value });
        }
    }

    public class Calc1ToCalc2Rule : Rule
    {
        public override void Define()
        {
            Calc1? calc = null;

            When()
                .Match(() => calc);

            Filter()
                .OnChange(() => calc!);

            Then()
                .Yield(_ => new Calc2 { Key = calc!.Key });
        }
    }

    public class Calc1Calc2ToCalc3Rule : Rule
    {
        public override void Define()
        {
            Calc1? calc1 = null;
            Calc2? calc2 = null;

            When()
                .Match(() => calc1)
                .Match(() => calc2, c => c!.Key!.Equals(calc1!.Key));

            Then()
                .Yield(_ => new Calc3());
        }
    }
}