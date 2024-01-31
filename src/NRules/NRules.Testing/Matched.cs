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
    /// <typeparam name="TFact">Type of the matched fact.</typeparam>
    public static FactConstraint<TFact> Fact<TFact>()
        where TFact : notnull
    {
        return new TypedFactConstraint<TFact>();
    }

    /// <summary>
    /// Creates a constraint that the matched fact is equal to the specified value.
    /// </summary>
    /// <typeparam name="TFact">Type of the matched fact.</typeparam>
    /// <param name="factValue">Value that the matched fact must be equal to.</param>
    public static FactConstraint<TFact> Fact<TFact>(TFact factValue)
        where TFact : notnull
    {
        return new EqualFactConstraint<TFact>(factValue);
    }

    /// <summary>
    /// Creates a constraint that the matched fact satisfies the specified predicate.
    /// </summary>
    /// <typeparam name="TFact">Type of the matched fact.</typeparam>
    /// <param name="predicateExpression">Predicate that the matched fact must satisfy.</param>
    public static FactConstraint<TFact> Fact<TFact>(Expression<Func<TFact, bool>> predicateExpression)
        where TFact : notnull
    {
        return new PredicatedFactConstraint<TFact>(predicateExpression);
    }
}
