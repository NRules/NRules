using System;
using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Setter
{
    public Setter(string property, string? value, string? expression)
    {
        Property = property ?? throw new ArgumentNullException(nameof(property));
        Value = value;
        Expression = expression;
    }

    public string Property { get; init; }
    public string? Value { get; init; }
    public string? Expression { get; init; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Setter));
        writer.WriteAttributeString(nameof(Property), Property);
        writer.WriteAttributeIfNotNull(nameof(Value), Value);
        writer.WriteAttributeIfNotNull(nameof(Expression), Expression);
        writer.WriteEndElement();
    }
}