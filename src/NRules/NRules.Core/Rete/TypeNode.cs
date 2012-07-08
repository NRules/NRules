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
            throw new NotImplementedException();
        }

        public override void PropagateRetract(Fact fact)
        {
            throw new NotImplementedException();
        }
    }
}