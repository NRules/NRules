using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules
{
    public static class DslExtensions
    {
        public static ILeftHandSide Claim(this ILeftHandSide lhs, Expression<Func<Claim>> alias, params Expression<Func<Claim, bool>>[] conditions)
        {
            return lhs.Match(alias, conditions);
        }

        public static ILeftHandSide Patient(this ILeftHandSide lhs, Expression<Func<Patient>> alias, params Expression<Func<Patient, bool>>[] conditions)
        {
            return lhs.Match(alias, conditions);
        }

        public static ILeftHandSide Patient(this ILeftHandSide lhs, Expression<Func<Patient, bool>> condition, params Expression<Func<Patient, bool>>[] conditions)
        {
            return lhs.Match(condition, conditions);
        }
 
        public static ILeftHandSide Insured(this ILeftHandSide lhs, Expression<Func<Insured>> alias, params Expression<Func<Insured, bool>>[] conditions)
        {
            return lhs.Match(alias, conditions);
        }

        public static ILeftHandSide Insured(this ILeftHandSide lhs, Expression<Func<Insured, bool>> condition, params Expression<Func<Insured, bool>>[] conditions)
        {
            return lhs.Match(condition, conditions);
        }
    }
}