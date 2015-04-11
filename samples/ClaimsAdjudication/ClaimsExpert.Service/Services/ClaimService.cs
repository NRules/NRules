using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using AutoMapper;
using NRules.Samples.ClaimsExpert.Contract;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Service.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, IncludeExceptionDetailInFaults = true)]
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepository;

        public ClaimService(IClaimRepository claimRepository)
        {
            _claimRepository = claimRepository;
        }

        public IEnumerable<ClaimDto> GetAll()
        {
            var claims = _claimRepository.GetAll().Select(Mapper.Map<ClaimDto>);
            return claims;
        }

        public ClaimDto GetById(long claimId)
        {
            var claim = _claimRepository.GetById(claimId);
            return Mapper.Map<ClaimDto>(claim);
        }
    }
}