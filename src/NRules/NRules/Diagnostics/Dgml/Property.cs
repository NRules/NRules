using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Property : ICanWriteXml
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

    public async Task WriteXmlAsync(XmlWriter writer, CancellationToken cancellationToken)
    {
        await writer.WriteStartElementAsync(nameof(Property));
        await writer.WriteAttributeStringAsync(nameof(Id), Id);
        await writer.WriteAttributeStringAsync(nameof(DataType), DataType);
        await writer.WriteAttributeIfNotNullAsync(nameof(Label), Label);
        await writer.WriteAttributeIfNotNullAsync(nameof(Description), Description);
        await writer.WriteEndElementAsync();
    }
}