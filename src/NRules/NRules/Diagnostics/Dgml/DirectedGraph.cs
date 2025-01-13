using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class DirectedGraph
{
    private const string Namespace = @"http://schemas.microsoft.com/vs/2009/dgml";

    public string? Title { get; set; }
    public string? Background { get; set; }

    public List<Node> Nodes { get; set; } = new();
    public List<Link> Links { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Style> Styles { get; set; } = new();
    public List<Property> Properties { get; set; } = new();

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(DirectedGraph), Namespace);
        writer.WriteAttributeIfNotNull(nameof(Title), Title);
        writer.WriteAttributeIfNotNull(nameof(Background), Background);

        writer.WriteStartElement(nameof(Nodes));
        foreach (var node in Nodes)
            node.WriteXml(writer);
        writer.WriteEndElement();

        writer.WriteStartElement(nameof(Links));
        foreach (var link in Links)
            link.WriteXml(writer);
        writer.WriteEndElement();

        if (Categories.Any())
        {
            writer.WriteStartElement(nameof(Categories));
            foreach (var category in Categories)
                category.WriteXml(writer);
            writer.WriteEndElement();
        }

        if (Styles.Any())
        {
            writer.WriteStartElement(nameof(Styles));
            foreach (var style in Styles)
                style.WriteXml(writer);
            writer.WriteEndElement();
        }

        if (Properties.Any())
        {
            writer.WriteStartElement(nameof(Properties));
            foreach (var property in Properties)
                property.WriteXml(writer);
            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }
}