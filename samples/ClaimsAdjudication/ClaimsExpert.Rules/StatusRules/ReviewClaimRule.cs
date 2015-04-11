using NRules.Fluent.Dsl;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules.StatusRules
{
    [Name("Review claim")]
    [Priority(1000)]
    public class ReviewClaimRule : Rule
    {
        public override void Define()
        {
            Claim claim = null;

            When()
                .Match<Claim>(() => claim, c => c.Status == ClaimStatus.Open)
                .Exists<ClaimAlert>(ce => ce.Claim == claim, ce => ce.Severity == 2);

            Then()
                .Do(ctx => Review(claim));
        }

        private static ClaimStatus Review(Claim claim)
        {
            return claim.Status = ClaimStatus.Review;
        }
    }
}