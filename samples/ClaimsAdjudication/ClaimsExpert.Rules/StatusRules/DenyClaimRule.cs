using NRules.Fluent.Dsl;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules.StatusRules
{
    [Name("Deny claim")]
    public class DenyClaimRule : Rule
    {
        public override void Define()
        {
            Claim claim = null;

            When()
                .Claim(() => claim, c => c.Open)
                .Exists<ClaimAlert>(ce => ce.Claim == claim, ce => ce.Severity > 2);

            Then()
                .Do(ctx => Deny(claim))
                .Do(ctx => ctx.Update(claim));
        }

        private static ClaimStatus Deny(Claim claim)
        {
            return claim.Status = ClaimStatus.Denied;
        }
    }
}