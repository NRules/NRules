using System;
using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class ReteBuilderContext
    {
        public ReteBuilderContext()
        {
            BetaFactTypes = new List<Type>();
            BetaConditions = new List<BetaCondition>();
        }

        public List<Type> BetaFactTypes { get; private set; } 
        public List<BetaCondition> BetaConditions { get; private set; } 
        public IAlphaMemoryNode AlphaSource { get; set; }
        public IBetaMemoryNode BetaSource { get; set; }
    }
}