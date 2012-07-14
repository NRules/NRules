using System;
using System.Linq;
using NRules.Core.IntegrationTests.Domain;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.Rules
{
    //an applicant has had self beneficial policies
    public class SelfBeneficialPolicyRule : IRule
    {
        private readonly EventHandler _ruleHandler;

        public SelfBeneficialPolicyRule(EventHandler ruleHandler)
        {
            _ruleHandler = ruleHandler;
        }

        //bug: Figure out why i can't make this work via joins.
        //either there is a bug in the code or (more likely) i just screwed it up.
        public void Define(IRuleDefinition definition)
        {
            definition.When()
                //.If<InsuranceApplicant>(applicant => applicant.InsuranceHistory.Policies.Where(policy => policy.Beneficiary.TheBeneficiary == applicant.Applicant).Any())
                .If<InsuranceApplicant, InsuranceHistory>((applicant, history) => applicant.InsuranceHistory == history)
                .If<InsuranceHistory, InsurancePolicy>((history, policy) => history.Policies.Contains(policy))
                .If<InsurancePolicy, Beneficiary>((policy, beneficiary) => policy.Beneficiary == beneficiary)
                .If<Beneficiary, InsuranceApplicant>(
                    (beneficiary, applicant) => beneficiary.TheBeneficiary == applicant.Applicant);

            definition.Then()
                .Do(ctx =>
                        {
                            Console.WriteLine("Found a self beneficial policy!");
                            var applicant = ctx.Arg<InsuranceApplicant>();
                            _ruleHandler(this, new QualificationEventArgs(applicant));
                        });
        }
    }
}