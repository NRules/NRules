using System;
using NRules.Core.IntegrationTests.Domain;
using NRules.Dsl;

namespace NRules.Core.IntegrationTests.Rules
{
    //Applicant is likely to commit insurance fraud.
    public class SimplePersonalFinancesRule : IRule
    {
        private readonly EventHandler _ruleHandler;

        public SimplePersonalFinancesRule(EventHandler ruleHandler)
        {
            _ruleHandler = ruleHandler;
        }

        public void Define(IRuleDefinition definition)
        {
            definition.When()
                .If<InsuranceApplicant>(applicant => applicant.Applicant.Age >= 21)
                .If<PersonalFinances>(finances => finances.LiquidAssets + finances.YearlyPreTaxIncome >= finances.TotalDebt * 0.6m)
                .If<InsuranceApplicant, PersonalFinances>((applicant, finances) => applicant.PersonalFinances == finances);

            definition.Then()
                .Do(ctx =>
                        {
                            var applicant = ctx.Arg<InsuranceApplicant>();
                            var finances = ctx.Arg<PersonalFinances>();
                            Console.WriteLine(applicant.Applicant.Age);
                            Console.WriteLine(finances.LiquidAssets + finances.YearlyPreTaxIncome);
                            Console.WriteLine(finances.TotalDebt * 0.6m);
                            _ruleHandler(this, new QualificationEventArgs(applicant));
                        });
        }
    }
}
