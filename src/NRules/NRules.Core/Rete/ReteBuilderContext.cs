using System;
using System.Collections.Generic;
using NRules.Core.Rules;

namespace NRules.Core.Rete
{
    internal class ReteBuilderContext
    {
        public ReteBuilderContext(CompiledRule rule)
        {
            Rule = rule;
            BetaFactTypes = new List<Type>();
        }

        public List<Type> BetaFactTypes { get; private set; }
        public CompiledRule Rule { get; private set; }
    }
}