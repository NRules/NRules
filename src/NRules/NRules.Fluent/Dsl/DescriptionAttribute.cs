using System;

namespace NRules.Fluent.Dsl;

/// <summary>
/// Sets rule's description.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DescriptionAttribute(string value) : Attribute
{
    internal string Value { get; } = value;
}