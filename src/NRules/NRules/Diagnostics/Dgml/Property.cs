using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Property(string id)
{
    public string Id { get; } = id;
    public string? DataType { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Property));
        writer.WriteAttributeString(nameof(Id), Id);
        writer.WriteAttributeIfNotNull(nameof(DataType), DataType);
        writer.WriteAttributeIfNotNull(nameof(Label), Label);
        writer.WriteAttributeIfNotNull(nameof(Description), Description);
        writer.WriteEndElement();
    }
}