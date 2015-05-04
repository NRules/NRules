using System.ServiceModel;
using Common.Logging;
using NRules.Samples.ClaimsExpert.Contract;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Service.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, IncludeExceptionDetailInFaults = true)]
    public class AdjudicationService : IAdjudicationService
    {
        private static readonly ILog Log = LogManager.GetLogger<AdjudicationService>();

        private readonly ISessionFactory _sessionFactory;
        private readonly IClaimRepository _claimRepository;

        public AdjudicationService(ISessionFactory sessionFactory, IClaimRepository claimRepository)
        {
            _sessionFactory = sessionFactory;
            _claimRepository = claimRepository;
        }

        public void Adjudicate(long claimId)
        {
            var claim = _claimRepository.GetById(claimId);
            Log.InfoFormat("Adjudicating claim. ClaimId={0}", claimId);

            claim.Alerts.Clear();
            claim.Status = ClaimStatus.Open;

            ISession session = _sessionFactory.CreateSession();
            session.Insert(claim);
            session.Insert(claim.Patient);
            session.Insert(claim.Insured);
            session.Fire();

            var alerts = session.Query<ClaimAlert>();

            foreach (var alert in alerts)
            {
                claim.Alerts.Add(alert);
            }
            _claimRepository.Save(claim);
        }
    }
}