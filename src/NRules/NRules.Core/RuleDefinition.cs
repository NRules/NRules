using System;
using System.Linq;
using System.Linq.Expressions;
using NRules.Core.Rules;
using NRules.Dsl;

namespace NRules.Core
{
    internal class RuleDefinition : IRuleDefinition, ILeftHandSide, IRightHandSide
    {
        private readonly CompiledRule _rule;

        public RuleDefinition(CompiledRule rule)
        {
            _rule = rule;
        }

        public ILeftHandSide If<T>(Expression<Func<T, bool>> condition)
        {
            var declaration = CheckDeclaration(typeof (T));
            var conditionElement = CreateConditionElement(condition);
            var predicate = new Predicate(PredicateTypes.Selection);
            predicate.Declarations.Add(declaration);
            predicate.Conditions.Add(conditionElement);
            _rule.Predicates.Add(predicate);
            return this;
        }

        public ILeftHandSide If<T1, T2>(Expression<Func<T1, T2, bool>> condition)
        {
            var declaration1 = CheckDeclaration(typeof (T1));
            var declaration2 = CheckDeclaration(typeof (T2));
            var conditionElement = CreateConditionElement(condition);
            var predicate = new Predicate(PredicateTypes.Join);
            predicate.Declarations.Add(declaration1);
            predicate.Declarations.Add(declaration2);
            predicate.Conditions.Add(conditionElement);
            _rule.Predicates.Add(predicate);
            return this;
        }

        public ILeftHandSide Collect<T>(Expression<Func<T, bool>> itemCondition)
        {
            var localDeclaration = new Declaration(string.Empty, typeof (T));
            var conditionElement = CreateConditionElement(itemCondition);
            var predicate = new Predicate(PredicateTypes.Aggregate);
            predicate.Declarations.Add(localDeclaration);
            predicate.Conditions.Add(conditionElement);
            predicate.AggregationStrategy = new CollectionAggregate<T>();
            _rule.Predicates.Add(predicate);
            return this;
        }

        public ILeftHandSide Exists<T>(Expression<Func<T, bool>> condition)
        {
            return this;
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

        private IDeclaration CheckDeclaration(Type type)
        {
            var declaration = _rule.Declarations.FirstOrDefault(d => d.Type == type);
            if (declaration == null)
            {
                declaration = new Declaration(string.Empty, type);
                _rule.Declarations.Add(declaration);
            }
            return declaration;
        }

        private Condition CreateConditionElement<T>(Expression<Func<T, bool>> condition)
        {
            string key = condition.ToString();
            Delegate compiledExpression = condition.Compile();
            var conditionElement = new Condition(key, compiledExpression, new[] {typeof (T)});
            return conditionElement;
        }

        private Condition CreateConditionElement<T1, T2>(Expression<Func<T1, T2, bool>> condition)
        {
            string key = condition.ToString();
            Delegate compiledExpression = condition.Compile();
            var conditionElement = new Condition(key, compiledExpression, new[] {typeof (T1), typeof (T2)});
            return conditionElement;
        }
    }
}