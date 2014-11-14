using System;
using NRules.RuleModel;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while compiling rule.
    /// </summary>
    public class RuleCompilationException : Exception
    {
        internal RuleCompilationException(string message, IRuleDefinition rule, Exception innerException)
            : base(message, innerException)
        {
            Rule = rule;
        }

        /// <summary>
        /// Rule that caused exception.
        /// </summary>
        public IRuleDefinition Rule { get; private set; }
    }
}