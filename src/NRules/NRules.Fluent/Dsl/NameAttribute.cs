using System;

namespace NRules.Fluent.Dsl;

/// <summary>
/// Sets rule's name.
/// Name set via the attribute overrides the default name, which is the fully qualified class name.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class NameAttribute(string value) : Attribute
{
    internal string Value { get; } = value;
}