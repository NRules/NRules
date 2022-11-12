using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Link : ICanWriteXml
{
    private readonly Dictionary<string, object?> _properties = new();

    public Link(int source, int target)
    {
        Source = source;
        Target = target;
    }

    public int Source { get; }
    public int Target { get; }
    public string? Category { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
    public string? StrokeThickness { get; set; }
    public IReadOnlyDictionary<string, object?> Properties => _properties;

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Link));
        writer.WriteAttributeString(nameof(Source), Source.ToString());
        writer.WriteAttributeString(nameof(Target), Target.ToString());
        writer.WriteAttributeIfNotNull(nameof(Category), Category);
        writer.WriteAttributeIfNotNull(nameof(Label), Label);
        writer.WriteAttributeIfNotNull(nameof(Description), Description);
        writer.WriteAttributeIfNotNull(nameof(StrokeThickness), StrokeThickness);
        writer.WriteProperties(Properties);
        writer.WriteEndElement();
    }

    public async Task WriteXmlAsync(XmlWriter writer, CancellationToken cancellationToken)
    {
        await writer.WriteStartElementAsync(nameof(Link));
        await writer.WriteAttributeStringAsync(nameof(Source), Source.ToString());
        await writer.WriteAttributeStringAsync(nameof(Target), Target.ToString());
        await writer.WriteAttributeIfNotNullAsync(nameof(Category), Category);
        await writer.WriteAttributeIfNotNullAsync(nameof(Label), Label);
        await writer.WriteAttributeIfNotNullAsync(nameof(Description), Description);
        await writer.WriteAttributeIfNotNullAsync(nameof(StrokeThickness), StrokeThickness);
        await writer.WritePropertiesAsync(Properties, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        await writer.WriteEndElementAsync();
    }
}