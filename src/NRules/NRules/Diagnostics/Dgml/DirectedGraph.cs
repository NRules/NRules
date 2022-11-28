using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class DirectedGraph
{
    private const string Namespace = "http://schemas.microsoft.com/vs/2009/dgml";

    private readonly List<Node> _nodes = new();
    private readonly List<Link> _links = new();
    private readonly List<Category> _categories = new();
    private readonly List<Style> _styles = new();
    private readonly List<Property> _properties = new();

    public string Title { get; set; }
    public string Background { get; set; }

    public IEnumerable<Node> Nodes => _nodes;
    public IEnumerable<Link> Links => _links;
    public IEnumerable<Category> Categories => _categories;
    public IEnumerable<Style> Styles => _styles;
    public IEnumerable<Property> Properties => _properties;

    public void AddRange(IEnumerable<Node> items)
    {
        _nodes.AddRange(items);
    }

    public void AddRange(IEnumerable<Link> items)
    {
        _links.AddRange(items);
    }

    public void AddRange(IEnumerable<Category> items)
    {
        _categories.AddRange(items);
    }

    public void AddRange(IEnumerable<Style> items)
    {
        _styles.AddRange(items);
    }

    public void AddRange(IEnumerable<Property> items)
    {
        _properties.AddRange(items);
    }

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