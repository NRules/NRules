using System;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that creates new facts (aggregates) based on matching facts it receives as input.
    /// </summary>
    public class AggregateElement : PatternSourceElement
    {
        private readonly Type _aggregateType;
        private readonly PatternElement _source;

        /// <summary>
        /// Type of the aggregate. Must implement <see cref="IAggregate"/> interface.
        /// </summary>
        public Type AggregateType
        {
            get { return _aggregateType; }
        }

        /// <summary>
        /// Fact source of the aggregate.
        /// </summary>
        public PatternElement Source
        {
            get { return _source; }
        }

        internal AggregateElement(Type resultType, Type aggregateType, PatternElement source) : base(resultType)
        {
            _aggregateType = aggregateType;
            _source = source;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitAggregate(context, this);
        }
    }
}