using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregate Expression with a name used in constructing/building an aggregator.
    /// </summary>
    public class NamedAggregateExpression : INamedExpression<IAggregateExpression>
    {
        internal NamedAggregateExpression(string name, IAggregateExpression expression)
        {
            Name = name;
            Expression = expression;
        }

        /// <summary>
        /// Expression name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Expression.
        /// </summary>
        public IAggregateExpression Expression { get; }
    }
}