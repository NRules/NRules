using System.Collections.Generic;
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
        writer.WriteAttributeIfNotNull(GroupLabel);
        writer.WriteAttributeIfNotNull(ValueLabel);

        writer.WriteMany(_conditions);
        writer.WriteMany(_setters);

        writer.WriteEndElement();
    }
}