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
        }

        public Queue<AlphaMemory> AlphaMemories { get; private set; }
        public CompiledRule Rule { get; private set; }
    }
}