using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal static class XmlWriterExtensions
{
    public static void WriteAttributeIfNotNull<T>(this XmlWriter writer, T? value, [CallerArgumentExpression(nameof(value))] string localName = "")
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

    public static void WriteProperties(this XmlWriter writer, Dictionary<string, object?> properties)
    {
        foreach (var property in properties)
        {
            writer.WriteAttributeIfNotNull(property.Value, property.Key);
        }
    }

    public static void WriteXml<T>(this XmlWriter writer, IEnumerable<T> items, [CallerArgumentExpression(nameof(items))] string localName = "")
        where T : ICanWriteXml
    {
        writer.WriteStartElement(localName);
        writer.WriteMany(items);
        writer.WriteEndElement();
    }

    public static void WriteMany<T>(this XmlWriter writer, IEnumerable<T> items)
        where T : ICanWriteXml
    {
        foreach (var node in items)
            node.WriteXml(writer);
    }
}
