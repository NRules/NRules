﻿using System.Diagnostics;

namespace NRules.RuleModel;

/// <summary>
/// Rule element declaration.
/// </summary>
[DebuggerDisplay("{Name}: {Type}")]
public sealed class Declaration : IEquatable<Declaration>
{
    internal Declaration(Type type, string name)
    {
        Type = type;
        Name = name;
    }

    /// <summary>
    /// Symbol name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Symbol type.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Rule element that this declaration is referencing.
    /// </summary>
    public RuleElement? Target { get; internal set; }

    public bool Equals(Declaration? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return string.Equals(Name, other.Name);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((Declaration)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}