using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;

namespace NRules.IntegrationTests.TestRules
{
    public class MultipleQueriesSingleJoinRule : BaseRule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            IEnumerable<FactType2> collection2 = null;
            IEnumerable<FactType3> collection3 = null;
            IEnumerable<FactType4> collection4 = null;
            
            When()
                .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Query(() => collection2, q => q
                    .Match<FactType2>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect())
                .Query(() => collection3, q => q
                    .Match<FactType3>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect())
                .Query(() => collection4, q => q
                    .Match<FactType4>(f => f.TestProperty.StartsWith("Valid"))
                    .Collect()
                    .Where(x => IsMatch(fact1, collection2, collection3, x)));
            Then()
                .Do(ctx => Action(ctx));
        }

        private bool IsMatch(FactType1 fact1, IEnumerable<FactType2> collection2, IEnumerable<FactType3> collection3, IEnumerable<FactType4> collection4)
        {
            return collection4.Any(x => x.TestProperty == fact1.TestProperty);
        }
    }
}