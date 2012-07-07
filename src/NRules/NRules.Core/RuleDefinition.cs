using System;
using System.Linq;
using System.Linq.Expressions;
using NRules.Core.Rules;
using NRules.Dsl;

namespace NRules.Core
{
    internal class RuleDefinition : IRuleDefinition, ILeftHandSide, IRightHandSide
    {
        private readonly Rule _rule;

        public RuleDefinition(Rule rule)
        {
            _rule = rule;
        }

        public ILeftHandSide If<T>(Expression<Func<T, bool>> condition)
        {
            CheckDeclaration(typeof (T));
            _rule.Conditions.Add(new Condition<T>(condition));
            return this;
        }

        public ILeftHandSide If<T1, T2>(Expression<Func<T1, T2, bool>> condition)
        {
            CheckDeclaration(typeof (T1));
            CheckDeclaration(typeof (T2));
            _rule.JoinConditions.Add(new JoinCondition<T1, T2>(condition));
            return this;
        }

        private void CheckDeclaration(Type type)
        {
            if (_rule.Declarations.Any(d => d.Type == type)) return;
            _rule.Declarations.Add(new Declaration(string.Empty, type));
        }

        public IRightHandSide Do(Action<IActionContext> action)
        {
            _rule.Actions.Add(new RuleAction(action));
            return this;
        }

        public ILeftHandSide When()
        {
            return this;
        }

        public IRightHandSide Then()
        {
            return this;
        }
    }
}