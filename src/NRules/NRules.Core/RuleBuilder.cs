using System;
using System.Linq;
using System.Linq.Expressions;
using NRules.Core.Rules;

namespace NRules.Core
{
    public interface IRuleBuilder
    {
        IRuleBuilder Name(string name);
        IRuleBuilder Condition(LambdaExpression expression);
        IRuleBuilder Collect(LambdaExpression itemExpression);
        IRuleBuilder Exists(LambdaExpression expression);
        IRuleBuilder Action(Action<IActionContext> action);
    }

    internal class RuleBuilder : IRuleBuilder
    {
        private readonly CompiledRule _rule;

        public RuleBuilder(CompiledRule rule)
        {
            _rule = rule;
        }

        public IRuleBuilder Name(string name)
        {
            _rule.Name = name;
            return this;
        }

        public IRuleBuilder Condition(LambdaExpression expression)
        {
            var parameters = expression.Parameters.Select(p => p.Type).ToArray();
            var declaration = Declare(parameters.First());
            parameters.Skip(1).ToList().ForEach(CheckDeclaration);

            var conditionElement = CreateConditionElement(expression);

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

        public IRuleBuilder Collect(LambdaExpression itemExpression)
        {
            var parameterType = itemExpression.Parameters.Select(p => p.Type).First();
            var declaration = Declare(parameterType);
            var conditionElement = CreateConditionElement(itemExpression);

            var predicate = _rule.Predicates.FirstOrDefault(
                p => p.PredicateType == PredicateTypes.Aggregate &&
                     p.Declaration == declaration);
            if (predicate != null)
            {
                throw new InvalidOperationException(
                    string.Format("More than one collection of a given type defined. Type={0}",
                                  parameterType.Name));
            }

            predicate = new Predicate(PredicateTypes.Aggregate, declaration);
            predicate.StrategyType = typeof (CollectionAggregate<>).MakeGenericType(parameterType);
            _rule.Predicates.Add(predicate);

            _rule.Conditions.Add(conditionElement);
            return this;
        }

        public IRuleBuilder Exists(LambdaExpression expression)
        {
            var parameterType = expression.Parameters.Select(p => p.Type).First();
            var declaration = Declare(parameterType);
            var conditionElement = CreateConditionElement(expression);

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

        public IRuleBuilder Action(Action<IActionContext> action)
        {
            _rule.Actions.Add(new RuleAction(action));
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

        private Condition CreateConditionElement(LambdaExpression expression)
        {
            string key = expression.ToString();
            var conditionElement = new Condition(key, expression);
            return conditionElement;
        }
    }
}