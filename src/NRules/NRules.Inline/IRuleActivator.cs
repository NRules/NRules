using System;
using NRules.Dsl;

namespace NRules.Inline
{
    public interface IRuleActivator
    {
        IRule Activate(Type type);
    }

    internal class RuleActivator : IRuleActivator
    {
        public IRule Activate(Type type)
        {
            return (IRule) Activator.CreateInstance(type);
        }
    }
}