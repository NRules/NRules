using System;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that creates new facts (aggregates) based on matching facts it receives as input.
    /// </summary>
    public class AggregateElement : PatternSourceElement
    {
        /// <summary>
        /// Type of the aggregate. Must implement <code>IAggregate</code> interface.
        /// </summary>
        public Type AggregateType { get; private set; }

        /// <summary>
        /// Fact source of the aggregate.
        /// </summary>
        public PatternElement Source { get; private set; }

        internal AggregateElement(Type resultType, Type aggregateType, PatternElement source) : base(resultType)
        {
            AggregateType = aggregateType;
            Source = source;
        }

        internal override void Accept(RuleElementVisitor visitor)
        {
            visitor.VisitAggregate(this);
        }
    }
}