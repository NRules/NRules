using System;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

/// <summary>
/// Fluent interface to build rules verification.
/// </summary>
public interface IRulesVerification
{
    /// <summary>
    /// Single registered rule under test.
    /// </summary>
    /// <returns>Specific rule verification builder.</returns>
    IRuleVerification Rule();

    /// <summary>
    /// Registered rule under test with a specifies rule type.
    /// Call this if you have multiple registered rules under test, or want to be specific.
    /// </summary>
    /// <typeparam name="T">Type of the rule to look for.</typeparam>
    /// <returns>Specific rule verification builder.</returns>
    IRuleVerification Rule<T>() where T : Rule;

    /// <summary>
    /// Registered rule under test with a specifies rule type.
    /// Call this if you have multiple registered rules under test, or want to be specific.
    /// </summary>
    /// <param name="ruleType"><see cref="Type"/> of the rule to look for.</param>
    /// <returns>Specific rule verification builder.</returns>
    IRuleVerification Rule(Type ruleType);
}
