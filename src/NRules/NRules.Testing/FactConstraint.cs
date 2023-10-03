using NRules.RuleModel;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

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
    /// Called with the corresponding <see cref="IFactMatch"/> when all rule firing fact constraints are satisfied.
    /// </summary>
    /// <param name="factMatch"></param>
    internal virtual void OnSatisfied(IFactMatch factMatch)
    {
    }
}

/// <summary>
/// Represents a strongly-typed constraint on facts matched by a rule.
/// </summary>
public abstract class FactConstraint<T> : FactConstraint
{
    private List<Action<T>> _callbacks;

    /// <summary>
    /// Called with the corresponding fact when all rule firing fact constraints are satisfied.
    /// </summary>
    /// <param name="callback">The delegate to call.</param>
    public FactConstraint<T> Callback(Action<T> callback)
    {
        if (_callbacks == null)
        {
            _callbacks = new List<Action<T>>();
        }
        _callbacks.Add(callback);
        return this;
    }

    internal override void OnSatisfied(IFactMatch factMatch)
    {
        base.OnSatisfied(factMatch);
        if (_callbacks != null)
        {
            foreach (var callback in _callbacks)
            {
                callback.Invoke((T)factMatch.Value);
            }
        }
    }
}

internal class TypedFactConstraint<T> : FactConstraint<T>
{
    internal override bool IsSatisfied(IFactMatch factMatch)
    {
        return typeof(T).IsAssignableFrom(factMatch.Declaration.Type);
    }

    internal override string GetText()
    {
        return $"Fact {typeof(T)}";
    }
}

internal class PredicatedFactConstraint<T> : FactConstraint<T>
{
    private readonly Expression<Func<T, bool>> _predicateExpression;
    private readonly Func<T, bool> _predicate;

    public PredicatedFactConstraint(Expression<Func<T, bool>> predicateExpression)
    {
        _predicateExpression = predicateExpression;
        _predicate = predicateExpression.Compile();
    }

    internal override bool IsSatisfied(IFactMatch factMatch)
    {
        if (typeof(T).IsAssignableFrom(factMatch.Declaration.Type))
        {
            return _predicate((T)factMatch.Value);
        }
        return false;
    }

    internal override string GetText()
    {
        return $"Fact {typeof(T)} where {_predicateExpression}";
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