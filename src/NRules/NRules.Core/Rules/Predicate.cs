using System.Collections.Generic;

namespace NRules.Core.Rules
{
    internal class Predicate : IPredicate
    {
        public Predicate(PredicateTypes predicateType)
        {
            PredicateType = predicateType;
            Declarations = new List<IDeclaration>();
            Conditions = new List<ICondition>();
        }

        public PredicateTypes PredicateType { get; private set; }
        public IList<IDeclaration> Declarations { get; private set; }
        public IList<ICondition> Conditions { get; private set; }
        public IAggregate AggregationStrategy { get; set; }
    }
}