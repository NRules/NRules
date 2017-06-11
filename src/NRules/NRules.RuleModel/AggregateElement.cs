using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Fact source of the aggregate.
        /// </summary>
        public PatternElement Source { get; }

        /// <summary>
        /// Aggregate name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Expressions used by the aggregate.
        /// </summary>
        public ExpressionMap ExpressionMap { get; }

        internal AggregateElement(IEnumerable<Declaration> declarations, Type resultType, string name, ExpressionMap expressionMap, PatternElement source) 
            : base(declarations, resultType) 
        {
            Name = name;
            ExpressionMap = expressionMap;
            Source = source;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitAggregate(context, this);
        }
    }
}