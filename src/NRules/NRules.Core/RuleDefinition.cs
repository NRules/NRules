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
            var declaration = Declare(typeof (T));
            var conditionElement = CreateConditionElement(condition);

            var predicate =
                _rule.Predicates.FirstOrDefault(
                    p => p.PredicateType == PredicateTypes.Selection &&
                         p.Declaration == declaration);
            if (predicate == null)
            {
                predicate = new Predicate(PredicateTypes.Selection, declaration);
                _rule.Predicates.Add(predicate);
            }

            _rule.Conditions.Add(conditionElement);
            return this;
        }

        public ILeftHandSide If<T1, T2>(Expression<Func<T1, T2, bool>> condition)
        {
            var declaration = Declare(typeof (T1));
            CheckDeclaration(typeof (T2));
            var conditionElement = CreateConditionElement(condition);

            var predicate = _rule.Predicates.FirstOrDefault(
                p => p.PredicateType == PredicateTypes.Selection &&
                     p.Declaration == declaration);
            if (predicate == null)
            {
                predicate = new Predicate(PredicateTypes.Selection, declaration);
                _rule.Predicates.Add(predicate);
            }

            _rule.Conditions.Add(conditionElement);
            return this;
        }

        public ILeftHandSide Collect<T>(Expression<Func<T, bool>> itemCondition)
        {
            var declaration = Declare(typeof (T));
            var conditionElement = CreateConditionElement(itemCondition);

            var predicate = _rule.Predicates.FirstOrDefault(
                p => p.PredicateType == PredicateTypes.Aggregate &&
                     p.Declaration == declaration);
            if (predicate != null)
            {
                throw new InvalidOperationException(
                    string.Format("More than one collection of a given type defined. Type={0}",
                                  typeof (T).Name));
            }

            predicate = new Predicate(PredicateTypes.Aggregate, declaration);
            predicate.StrategyType = typeof (CollectionAggregate<T>);
            _rule.Predicates.Add(predicate);

            _rule.Conditions.Add(conditionElement);
            return this;
        }

        public ILeftHandSide Exists<T>(Expression<Func<T, bool>> condition)
        {
            var declaration = Declare(typeof (T));
            var conditionElement = CreateConditionElement(condition);

            var predicate = _rule.Predicates.FirstOrDefault(
                p => p.PredicateType == PredicateTypes.Existential &&
                     p.Declaration == declaration);
            if (predicate == null)
            {
                predicate = new Predicate(PredicateTypes.Existential, declaration);
                _rule.Predicates.Add(predicate);
            }

            _rule.Conditions.Add(conditionElement);
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

        private IDeclaration Declare(Type type)
        {
            var declaration = _rule.Declarations.FirstOrDefault(d => d.Type == type);
            if (declaration == null)
            {
                declaration = new Declaration(string.Empty, type);
                _rule.Declarations.Add(declaration);
            }
            return declaration;
        }

        private void CheckDeclaration(Type type)
        {
            var declaration = _rule.Declarations.FirstOrDefault(d => d.Type == type);
            if (declaration == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Rule uses input of a given type before defining it. Rule={0}, Type={1}",
                    _rule.Name, type.Name));
            }
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