using System;
using System.Collections.Generic;

namespace NRules.Core.Rules
{
    internal class CompositeDeclaration : ICompositeDeclaration
    {
        private readonly List<Type> _factTypes = new List<Type>();
        private readonly List<ICondition> _conditions = new List<ICondition>();

        public CompositeDeclaration(IAggregate aggregationStrategy)
        {
            AggregationStrategy = aggregationStrategy;
        }

        public IList<Type> FactTypes
        {
            get { return _factTypes; }
        }

        public IList<ICondition> Conditions
        {
            get { return _conditions; }
        }

        public IAggregate AggregationStrategy { get; private set; }
    }
}