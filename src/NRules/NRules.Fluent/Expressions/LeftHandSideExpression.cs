using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class LeftHandSideExpression : ILeftHandSideExpression
    {
        private readonly GroupBuilder _builder;
        private readonly SymbolStack _symbolStack;
        private PatternBuilder _currentPatternBuilder;

        public LeftHandSideExpression(GroupBuilder builder, SymbolStack symbolStack)
        {
            _builder = builder;
            _symbolStack = symbolStack;
        }

        public ILeftHandSideExpression Match<TFact>(Expression<Func<TFact>> alias, params Expression<Func<TFact, bool>>[] conditions)
        {
            var symbol = alias.ToParameterExpression();
            var patternBuilder = _builder.Pattern(symbol.Type, symbol.Name);
            patternBuilder.DslConditions(_symbolStack.Scope.Declarations, conditions);
            _symbolStack.Scope.Add(patternBuilder.Declaration);
            _currentPatternBuilder = patternBuilder;
            return this;
        }

        public ILeftHandSideExpression Match<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            var symbol = Expression.Parameter(typeof (TFact));
            var patternBuilder = _builder.Pattern(symbol.Type, symbol.Name);
            patternBuilder.DslConditions(_symbolStack.Scope.Declarations, conditions);
            _symbolStack.Scope.Add(patternBuilder.Declaration);
            _currentPatternBuilder = patternBuilder;
            return this;
        }

        public ILeftHandSideExpression Exists<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            var existsBuilder = _builder.Exists();
            var patternBuilder = existsBuilder.Pattern(typeof(TFact));
            patternBuilder.DslConditions(_symbolStack.Scope.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression Not<TFact>(params Expression<Func<TFact, bool>>[] conditions)
        {
            var notBuilder = _builder.Not();
            var patternBuilder = notBuilder.Pattern(typeof(TFact));
            patternBuilder.DslConditions(_symbolStack.Scope.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> condition)
        {
            return ForAll(x => true, condition);
        }

        public ILeftHandSideExpression All<TFact>(Expression<Func<TFact, bool>> baseCondition, params Expression<Func<TFact, bool>>[] conditions)
        {
            return ForAll(baseCondition, conditions);
        }

        private ILeftHandSideExpression ForAll<TFact>(Expression<Func<TFact, bool>> baseCondition, params Expression<Func<TFact, bool>>[] conditions)
        {
            var forallBuilder = _builder.ForAll();

            var basePatternBuilder = forallBuilder.BasePattern(typeof(TFact));
            basePatternBuilder.DslConditions(_symbolStack.Scope.Declarations, baseCondition);

            var patternBuilder = forallBuilder.Pattern(typeof(TFact));
            patternBuilder.DslConditions(_symbolStack.Scope.Declarations, conditions);
            return this;
        }

        public ILeftHandSideExpression Query<TResult>(Expression<Func<TResult>> alias, Func<IQuery, IQuery<TResult>> queryAction)
        {
            var symbol = alias.ToParameterExpression();
            var queryBuilder = new QueryExpression(symbol, _symbolStack, _builder);
            queryAction(queryBuilder);
            _currentPatternBuilder = queryBuilder.Build();
            return this;
        }

        public ILeftHandSideExpression And(Action<ILeftHandSideExpression> builderAction)
        {
            var expressionBuilder = new LeftHandSideExpression(_builder.Group(GroupType.And), _symbolStack);
            builderAction(expressionBuilder);
            return this;
        }

        public ILeftHandSideExpression Or(Action<ILeftHandSideExpression> builderAction)
        {
            var expressionBuilder = new LeftHandSideExpression(_builder.Group(GroupType.Or), _symbolStack);
            builderAction(expressionBuilder);
            return this;
        }

        public ILeftHandSideExpression Let<TResult>(Expression<Func<TResult>> alias, Expression<Func<TResult>> expression)
        {
            var symbol = alias.ToParameterExpression();
            var patternBuilder = _builder.Pattern(symbol.Type, symbol.Name);
            var bindingBuilder = patternBuilder.Binding();
            bindingBuilder.DslBindingExpression(_symbolStack.Scope.Declarations, expression);
            _symbolStack.Scope.Add(patternBuilder.Declaration);
            _currentPatternBuilder = patternBuilder;
            return this;
        }

        public ILeftHandSideExpression Having(params Expression<Func<bool>>[] conditions)
        {
            if (_currentPatternBuilder == null)
            {
                throw new ArgumentException("HAVING clause can only be used on existing rule patterns");
            }
            _currentPatternBuilder.DslConditions(_symbolStack.Scope.Declarations, conditions);
            return this;
        }
    }
}
