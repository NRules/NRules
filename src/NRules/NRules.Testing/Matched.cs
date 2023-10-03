using System;
using System.Linq.Expressions;

namespace NRules.Testing;

/// <summary>
/// Fluent builder for specifying fact constraints in a rule firing expectation.
/// </summary>
public static class Matched
{
    /// <summary>
    /// Creates a constraint that the matched fact is of the specified type.
    /// </summary>
    /// <typeparam name="T">Type of the matched fact.</typeparam>
    public static FactConstraint<T> Fact<T>()
    {
        return new TypedFactConstraint<T>();
    }

    /// <summary>
    /// Creates a constraint that the matched fact that is equal to the specified value.
    /// </summary>
    /// <typeparam name="T">Type of the matched fact.</typeparam>
    public static FactConstraint<T> Fact<T>(T factValue)
    {
        return new EqualFactConstraint<T>(factValue);
    }

    /// <summary>
    /// Creates a constraint that the matched fact satisfies the specified predicate.
    /// </summary>
    /// <typeparam name="T">Type of the matched fact.</typeparam>
    /// <param name="predicateExpression">Predicate that the fact must satisfy.</param>
    public static FactConstraint<T> Fact<T>(Expression<Func<T, bool>> predicateExpression)
    {
        return new PredicatedFactConstraint<T>(predicateExpression);
    }
}
