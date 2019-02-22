using System;

namespace NRules.RuleModel
{
    /// <summary>
    /// Rule element that creates new facts (aggregates) based on matching facts it receives as input.
    /// </summary>
    public class AggregateElement : PatternSourceElement
    {
        public const string CollectName = "Collect";
        public const string GroupByName = "GroupBy";
        public const string ProjectName = "Project";
        public const string FlattenName = "Flatten";

        public const string SelectorName = "Selector";
        public const string ElementSelectorName = "ElementSelector";
        public const string KeySelectorName = "KeySelector";
        public const string KeySelectorAscendingName = "KeySelectorAscending";
        public const string KeySelectorDescendingName = "KeySelectorDescending";

        /// <summary>
        /// Fact source of the aggregate.
        /// </summary>
        public PatternElement Source { get; }

        /// <summary>
        /// Aggregate name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of custom aggregator factory.
        /// </summary>
        public Type CustomFactoryType { get; }

        /// <summary>
        /// Expressions used by the aggregate.
        /// </summary>
        public ExpressionCollection ExpressionCollection { get; }

        internal AggregateElement(Type resultType, string name, ExpressionCollection expressionCollection, PatternElement source, Type customFactoryType)
            : base(resultType)
        {
            Name = name;
            ExpressionCollection = expressionCollection;
            Source = source;
            CustomFactoryType = customFactoryType;

            AddImports(source);
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitAggregate(context, this);
        }
    }
}