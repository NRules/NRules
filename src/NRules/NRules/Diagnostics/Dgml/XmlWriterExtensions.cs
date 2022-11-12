using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal static class XmlWriterExtensions
{
    public static void WriteProperties(this XmlWriter writer, IReadOnlyDictionary<string, object?> properties)
    {
        foreach (var property in properties)
        {
            writer.WriteAttributeIfNotNull(property.Key, property.Value);
        }
    }

    public static async Task WritePropertiesAsync(this XmlWriter writer, IReadOnlyDictionary<string, object?> properties, CancellationToken cancellationToken)
    {
        foreach (var property in properties)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            await writer.WriteAttributeIfNotNullAsync(property.Key, property.Value);
        }
    }

    public static void WriteAttributeIfNotNull<T>(this XmlWriter writer, string localName, T? value)
    {
        switch (value)
        {
            case string text:
                writer.WriteAttributeString(localName, text);
                break;
            case not null:
                writer.WriteAttributeString(localName, value.ToString());
                break;
        }
    }

    public static Task WriteAttributeIfNotNullAsync<T>(this XmlWriter writer, string localName, T? value) =>
        value switch
        {
            string text => writer.WriteAttributeStringAsync(localName, text),
            not null => writer.WriteAttributeStringAsync(localName, value.ToString()),
            _ => Task.CompletedTask,
        };

    public static Task WriteAttributeStringAsync(this XmlWriter writer, string localName, string value) =>
        writer.WriteAttributeStringAsync(null, localName, null, value);

    public static void WriteXml<T>(this XmlWriter writer, string localName, IEnumerable<T> items)
        where T : ICanWriteXml
    {
        writer.WriteStartElement(localName);
        writer.WriteMany(items);
        writer.WriteEndElement();
    }

    public static async Task WriteXmlAsync<T>(this XmlWriter writer, string localName, IEnumerable<T> items, CancellationToken cancellationToken)
        where T : ICanWriteXml
    {
        await writer.WriteStartElementAsync(localName);
        await writer.WriteManyAsync(items, cancellationToken);
        await writer.WriteEndElementAsync();
    }

    public static Task WriteStartElementAsync(this XmlWriter writer, string localName, string? ns = null) =>
        writer.WriteStartElementAsync(null, localName, ns);

    public static void WriteMany<T>(this XmlWriter writer, IEnumerable<T> items)
        where T : ICanWriteXml
    {
        foreach (var node in items)
            node.WriteXml(writer);
    }

    public static async Task WriteManyAsync<T>(this XmlWriter writer, IEnumerable<T> items, CancellationToken cancellationToken)
        where T : ICanWriteXml
    {
        foreach (var node in items)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            await node.WriteXmlAsync(writer, cancellationToken);
        }
    }
}
