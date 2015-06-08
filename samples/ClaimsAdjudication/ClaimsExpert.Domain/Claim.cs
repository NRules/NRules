using System.Collections.Generic;

namespace NRules.Samples.ClaimsExpert.Domain
{
    public class Claim
    {
        public virtual long Id { get; set; }
        public virtual ClaimType ClaimType { get; set; }
        public virtual ClaimStatus Status { get; set; }
        public virtual IList<ClaimAlert> Alerts { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Insured Insured { get; set; }

        public virtual bool Open { get { return Status == ClaimStatus.Open; } }
    }
}