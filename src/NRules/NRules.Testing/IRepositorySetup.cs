using System;
using NRules.Fluent.Dsl;

namespace NRules.Testing;

/// <summary>
/// Sets up rules repository.
/// </summary>
public interface IRepositorySetup
{
    /// <summary>
    /// Adds specific rule under test to the repository.
    /// </summary>
    /// <typeparam name="T">Type of the rule to add.</typeparam>
    /// <remarks>If <typeparamref name="T"/> is not concrete, it will be ignored</remarks>
    void Rule<T>() where T : Rule;

    /// <summary>
    /// Adds specific rule under test to the repository.
    /// </summary>
    /// <param name="ruleType"><see cref="Type"/> of the rule to add.</param>
    /// <remarks>If <paramref name="ruleType"/> is not derived from <see cref="Fluent.Dsl.Rule"/> and not concrete, it will be ignored.</remarks>
    void Rule(Type ruleType);
}
