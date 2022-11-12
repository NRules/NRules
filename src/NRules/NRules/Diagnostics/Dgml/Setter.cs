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
        writer.WriteAttributeIfNotNull(nameof(Value), Value);
        writer.WriteAttributeIfNotNull(nameof(Expression), Expression);
        writer.WriteEndElement();
    }

    public async Task WriteXmlAsync(XmlWriter writer, CancellationToken cancellationToken)
    {
        await writer.WriteStartElementAsync(nameof(Setter));
        await writer.WriteAttributeStringAsync(nameof(Property), Property);
        await writer.WriteAttributeIfNotNullAsync(nameof(Value), Value);
        await writer.WriteAttributeIfNotNullAsync(nameof(Expression), Expression);
        await writer.WriteEndElementAsync();
    }
}