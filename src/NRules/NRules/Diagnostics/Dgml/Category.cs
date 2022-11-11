using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Category : ICanWriteXml
{
    public Category(string id)
    {
        Id = id;
    }

    public string Id { get; }
    public string? Label { get; set; }
    public string? Background { get; set; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Category));
        writer.WriteAttributeString(nameof(Id), Id);
        writer.WriteAttributeIfNotNull(Label);
        writer.WriteAttributeIfNotNull(Background);
        writer.WriteEndElement();
    }
}