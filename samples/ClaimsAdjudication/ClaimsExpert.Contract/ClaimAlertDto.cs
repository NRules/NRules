using System.Runtime.Serialization;

namespace NRules.Samples.ClaimsExpert.Contract
{
    [DataContract]
    public class ClaimAlertDto
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string RuleName { get; set; }
    }
}