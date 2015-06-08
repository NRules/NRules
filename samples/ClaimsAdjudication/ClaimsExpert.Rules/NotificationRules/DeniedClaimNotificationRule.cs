using NRules.Fluent.Dsl;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules.NotificationRules
{
    [Name("Notify claim denied")]
    public class DeniedClaimNotificationRule : Rule
    {
        public override void Define()
        {
            INotificationService service = null;
            Claim claim = null;

            Dependency()
                .Resolve(() => service);

            When()
                .Match(() => claim, c => c.Status == ClaimStatus.Denied);

            Then()
                .Do(_ => service.ClaimDenied(claim));
        }
    }
}
