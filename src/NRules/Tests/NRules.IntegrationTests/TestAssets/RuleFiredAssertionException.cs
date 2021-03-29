using Xunit.Sdk;

namespace NRules.IntegrationTests.TestAssets
{
    public class RuleFiredAssertionException : AssertActualExpectedException
    {
        public RuleFiredAssertionException(object expected, object actual, string ruleName) 
            : base(expected, actual, $"Rule {ruleName}", "Expected fired times", "Actual fired times")
        {
        }
    }
}