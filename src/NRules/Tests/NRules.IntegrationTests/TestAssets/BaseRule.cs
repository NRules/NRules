using System;
using NRules.Fluent.Dsl;

namespace NRules.IntegrationTests.TestAssets
{
    public abstract class BaseRule : Rule
    {
        protected BaseRule()
        {
            Action = () => { };
        }

        public Action Action { get; set; }
    }
}