using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Node : ICanWriteXml
{
    private readonly Dictionary<string, object?> _properties = new();

    public Node(int id, string label, string category)
    {
        Id = id;
        Label = label;
        Category = category;
    }

    public int Id { get; }
    public string Label { get; }
    public string Category { get; }
    public string? Group { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public string? Background { get; set; }
    public string? Foreground { get; set; }
    public string? FontSize { get; set; }
    public IReadOnlyDictionary<string, object?> Properties => _properties;
    public void AddProperty(string name, object? value) => _properties.Add(name, value);

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Node));
        writer.WriteAttributeString(nameof(Id), Id.ToString());
        writer.WriteAttributeIfNotNull(nameof(Label), Label);
        writer.WriteAttributeIfNotNull(nameof(Category), Category);
        writer.WriteAttributeIfNotNull(nameof(Group), Group);
        writer.WriteAttributeIfNotNull(nameof(Description), Description);
        writer.WriteAttributeIfNotNull(nameof(Reference), Reference);
        writer.WriteAttributeIfNotNull(nameof(Background), Background);
        writer.WriteAttributeIfNotNull(nameof(Foreground), Foreground);
        writer.WriteAttributeIfNotNull(nameof(FontSize), FontSize);
        writer.WriteProperties(Properties);
        writer.WriteEndElement();
    }

    public async Task WriteXmlAsync(XmlWriter writer, CancellationToken cancellationToken)
    {
        await writer.WriteStartElementAsync(nameof(Node));
        await writer.WriteAttributeStringAsync(nameof(Id), Id.ToString());
        await writer.WriteAttributeIfNotNullAsync(nameof(Label), Label);
        await writer.WriteAttributeIfNotNullAsync(nameof(Category), Category);
        await writer.WriteAttributeIfNotNullAsync(nameof(Group), Group);
        await writer.WriteAttributeIfNotNullAsync(nameof(Description), Description);
        await writer.WriteAttributeIfNotNullAsync(nameof(Reference), Reference);
        await writer.WriteAttributeIfNotNullAsync(nameof(Background), Background);
        await writer.WriteAttributeIfNotNullAsync(nameof(Foreground), Foreground);
        await writer.WriteAttributeIfNotNullAsync(nameof(FontSize), FontSize);
        await writer.WritePropertiesAsync(Properties, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        await writer.WriteEndElementAsync();
    }
}
