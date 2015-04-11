namespace NRules.Samples.ClaimsExpert.Domain
{
    public class ClaimAlert
    {
        public virtual long Id { get; set; }
        public virtual Claim Claim { get; set; }
        public virtual int Severity { get; set; }
        public virtual string RuleName { get; set; }
        public virtual string Message { get; set; }
    }
}