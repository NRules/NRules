using System.ServiceModel;

namespace NRules.Samples.ClaimsExpert.Contract
{
    [ServiceContract]
    public interface IAdjudicationService
    {
        [OperationContract]
        void Adjudicate(long claimId);
    }
}