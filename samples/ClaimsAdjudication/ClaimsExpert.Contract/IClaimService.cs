using System.Collections.Generic;
using System.ServiceModel;

namespace NRules.Samples.ClaimsExpert.Contract
{
    [ServiceContract]
    public interface IClaimService
    {
        [OperationContract]
        IEnumerable<ClaimDto> GetAll();
        [OperationContract]
        ClaimDto GetById(long claimId);
    }
}