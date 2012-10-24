using System;
using System.Collections.Generic;
using NRules.Rule;

namespace NRules.Core.Rete
{
    internal class ReteBuilderContext
    {
        public ReteBuilderContext(ICompiledRule rule)
        {
            Rule = rule;
            BetaFactTypes = new List<Type>();
        }

        public List<Type> BetaFactTypes { get; private set; }
        public ICompiledRule Rule { get; private set; }
    }
}