using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules
{
    public interface INotificationService
    {
        void ClaimDenied(Claim claim);
    }
}