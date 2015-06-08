using System;
using Common.Logging;
using NRules.Samples.ClaimsExpert.Domain;
using NRules.Samples.ClaimsExpert.Rules;

namespace NRules.Samples.ClaimsExpert.Service.Services
{
    public class NotificationService : INotificationService
    {
        private static readonly ILog Log = LogManager.GetLogger<ServiceController>();

        public void ClaimDenied(Claim claim)
        {
            Log.WarnFormat("Notification, claim denied. ClaimId={0}", claim.Id);
        }
    }
}