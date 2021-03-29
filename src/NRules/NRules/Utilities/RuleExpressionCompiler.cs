using System;
using System.Collections.Generic;
using System.Linq;
using NRules.AgendaFilters;
using NRules.Aggregators;
using NRules.Extensibility;
using NRules.Rete;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Utilities
{
    internal interface IRuleExpressionCompiler
    {
        IExpressionCompiler ExpressionCompiler { get; set; }
        ILhsExpression<TResult> CompileLhsExpression<TResult>(ExpressionElement element, List<Declaration> declarations);
        ILhsFactExpression<TResult> CompileLhsFactExpression<TResult>(ExpressionElement element);
        ILhsTupleExpression<TResult> CompileLhsTupleExpression<TResult>(ExpressionElement element, List<Declaration> declarations);
        IActivationExpression<TResult> CompileActivationExpression<TResult>(ExpressionElement element,
            List<Declaration> declarations, IndexMap tupleFactMap);
        IRuleAction CompileAction(ActionElement element, List<Declaration> declarations,
            List<DependencyElement> dependencies, IndexMap tupleFactMap);
        IAggregateExpression CompileAggregateExpression(NamedExpressionElement element, List<Declaration> declarations);
    }

    internal class RuleExpressionCompiler : IRuleExpressionCompiler
    {
        public IExpressionCompiler ExpressionCompiler { get; set; } = new ExpressionCompiler();

        public ILhsExpression<TResult> CompileLhsExpression<TResult>(ExpressionElement element, List<Declaration> declarations)
        {
            if (element.Imports.Count() == 1 &&
                Equals(element.Imports.Single(), declarations.Last()))
            {
                return CompileLhsFactExpression<TResult>(element);
            }
            return CompileLhsTupleFactExpression<TResult>(element, declarations);
        }

        public ILhsFactExpression<TResult> CompileLhsFactExpression<TResult>(ExpressionElement element)
        {
            var optimizedExpression = ExpressionOptimizer.Optimize<Func<Fact, TResult>>(
                element.Expression, IndexMap.Unit, tupleInput: false, factInput: true);
            var @delegate = optimizedExpression.Compile();
            var argumentMap = new ArgumentMap(IndexMap.Unit, 1);
            var expression = new LhsFactExpression<TResult>(element.Expression, @delegate, argumentMap);
            return expression;
        }

        public ILhsTupleExpression<TResult> CompileLhsTupleExpression<TResult>(ExpressionElement element, List<Declaration> declarations)
        {
            var factMap = IndexMap.CreateMap(element.Imports, declarations);
            var optimizedExpression = ExpressionOptimizer.Optimize<Func<Tuple, TResult>>(
                element.Expression, factMap, tupleInput: true, factInput: false);
            var @delegate = ExpressionCompiler.Compile(optimizedExpression);
            var argumentMap = new ArgumentMap(factMap, element.Expression.Parameters.Count);
            var expression = new LhsTupleExpression<TResult>(element.Expression, @delegate, argumentMap);
            return expression;
        }

        public ILhsExpression<TResult> CompileLhsTupleFactExpression<TResult>(ExpressionElement element, List<Declaration> declarations)
        {
            var factMap = IndexMap.CreateMap(element.Imports, declarations);
            var optimizedExpression = ExpressionOptimizer.Optimize<Func<Tuple, Fact, TResult>>(
                element.Expression, factMap, tupleInput: true, factInput: true);
            var @delegate = ExpressionCompiler.Compile(optimizedExpression);
            var argumentMap = new ArgumentMap(factMap, element.Expression.Parameters.Count);
            var expression = new LhsExpression<TResult>(element.Expression, @delegate, argumentMap);
            return expression;
        }

        public IActivationExpression<TResult> CompileActivationExpression<TResult>(ExpressionElement element,
            List<Declaration> declarations, IndexMap tupleFactMap)
        {
            var activationFactMap = IndexMap.CreateMap(element.Imports, declarations);
            var factMap = IndexMap.Compose(tupleFactMap, activationFactMap);
            var optimizedExpression = ExpressionOptimizer.Optimize<Func<Tuple, TResult>>(
                element.Expression, factMap, tupleInput: true, factInput: false);
            var @delegate = ExpressionCompiler.Compile(optimizedExpression);
            var argumentMap = new ArgumentMap(factMap, element.Expression.Parameters.Count);
            var expression = new ActivationExpression<TResult>(element.Expression, @delegate, argumentMap);
            return expression;
        }

        public IRuleAction CompileAction(ActionElement element, List<Declaration> declarations,
            List<DependencyElement> dependencies, IndexMap tupleFactMap)
        {
            var activationFactMap = IndexMap.CreateMap(element.Imports, declarations);
            var factMap = IndexMap.Compose(tupleFactMap, activationFactMap);

            var dependencyIndexMap = IndexMap.CreateMap(element.Imports, dependencies.Select(x => x.Declaration));
            if (dependencyIndexMap.HasData)
            {
                var optimizedExpression = ExpressionOptimizer
                    .Optimize<Action<IContext, Tuple, IDependencyResolver, IResolutionContext>>(
                        element.Expression, factMap, dependencies, dependencyIndexMap);
                var @delegate = ExpressionCompiler.Compile(optimizedExpression);
                var argumentMap = new ArgumentMap(factMap, element.Expression.Parameters.Count - 1);
                var action = new RuleActionWithDependencies(element.Expression, @delegate, argumentMap, element.ActionTrigger);
                return action;
            }
            else
            {
                var optimizedExpression = ExpressionOptimizer.Optimize<Action<IContext, Tuple>>(
                    element.Expression, 1, factMap, tupleInput: true, factInput: false);
                var @delegate = ExpressionCompiler.Compile(optimizedExpression);
                var argumentMap = new ArgumentMap(factMap, element.Expression.Parameters.Count - 1);
                var action = new RuleAction(element.Expression, @delegate, argumentMap, element.ActionTrigger);
                return action;
            }
        }
        
        public IAggregateExpression CompileAggregateExpression(NamedExpressionElement element, List<Declaration> declarations)
        {
            var compiledExpression = CompileLhsExpression<object>(element, declarations);
            var expression = new AggregateExpression(element.Name, compiledExpression);
            return expression;
        }
    }
}