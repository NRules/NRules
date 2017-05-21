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

        private readonly string _name;
        private readonly ExpressionMap _expressionMap;
        private readonly PatternElement _source;

        /// <summary>
        /// Fact source of the aggregate.
        /// </summary>
        public PatternElement Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Aggregate name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Expressions used by the aggregate.
        /// </summary>
        public ExpressionMap ExpressionMap
        {
            get { return _expressionMap; }
        }

        internal AggregateElement(IEnumerable<Declaration> declarations, Type resultType, string name, ExpressionMap expressionMap, PatternElement source) 
            : base(declarations, resultType) 
        {
            _name = name;
            _expressionMap = expressionMap;
            _source = source;
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitAggregate(context, this);
        }
    }
}