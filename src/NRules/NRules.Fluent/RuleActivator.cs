using System;
using NRules.Fluent.Dsl;

namespace NRules.Fluent
{
    /// <summary>
    /// Rule activator that instantiates rules based on .NET types.
    /// Default activator uses .NET reflection activator.
    /// </summary>
    public interface IRuleActivator
    {
        /// <summary>
        /// Creates an instance of a rule from a .NET type.
        /// </summary>
        /// <param name="type">Rule type.</param>
        /// <returns>Rule instance.</returns>
        Rule Activate(Type type);
    }

    internal class RuleActivator : IRuleActivator
    {
        public Rule Activate(Type type)
        {
            return (Rule) Activator.CreateInstance(type);
        }
    }
}