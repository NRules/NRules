using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel;

/// <summary>
/// Ordered readonly collection of named expressions.
/// </summary>
public class ExpressionCollection : IReadOnlyList<NamedExpressionElement>
{
    private readonly NamedExpressionElement[] _expressions;

    internal ExpressionCollection(NamedExpressionElement[] expressions)
    {
        _expressions = expressions;
    }

    /// <summary>
    /// Number of expressions in the collection.
    /// </summary>
    public int Count => _expressions.Length;

    /// <summary>
    /// Retrieves single expression by name.
    /// </summary>
    /// <param name="name">Expression name.</param>
    /// <returns>Matching expression.</returns>
    public NamedExpressionElement this[string name]
    {
        get
        {
            var result = FindSingleOrDefault(name);
            if (result == null)
            {
                throw new ArgumentException(
                    $"Expression with the given name not found. Name={name}", nameof(name));
            }
            return result;
        }
    }

    /// <summary>
    /// Retrieves expressions by name.
    /// </summary>
    /// <param name="name">Expression name.</param>
    /// <returns>Matching expression or empty IEnumerable.</returns>
    public IEnumerable<NamedExpressionElement> Find(string name)
    {
        return _expressions.Where(e => e.Name == name);
    }

    /// <summary>
    /// Retrieves expression by index.
    /// </summary>
    /// <param name="index">Expression index.</param>
    public NamedExpressionElement this[int index] => _expressions[index];

    /// <summary>
    /// Retrieves single expression by name.
    /// </summary>
    /// <param name="name">Expression name.</param>
    /// <returns>Matching expression or <c>null</c>.</returns>
    public NamedExpressionElement FindSingleOrDefault(string name)
    {
        return Find(name).SingleOrDefault();
    }

    /// <summary>
    /// Returns an enumerator for the contained expression elements.
    /// </summary>
    public IEnumerator<NamedExpressionElement> GetEnumerator()
    {
        return _expressions.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public ExpressionCollection Update(IReadOnlyList<NamedExpressionElement> expressions)
    {
        if (ReferenceEquals(expressions, this)) return this;
        return new ExpressionCollection(expressions.AsArray());
    }
}