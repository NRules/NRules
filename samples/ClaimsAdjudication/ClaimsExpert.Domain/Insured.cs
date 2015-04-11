using System;

namespace NRules.Samples.ClaimsExpert.Domain
{
    public class Insured
    {
        public virtual Name Name { get; set; }
        public virtual Address Address { get; set; }
        public virtual string Phone { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual Sex Sex { get; set; }
        public virtual string EmployerName { get; set; }
        public virtual string PolicyNumber { get; set; }
        public virtual string PlanName { get; set; }
    }
}