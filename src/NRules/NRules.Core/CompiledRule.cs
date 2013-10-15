using System;
using System.Collections.Generic;
using NRules.Rule;

namespace NRules.Core
{
    internal interface ICompiledRule
    {
        string Handle { get; }
        IRuleDefinition Definition { get; }
        IEnumerable<IRuleAction> Actions { get; } 
    }

    internal class CompiledRule : ICompiledRule
    {
        public CompiledRule(IRuleDefinition definition, IEnumerable<IRuleAction> actions)
        {
            Handle = Guid.NewGuid().ToString();
            Definition = definition;
            Actions = new List<IRuleAction>(actions);
        }

        public string Handle { get; private set; }
        public IRuleDefinition Definition { get; private set; }
        public IEnumerable<IRuleAction> Actions { get; private set; }
    }
}
