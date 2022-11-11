using System;
using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Setter : ICanWriteXml
{
    public Setter(string property, string? value, string? expression)
    {
        Property = property ?? throw new ArgumentNullException(nameof(property));
        Value = value;
        Expression = expression;
    }

    public string Property { get; }
    public string? Value { get; }
    public string? Expression { get; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Setter));
        writer.WriteAttributeString(nameof(Property), Property);
        writer.WriteAttributeIfNotNull(Value);
        writer.WriteAttributeIfNotNull(Expression);
        writer.WriteEndElement();
    }
}