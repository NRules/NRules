using System;
using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class ReteBuilderContext
    {
        public ReteBuilderContext()
        {
            BetaFactTypes = new List<Type>();
            BetaConditions = new List<ICondition>();
        }

        public List<Type> BetaFactTypes { get; private set; } 
        public List<ICondition> BetaConditions { get; private set; } 
        public IAlphaMemoryNode AlphaMemoryNode { get; set; }
        public IBetaMemoryNode BetaMemoryNode { get; set; }
    }
}