using Xunit.Sdk;

namespace NRules.IntegrationTests.TestAssets;

public class RuleFiredAssertionException : XunitException
{
    public RuleFiredAssertionException(object expected, object actual, string ruleName) 
        : base($"Rule {ruleName}: Expected fired times {expected}. Actual fired times {actual}")
    {
    }
}