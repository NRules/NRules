using System;
using System.Collections.Generic;
using NRules.Rule;

namespace NRules.Core.Rete
{
    internal class ReteBuilderContext
    {
        public ReteBuilderContext()
        {
            Declarations = new List<Declaration>();
            BetaConditions = new List<ConditionElement>();
        }

        public List<Declaration> Declarations { get; private set; } 
        public List<ConditionElement> BetaConditions { get; private set; } 
        public IAlphaMemoryNode AlphaSource { get; set; }
        public IBetaMemoryNode BetaSource { get; set; }
    }
}