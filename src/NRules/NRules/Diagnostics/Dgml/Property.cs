using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Property
{
    public Property(string id, string dataType)
    {
        Id = id;
        DataType = dataType;
    }

    public string Id { get; }
    public string DataType { get; }
    public string? Label { get; set; }
    public string? Description { get; set; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Property));
        writer.WriteAttributeString(nameof(Id), Id);
        writer.WriteAttributeString(nameof(DataType), DataType);
        writer.WriteAttributeIfNotNull(nameof(Label), Label);
        writer.WriteAttributeIfNotNull(nameof(Description), Description);
        writer.WriteEndElement();
    }
}