using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class DirectedGraph : ICanWriteXml
{
    private const string Namespace = "http://schemas.microsoft.com/vs/2009/dgml";
    private readonly List<Node> _nodes = new();
    private readonly List<Link> _links = new();
    private readonly List<Category> _categories = new();
    private readonly List<Style> _styles = new();
    private readonly List<Property> _properties = new();

    public DirectedGraph(string title, string? background = null)
    {
        Title = title;
        Background = background;
    }

    public string Title { get; }
    public string? Background { get; }

    public IReadOnlyCollection<Node> Nodes => _nodes;
    public IReadOnlyCollection<Link> Links => _links;
    public IReadOnlyCollection<Category> Categories => _categories;
    public IReadOnlyCollection<Style> Styles => _styles;
    public IReadOnlyCollection<Property> Properties => _properties;

    public void AddRange(IEnumerable<Node> items) => _nodes.AddRange(items);

    public void AddRange(IEnumerable<Link> items) => _links.AddRange(items);

    public void AddRange(IEnumerable<Category> items) => _categories.AddRange(items);

    public void AddRange(IEnumerable<Style> items) => _styles.AddRange(items);

    public void AddRange(IEnumerable<Property> items) => _properties.AddRange(items);

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(DirectedGraph), Namespace);

        writer.WriteAttributeIfNotNull(nameof(Title), Title);
        writer.WriteAttributeIfNotNull(nameof(Background), Background);

        writer.WriteXml(nameof(Nodes), Nodes);
        writer.WriteXml(nameof(Links), Links);

        if (Categories.Any())
        {
            writer.WriteXml(nameof(Categories), Categories);
        }

        if (Styles.Any())
        {
            writer.WriteXml(nameof(Styles), Styles);
        }

        if (_properties.Any())
        {
            writer.WriteXml(nameof(Properties), Properties);
        }

        writer.WriteEndElement();
    }

    public async Task WriteXmlAsync(XmlWriter writer, CancellationToken cancellationToken)
    {
        await writer.WriteStartElementAsync(nameof(DirectedGraph), Namespace);

        await writer.WriteAttributeIfNotNullAsync(nameof(Title), Title);
        await writer.WriteAttributeIfNotNullAsync(nameof(Background), Background);

        await writer.WriteXmlAsync(nameof(Nodes), Nodes, cancellationToken);
        await writer.WriteXmlAsync(nameof(Links), Links, cancellationToken);

        if (Categories.Any())
        {
            await writer.WriteXmlAsync(nameof(Categories), Categories, cancellationToken);
        }

        if (Styles.Any())
        {
            await writer.WriteXmlAsync(nameof(Styles), Styles, cancellationToken);
        }

        if (_properties.Any())
        {
            await writer.WriteXmlAsync(nameof(Properties), Properties, cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();

        await writer.WriteEndElementAsync();
    }
}