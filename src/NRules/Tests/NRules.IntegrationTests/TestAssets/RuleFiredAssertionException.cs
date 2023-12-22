using Xunit.Sdk;

namespace NRules.IntegrationTests.TestAssets;

public class RuleFiredAssertionException : XunitException
{
    public RuleFiredAssertionException(string message) 
        : base(message)
    {
    }
}