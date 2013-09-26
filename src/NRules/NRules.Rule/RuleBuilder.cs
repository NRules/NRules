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
        private readonly RuleDefinition _ruleDefinition;

        public RuleBuilder(RuleDefinition ruleDefinition)
        {
            _ruleDefinition = ruleDefinition;
        }

        public IRuleBuilder Name(string name)
        {
            _ruleDefinition.Name = name;
            return this;
        }

        public IRuleBuilder Priority(int priority)
        {
            _ruleDefinition.Priority = priority;
            return this;
        }

        public IRuleBuilder Condition(LambdaExpression expression)
        {
            var root = _ruleDefinition.LeftHandSide; 
            
            var parameter = expression.Parameters.First();
            var declaration = root.SymbolTable.Lookup(parameter.Name, parameter.Type);
            if (declaration == null)
            {
                var patternElement = new PatternElement(parameter.Type);
                root.AddChild(patternElement);
                declaration = patternElement.Declare(parameter.Name);
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
            
            var patternElement = new PatternElement(parameter.Type);
            var patternDeclaration = patternElement.Declare(parameter.Name);
            var condition = new ConditionElement(new[] { patternDeclaration }, itemExpression);
            patternElement.Add(condition);

            Type collectionType = typeof(IEnumerable<>).MakeGenericType(parameter.Type);
            Type aggregateType = typeof (CollectionAggregate<>).MakeGenericType(parameter.Type);
            var aggregateElement = new AggregateElement(collectionType, aggregateType, patternElement);
            _ruleDefinition.LeftHandSide.AddChild(aggregateElement);
            
            return this;
        }

        public IRuleBuilder Exists(LambdaExpression expression)
        {
            var parameter = expression.Parameters.First();

            var patternElement = new PatternElement(parameter.Type);
            var patternDeclaration = patternElement.Declare(parameter.Name);
            var condition = new ConditionElement(new[] { patternDeclaration }, expression);
            patternElement.Add(condition);

            var existsElement = new GroupElement(GroupType.Exists);
            existsElement.AddChild(patternElement);
            _ruleDefinition.LeftHandSide.AddChild(existsElement);

            return this;
        }

        public IRuleBuilder Action(Expression<Action<IActionContext>> action)
        {
            _ruleDefinition.AddAction(new RuleAction(action));
            return this;
        }
    }
}