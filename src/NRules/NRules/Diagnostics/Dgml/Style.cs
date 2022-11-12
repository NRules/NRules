using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Style : ICanWriteXml
{
    private readonly List<Condition> _conditions = new();
    private readonly List<Setter> _setters = new();

    public Style(string targetType)
    {
        TargetType = targetType;
    }

    public string TargetType { get; }
    public string? GroupLabel { get; set; }
    public string? ValueLabel { get; set; }

    public IReadOnlyCollection<Condition> Conditions => _conditions;

    public IReadOnlyCollection<Setter> Setters => _setters;

    public Style HasCategory(string category) => Condition($"HasCategory('{category}')");

    public Style Condition(string expression)
    {
        var condition = new Condition(expression);
        _conditions.Add(condition);
        return this;
    }

    public Style SetBackground(string value) => Setter(nameof(Node.Background), value);

    public Style Setter(string property, string value)
    {
        var setter = new Setter(property, value, null);
        _setters.Add(setter);
        return this;
    }

    public Style Expression(string property, string expression)
    {
        var setter = new Setter(property, null, expression);
        _setters.Add(setter);
        return this;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Style));

        writer.WriteAttributeString(nameof(TargetType), TargetType);
        writer.WriteAttributeIfNotNull(nameof(GroupLabel), GroupLabel);
        writer.WriteAttributeIfNotNull(nameof(ValueLabel), ValueLabel);

        writer.WriteMany(_conditions);
        writer.WriteMany(_setters);

        writer.WriteEndElement();
    }

    public async Task WriteXmlAsync(XmlWriter writer, CancellationToken cancellationToken)
    {
        await writer.WriteStartElementAsync(nameof(Style));

        await writer.WriteAttributeStringAsync(nameof(TargetType), TargetType);
        await writer.WriteAttributeIfNotNullAsync(nameof(GroupLabel), GroupLabel);
        await writer.WriteAttributeIfNotNullAsync(nameof(ValueLabel), ValueLabel);

        await writer.WriteManyAsync(_conditions, cancellationToken);
        await writer.WriteManyAsync(_setters, cancellationToken);

        await writer.WriteEndElementAsync();
    }
}