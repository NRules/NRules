using System;
using NRules.Dsl;

namespace NRules
{
    public interface IRule
    {
        void Define(IRuleDefinition definition);
        void InjectEventHandler(EventHandler eventHandler);
    }
}