using System;

namespace NRules.Samples.ClaimsExpert.Domain
{
    public class Patient
    {
        public virtual Name Name { get; set; }
        public virtual Address Address { get; set; }
        public virtual string Phone { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual Sex Sex { get; set; }
        public virtual Relationship RelationshipToInsured { get; set; }
        public virtual MaritalStatus MaritalStatus { get; set; }
        public virtual EmploymentStatus EmploymentStatus { get; set; }
    }
}