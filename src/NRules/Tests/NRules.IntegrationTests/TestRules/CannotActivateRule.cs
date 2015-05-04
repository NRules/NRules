using System;
using NRules.Fluent.Dsl;

namespace NRules.IntegrationTests.TestRules
{
    public class CannotActivateRule : Rule
    {
        public CannotActivateRule()
        {
            throw new InvalidOperationException("Failed in ctor");
        }

        public override void Define()
        {
        }
    }
}