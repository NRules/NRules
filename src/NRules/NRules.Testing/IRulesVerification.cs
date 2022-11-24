using System;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

/// <summary>
/// Fluent interface to build rules verification
/// </summary>
public interface IRulesVerification
{
    /// <summary>
    /// Selects single registered rule type
    /// </summary>
    /// <returns>Specific rule verification builder</returns>
    IRuleVerification Rule();

    /// <summary>
    /// Specifies rule type. Call this if you have multiple registered rule types, or want to be specific
    /// </summary>
    /// <typeparam name="T">Type of the rule to look for</typeparam>
    /// <returns>Specific rule verification builder</returns>
    IRuleVerification Rule<T>() where T : Rule;

    /// <summary>
    /// Specifies rule type. Call this if you have multiple registered rule types, or want to be specific
    /// </summary>
    /// <param name="ruleType"><see cref="Type"/> of the rule to look for</param>
    /// <returns>Specific rule verification builder</returns>
    IRuleVerification Rule(Type ruleType);
}
