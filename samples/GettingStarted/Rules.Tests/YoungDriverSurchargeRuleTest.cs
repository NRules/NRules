using Domain;
using NRules.Testing;
using Xunit;

namespace Rules.Tests;

public class YoungDriverSurchargeRuleTest : RulesTestFixture
{
    [Fact]
    public void Fire_QuoteWithDriverAt25_DoesNotFire()
    {
        // Arrange
        var driver = new Driver("John Do", 25, 6);
        var quote = new InsuranceQuote(driver, 1000);

        // Act
        Session.Insert(quote);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
    
    [Fact]
    public void Fire_QuoteWithDriverUnder25_Fires()
    {
        // Arrange
        var driver = new Driver("John Do", 24, 6);
        var quote = new InsuranceQuote(driver, 1000);

        // Act
        Session.Insert(quote);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Matched.Fact(quote)));
        Assert.Equal(1100, quote.FinalPremium);
    }
    
    public YoungDriverSurchargeRuleTest()
    {
        Setup.Rule<YoungDriverSurchargeRule>();
    }
}