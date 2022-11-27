using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Condition
{
    public Condition(string expression)
    {
        Expression = expression;
    }

    public string Expression { get; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Condition));
        writer.WriteAttributeString(nameof(Expression), Expression);
        writer.WriteEndElement();
    }
}