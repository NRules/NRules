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

        public IRuleBuilder Priority(int priority)
        {
            _rule.Priority = priority;
            return this;
        }

        public IRuleBuilder Condition(LambdaExpression expression)
        {
            var parameter = expression.Parameters.First();
            var declaration = Declare(parameter.Name, parameter.Type);
            var declarations = expression.Parameters.Select(p => GetDeclaration(p.Name, p.Type));

            if (declaration.Source == null)
            {
                declaration.Source = new ConditionElement {ObjectType = declaration.Type};
            }

            var condition = new Condition(declarations, expression);
            declaration.Source.Add(condition);

            return this;
        }

        public IRuleBuilder Collect(LambdaExpression itemExpression)
        {
            var inputParameter = itemExpression.Parameters.First();
            var inputDeclaration = new Declaration(inputParameter.Name, inputParameter.Type);

            inputDeclaration.Source = new ConditionElement { ObjectType = inputDeclaration.Type };
            var condition = new Condition(new[] { inputDeclaration }, itemExpression);
            inputDeclaration.Source.Add(condition);
            
            Type collectionType = typeof(IEnumerable<>).MakeGenericType(inputParameter.Type);
            var outputParameter = Expression.Parameter(collectionType, "collection");
            var outputDeclaration = Declare(outputParameter.Name, outputParameter.Type);
            outputDeclaration.Source = new AggregateElement { ObjectType = outputDeclaration.Type, Declaration = inputDeclaration};

            return this;
        }

        public IRuleBuilder Exists(LambdaExpression expression)
        {
            var inputParameter = expression.Parameters.First();
            var inputDeclaration = new Declaration(inputParameter.Name, inputParameter.Type);

            inputDeclaration.Source = new ConditionElement(){ObjectType = inputDeclaration.Type};
            var condition = new Condition(new []{inputDeclaration}, expression);
            inputDeclaration.Source.Add(condition);

            var outputParameter = Expression.Parameter(inputParameter.Type, "exists");
            var outputDeclaration = Declare(outputParameter.Name, outputParameter.Type);
            outputDeclaration.Source = new ExistsElement {ObjectType = outputDeclaration.Type, Declaration = inputDeclaration};

            return this;
        }

        public IRuleBuilder Action(Action<IActionContext> action)
        {
            _rule.AddAction(new RuleAction(action));
            return this;
        }

        private Declaration Declare(string name, Type type)
        {
            var declaration = _rule.Declarations.FirstOrDefault(d => d.Type == type && d.Name == name);
            if (declaration == null)
            {
                declaration = new Declaration(name, type);
                _rule.Declarations.Add(declaration);
            }
            return declaration;
        }

        private Declaration GetDeclaration(string name, Type type)
        {
            var declaration = _rule.Declarations.FirstOrDefault(d => d.Type == type && d.Name == name);
            if (declaration == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Rule uses undeclared variable. Rule={0}, Name={1}, Type={2}",
                    _rule.Name, name, type.Name));
            }
            return declaration;
        }
    }
}