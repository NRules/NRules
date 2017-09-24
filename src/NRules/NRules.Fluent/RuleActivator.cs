using System;
using System.Collections.Generic;
using NRules.Fluent.Dsl;

namespace NRules.Fluent
{
    /// <summary>
    /// Rule activator that instantiates rules based on the .NET types.
    /// Default activator uses .NET reflection activator.
    /// An instance of <c>IRuleActivator</c> can be assigned to <see cref="RuleRepository.Activator"/>,
    /// so that all rule instantiation requests are delegated to the rule activator.
    /// </summary>
    public interface IRuleActivator
    {
        /// <summary>
        /// Creates rule's instances from a .NET type.
        /// </summary>
        /// <param name="type">Rule type.</param>
        /// <returns>Rule instances.</returns>
        /// <remarks>
        /// The same rule type may be instantiated multiple times with different parameters. 
        /// Each instance is considered as separate rule, and should have a unique name.
        /// </remarks>
        IEnumerable<Rule> Activate(Type type);
    }

    internal class RuleActivator : IRuleActivator
    {
        public IEnumerable<Rule> Activate(Type type)
        {
            yield return (Rule) Activator.CreateInstance(type);
        }
    }
}