using System;

namespace NRules.Core.Rete
{
    internal class TypeNode : AlphaNode
    {
        public TypeNode(Type filterType)
        {
            FilterType = filterType;
        }

        public Type FilterType { get; private set; }

        public bool IsSatisfiedBy(Fact fact)
        {
            bool isMatchingType = FilterType.IsAssignableFrom(fact.FactType);
            return isMatchingType;
        }

        public override void PropagateAssert(Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateAssert(fact);
                }
            }
        }

        public override void PropagateUpdate(Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateUpdate(fact);
                }
            }
        }

        public override void PropagateRetract(Fact fact)
        {
            if (IsSatisfiedBy(fact))
            {
                foreach (var childNode in ChildNodes)
                {
                    childNode.PropagateRetract(fact);
                }
            }
        }

        public override void ForceRetract(Fact fact)
        {
            PropagateRetract(fact);
        }
    }
}