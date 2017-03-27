using System;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.IntegrationTests.TestAssets
{
    public abstract class BaseRule : Rule
    {
        protected BaseRule()
        {
            Action = ctx => { };
        }

        public Action<IContext> Action { get; set; }
    }
}