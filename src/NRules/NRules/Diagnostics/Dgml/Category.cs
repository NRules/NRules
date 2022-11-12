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
        writer.WriteAttributeIfNotNull(nameof(Label), Label);
        writer.WriteAttributeIfNotNull(nameof(Background), Background);
        writer.WriteEndElement();
    }

    public async Task WriteXmlAsync(XmlWriter writer, CancellationToken cancellationToken)
    {
        await writer.WriteStartElementAsync(nameof(Category));
        await writer.WriteAttributeStringAsync(nameof(Id), Id);
        await writer.WriteAttributeIfNotNullAsync(nameof(Label), Label);
        await writer.WriteAttributeIfNotNullAsync(nameof(Background), Background);
        await writer.WriteEndElementAsync();
    }
}