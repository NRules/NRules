using System.Collections.Generic;
using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Style
{
    public Style(string targetType)
    {
        TargetType = targetType;
    }

    public string TargetType { get; }
    public string? GroupLabel { get; set; }
    public string? ValueLabel { get; set; }
    public List<Condition> Conditions { get; } = new();
    public List<Setter> Setters { get; } = new();

    public Style HasCategory(string category)
    {
        return Condition($"HasCategory('{category}')");
    }

    public Style Condition(string expression)
    {
        var condition = new Condition(expression);
        Conditions.Add(condition);
        return this;
    }

    public Style Setter(string property, string? value = null, string? expression = null)
    {
        var setter = new Setter(property) { Value = value, Expression = expression };
        Setters.Add(setter);
        return this;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Style));
        writer.WriteAttributeString(nameof(TargetType), TargetType);
        writer.WriteAttributeIfNotNull(nameof(GroupLabel), GroupLabel);
        writer.WriteAttributeIfNotNull(nameof(ValueLabel), ValueLabel);

        foreach (var condition in Conditions)
            condition.WriteXml(writer);

        foreach (var setter in Setters)
            setter.WriteXml(writer);

        writer.WriteEndElement();
    }
}