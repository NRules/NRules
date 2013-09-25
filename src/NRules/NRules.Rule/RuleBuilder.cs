using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Dsl;

namespace NRules.Rule
{
    public interface IRuleBuilder
    {
        IRuleBuilder Name(string name);
        IRuleBuilder Priority(int priority);
        IRuleBuilder Condition(LambdaExpression expression);
        IRuleBuilder Collect(LambdaExpression itemExpression);
        IRuleBuilder Exists(LambdaExpression expression);
        IRuleBuilder Action(Expression<Action<IActionContext>> action);
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

        public IRuleBuilder Priority(int priority)
        {
            _rule.Priority = priority;
            return this;
        }

        public IRuleBuilder Condition(LambdaExpression expression)
        {
            var root = _rule.LeftSide; 
            
            var parameter = expression.Parameters.First();
            var declaration = root.SymbolTable.Lookup(parameter.Name, parameter.Type);
            if (declaration == null)
            {
                var matchElement = new MatchElement(parameter.Type);
                root.AddChild(matchElement);
                declaration = matchElement.Declare(parameter.Name);
                root.SymbolTable.Add(declaration);
            }

            var declarations = expression.Parameters.Select(p => root.SymbolTable.Lookup(p.Name, p.Type));
            var condition = new ConditionElement(declarations, expression);
            declaration.Target.Add(condition);

            return this;
        }

        public IRuleBuilder Collect(LambdaExpression itemExpression)
        {
            var parameter = itemExpression.Parameters.First();
            
            var matchElement = new MatchElement(parameter.Type);
            var matchDeclaration = matchElement.Declare(parameter.Name);
            var condition = new ConditionElement(new[] { matchDeclaration }, itemExpression);
            matchElement.Add(condition);

            Type collectionType = typeof(IEnumerable<>).MakeGenericType(parameter.Type);
            Type aggregateType = typeof (CollectionAggregate<>).MakeGenericType(parameter.Type);
            var aggregateElement = new AggregateElement(collectionType, matchElement, aggregateType);
            _rule.LeftSide.AddChild(aggregateElement);
            
            return this;
        }

        public IRuleBuilder Exists(LambdaExpression expression)
        {
            var parameter = expression.Parameters.First();

            var matchElement = new MatchElement(parameter.Type);
            var matchDeclaration = matchElement.Declare(parameter.Name);
            var condition = new ConditionElement(new[] { matchDeclaration }, expression);
            matchElement.Add(condition);

            var existsElement = new GroupElement(GroupType.Exists);
            existsElement.AddChild(matchElement);
            _rule.LeftSide.AddChild(existsElement);

            return this;
        }

        public IRuleBuilder Action(Expression<Action<IActionContext>> action)
        {
            _rule.AddAction(new RuleAction(action));
            return this;
        }
    }
}