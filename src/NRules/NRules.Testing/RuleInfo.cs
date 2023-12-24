using System;
using NRules.RuleModel;

namespace NRules.Testing;

/// <summary>
/// Information about a rule under test.
/// </summary>
public struct RuleInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RuleInfo"/> class.
    /// </summary>
    /// <param name="type">CLR type that contains rule's definition using Fluent DSL.</param>
    /// <param name="instance">Instance of the rule's type.</param>
    /// <param name="definition">Rule's definition in a canonical form.</param>
    public RuleInfo(Type type, object instance, IRuleDefinition definition)
    {
        Type = type;
        Instance = instance;
        Definition = definition;
    }

    /// <summary>
    /// CLR type that contains rule's definition using Fluent DSL.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Instance of the rule's type.
    /// </summary>
    public object Instance { get; }

    /// <summary>
    /// Rule's definition.
    /// </summary>
    public IRuleDefinition Definition { get; }
}
