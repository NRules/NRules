using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Setter(string property)
{
    public string Property { get; } = property;
    public string? Value { get; set; }
    public string? Expression{ get; set; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Setter));
        writer.WriteAttributeString(nameof(Property), Property);
        writer.WriteAttributeIfNotNull(nameof(Value), Value);
        writer.WriteAttributeIfNotNull(nameof(Expression), Expression);
        writer.WriteEndElement();
    }
}