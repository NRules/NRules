using NRules.Fluent.Dsl;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules.StatusRules
{
    [Name("Review claim")]
    public class ReviewClaimRule : Rule
    {
        public override void Define()
        {
            Claim claim = null;

            When()
                .Claim(() => claim, c => c.Open)
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