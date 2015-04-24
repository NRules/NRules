using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.Samples.ClaimsExpert.Domain;

namespace NRules.Samples.ClaimsExpert.Rules
{
    public static class DslExtensions
    {
        public static ILeftHandSideExpression Claim(this ILeftHandSideExpression lhs, Expression<Func<Claim>> alias, params Expression<Func<Claim, bool>>[] conditions)
        {
            return lhs.Match(alias, conditions);
        }

        public static ILeftHandSideExpression Patient(this ILeftHandSideExpression lhs, Expression<Func<Patient>> alias, params Expression<Func<Patient, bool>>[] conditions)
        {
            return lhs.Match(alias, conditions);
        }

        public static ILeftHandSideExpression Patient(this ILeftHandSideExpression lhs, params Expression<Func<Patient, bool>>[] conditions)
        {
            return lhs.Match(conditions);
        }

        public static ILeftHandSideExpression Insured(this ILeftHandSideExpression lhs, Expression<Func<Insured>> alias, params Expression<Func<Insured, bool>>[] conditions)
        {
            return lhs.Match(alias, conditions);
        }

        public static ILeftHandSideExpression Insured(this ILeftHandSideExpression lhs, params Expression<Func<Insured, bool>>[] conditions)
        {
            return lhs.Match(conditions);
        }

        public static IRightHandSideExpression Info(this IRightHandSideExpression rhs, Claim claim, string message)
        {
            return rhs.Do(ctx => InsertAlert(ctx, 1, claim, message));
        }

        public static IRightHandSideExpression Warning(this IRightHandSideExpression rhs, Claim claim, string message)
        {
            return rhs.Do(ctx => InsertAlert(ctx, 2, claim, message));
        }

        public static IRightHandSideExpression Error(this IRightHandSideExpression rhs, Claim claim, string message)
        {
            return rhs.Do(ctx => InsertAlert(ctx, 3, claim, message));
        }

        private static void InsertAlert(IContext context, int severity, Claim claim, string message)
        {
            var alert = new ClaimAlert { Severity = severity, Claim = claim, RuleName = context.Rule.Name, Message = message };
            context.Insert(alert);
        }
    }
}