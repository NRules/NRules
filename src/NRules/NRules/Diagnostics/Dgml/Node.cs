using System.Collections.Generic;
using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Node : ICanWriteXml
{
    public Node(string id, string label, string category)
    {
        Id = id;
        Label = label;
        Category = category;
    }

    public string Id { get; }
    public string Label { get; }
    public string Category { get; }
    public string? Group { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public string? Background { get; set; }
    public string? Foreground { get; set; }
    public string? FontSize { get; set; }
    public Dictionary<string, object?> Properties { get; } = new();

    public void AddProperty(string name, object? value) => Properties.Add(name, value);

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Node));
        writer.WriteAttributeString(nameof(Id), Id);
        writer.WriteAttributeIfNotNull(Label);
        writer.WriteAttributeIfNotNull(Category);
        writer.WriteAttributeIfNotNull(Group);
        writer.WriteAttributeIfNotNull(Description);
        writer.WriteAttributeIfNotNull(Reference);
        writer.WriteAttributeIfNotNull(Background);
        writer.WriteAttributeIfNotNull(Foreground);
        writer.WriteAttributeIfNotNull(FontSize);
        writer.WriteProperties(Properties);
        writer.WriteEndElement();
    }
}
