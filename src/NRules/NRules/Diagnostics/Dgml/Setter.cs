using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Setter
{
    public Setter(string property, string? value, string? expression)
    {
        Property = property;
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
        writer.WriteAttributeIfNotNull(nameof(Value), Value);
        writer.WriteAttributeIfNotNull(nameof(Expression), Expression);
        writer.WriteEndElement();
    }
}