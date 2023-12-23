using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Testing;

/// <summary>
/// Represents a constraint on the facts matched by a rule.
/// Fact constraints are used to configure rule firing expectations. 
/// </summary>
public abstract class FactConstraint
{
    /// <summary>
    /// Checks if the constraint is satisfied by a <see cref="IFactMatch"/>.
    /// </summary>
    /// <param name="factMatch"><see cref="IFactMatch"/> to check.</param>
    /// <returns>The result of the check.</returns>
    internal abstract bool IsSatisfied(IFactMatch factMatch);

    /// <summary>
    /// Gets the text representation of the constraint.
    /// </summary>
    internal abstract string GetText();

    /// <summary>
    /// Called with the corresponding <see cref="IFactMatch"/> when all fact constraints are satisfied for a rule firing.
    /// </summary>
    /// <param name="factMatch"></param>
    internal virtual void OnSatisfied(IFactMatch factMatch)
    {
    }
}

/// <summary>
/// Represents a strongly-typed constraint on facts matched by a rule.
/// </summary>
public abstract class FactConstraint<TFact> : FactConstraint
{
    private List<Action<TFact>> _callbacks;

    /// <summary>
    /// Called with the corresponding fact when all rule firing fact constraints are satisfied.
    /// </summary>
    /// <param name="callback">The delegate to call.</param>
    public FactConstraint<TFact> Callback(Action<TFact> callback)
    {
        _callbacks ??= new List<Action<TFact>>();
        _callbacks.Add(callback);
        return this;
    }

    internal override void OnSatisfied(IFactMatch factMatch)
    {
        base.OnSatisfied(factMatch);
        if (_callbacks == null) return;
        foreach (var callback in _callbacks)
        {
            callback.Invoke((TFact)factMatch.Value);
        }
    }
}

internal class TypedFactConstraint<TFact> : FactConstraint<TFact>
{
    internal override bool IsSatisfied(IFactMatch factMatch)
    {
        return typeof(TFact).IsAssignableFrom(factMatch.Declaration.Type);
    }

    internal override string GetText()
    {
        return $"Fact {typeof(TFact)}";
    }
}

internal class PredicatedFactConstraint<TFact> : FactConstraint<TFact>
{
    private readonly Expression<Func<TFact, bool>> _predicateExpression;
    private readonly Func<TFact, bool> _predicate;

    public PredicatedFactConstraint(Expression<Func<TFact, bool>> predicateExpression)
    {
        _predicateExpression = predicateExpression;
        _predicate = predicateExpression.Compile();
    }

    internal override bool IsSatisfied(IFactMatch factMatch)
    {
        if (typeof(TFact).IsAssignableFrom(factMatch.Declaration.Type))
        {
            return _predicate((TFact)factMatch.Value);
        }
        return false;
    }

    internal override string GetText()
    {
        return $"Fact {typeof(TFact)} where {_predicateExpression}";
    }
}

internal class EqualFactConstraint<T> : FactConstraint<T>
{
    private readonly T _factValue;

    public EqualFactConstraint(T factValue)
    {
        _factValue = factValue;
    }

    internal override bool IsSatisfied(IFactMatch factMatch)
    {
        if (typeof(T).IsAssignableFrom(factMatch.Declaration.Type))
        {
            return Equals(_factValue, factMatch.Value);
        }
        return false;
    }

    internal override string GetText()
    {
        return $"Fact {typeof(T)} equal to {_factValue}";
    }
}