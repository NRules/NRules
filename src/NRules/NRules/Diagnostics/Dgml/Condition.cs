using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Condition(string expression)
{
    public string Expression { get; } = expression;

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Condition));
        writer.WriteAttributeString(nameof(Expression), Expression);
        writer.WriteEndElement();
    }
}