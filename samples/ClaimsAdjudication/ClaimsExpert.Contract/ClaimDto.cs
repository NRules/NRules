using System.Runtime.Serialization;

namespace NRules.Samples.ClaimsExpert.Contract
{
    [DataContract]
    public class ClaimDto
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string ClaimType { get; set; }
        [DataMember]
        public AdjudicationStatus Status { get; set; }
        [DataMember]
        public string PatientFirstName { get; set; }
        [DataMember]
        public string PatientLastName { get; set; }
        [DataMember]
        public string PatientMiddleName { get; set; }
        [DataMember]
        public string PatientAddressLine1 { get; set; }
        [DataMember]
        public string PatientAddressLine2 { get; set; }
        [DataMember]
        public string PatientAddressCity { get; set; }
        [DataMember]
        public string PatientAddressState { get; set; }
        [DataMember]
        public string PatientAddressZip { get; set; }
        [DataMember]
        public ClaimAlertDto[] Alerts { get; set; }
    }
}