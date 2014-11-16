using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRules.Fluent.Dsl;

namespace NRules.IntegrationTests.TestAssets
{
    [TagAttribute("ParentTag"), TagAttribute("ParentMetadata")]
    public abstract class BaseRuleWithMetadata : Rule
    {
        public INotifier Notifier { get; set; }
    }
}
