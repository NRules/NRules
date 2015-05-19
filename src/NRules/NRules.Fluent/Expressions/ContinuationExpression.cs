using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal interface IContinuationExpression
    {
        PatternBuilder Complete(IPatternContainerBuilder builder);
    }

    internal class ContinuationExpression<TFact> : LeftHandSideExpression, IContinuationExpression, IContinuationConditionExpression<TFact>, IConditionExpression<TFact>
    {
        private readonly LeftHandSideExpression _baseExpression;
        private readonly IContinuationExpression _sourceExpression;
        private Func<IPatternContainerBuilder, PatternBuilder> _buildAction;


        public ContinuationExpression(LeftHandSideExpression baseExpression)
            : base(baseExpression.Builder, baseExpression.GroupBuilder)
        {
            _baseExpression = baseExpression;
            _baseExpression.RegisterContinuation(this);
        }
        
        public ContinuationExpression(LeftHandSideExpression baseExpression, IContinuationExpression sourceExpression)
            : this(baseExpression)
        {
            _sourceExpression = sourceExpression;
        }

        public Func<IPatternContainerBuilder, PatternBuilder> BuildAction
        {
            get { return _buildAction; }
        }

        public LeftHandSideExpression BaseExpression
        {
            get { return _baseExpression; }
        }

        public IContinuationExpression<TFact> Match(params Expression<Func<TFact, bool>>[] conditions)
        {
            _buildAction = b =>
            {
                var patternBuilder = b.Pattern(typeof(TFact));
                patternBuilder.DslConditions(GroupBuilder.Declarations, conditions);
                return patternBuilder;
            };
            return new ContinuationExpression<TFact>(BaseExpression, this);
        }

        public IConditionExpression<IEnumerable<TFact>> Collect(Expression<Func<IEnumerable<TFact>>> alias)
        {
            var symbol = alias.ToParameterExpression();

            _buildAction = b =>
            {
                var aggregatePatternBuilder = b.Pattern(symbol.Type, symbol.Name);

                var aggregateBuilder = aggregatePatternBuilder.Aggregate();
                aggregateBuilder.CollectionOf(typeof(TFact));

                _sourceExpression.Complete(aggregateBuilder);
                return aggregatePatternBuilder;
            };

            return new ContinuationExpression<IEnumerable<TFact>>(BaseExpression, this);
        }

        public IConditionExpression<IGrouping<TKey, TFact>> GroupBy<TKey>(Expression<Func<IGrouping<TKey, TFact>>> alias, Expression<Func<TFact, TKey>> keySelector)
        {
            var symbol = alias.ToParameterExpression();

            _buildAction = b =>
            {
                var aggregatePatternBuilder = b.Pattern(symbol.Type, symbol.Name);

                var aggregateBuilder = aggregatePatternBuilder.Aggregate();
                aggregateBuilder.GroupBy(keySelector, x => x);

                _sourceExpression.Complete(aggregateBuilder);
                return aggregatePatternBuilder;
            };

            return new ContinuationExpression<IGrouping<TKey, TFact>>(BaseExpression, this);
        }

        public IContinuationConditionExpression<IGrouping<TKey, TFact>> GroupBy<TKey>(Expression<Func<TFact, TKey>> keySelector)
        {
            return GroupBy(keySelector, x => x);
        }

        public IContinuationConditionExpression<IGrouping<TKey, TValue>> GroupBy<TKey, TValue>(Expression<Func<TFact, TKey>> keySelector, Expression<Func<TFact, TValue>> valueSelector)
        {
            _buildAction = b =>
            {
                var aggregatePatternBuilder = b.Pattern(typeof(IGrouping<TKey, TFact>));

                var aggregateBuilder = aggregatePatternBuilder.Aggregate();
                aggregateBuilder.GroupBy(keySelector, valueSelector);

                _sourceExpression.Complete(aggregateBuilder);
                return aggregatePatternBuilder;
            };

            return new ContinuationExpression<IGrouping<TKey, TValue>>(BaseExpression, this);
        }

        IContinuationExpression<TFact> IContinuationConditionExpression<TFact>.Where(params Expression<Func<TFact, bool>>[] conditions)
        {
            _buildAction = b =>
            {
                var patternBuilder = _sourceExpression.Complete(b);
                patternBuilder.DslConditions(GroupBuilder.Declarations, conditions);
                return patternBuilder;
            };
            return new ContinuationExpression<TFact>(BaseExpression, this);
        }

        ILeftHandSideExpression IConditionExpression<TFact>.Where(params Expression<Func<TFact, bool>>[] conditions)
        {
            _buildAction = b =>
            {
                var patternBuilder = _sourceExpression.Complete(b);
                patternBuilder.DslConditions(GroupBuilder.Declarations, conditions);
                return patternBuilder;
            };
            return BaseExpression;
        }

        public PatternBuilder Complete(IPatternContainerBuilder builder)
        {
            if (_buildAction != null) return _buildAction(builder);
            return _sourceExpression.Complete(builder);
        }
    }
}
