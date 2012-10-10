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

        public override bool IsSatisfiedBy(Fact fact)
        {
            bool isMatchingType = FilterType.IsAssignableFrom(fact.FactType);
            return isMatchingType;
        }

        protected override void UnsatisfiedFactUpdate(IWorkingMemory workingMemory, Fact fact)
        {
            //Do nothing, since fact type will never change
        }
    }
}