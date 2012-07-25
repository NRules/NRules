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
            AlphaMemories = new Queue<AlphaMemory>();
            BetaFactTypes = new List<Type>();
        }

        public Queue<AlphaMemory> AlphaMemories { get; private set; }
        public List<Type> BetaFactTypes { get; private set; }
        public CompiledRule Rule { get; private set; }
    }
}