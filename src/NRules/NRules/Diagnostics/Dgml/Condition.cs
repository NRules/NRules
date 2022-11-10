using System;
using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Condition
{
    public Condition(string expression)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public string Expression { get; set; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement(nameof(Condition));
        writer.WriteAttributeString(nameof(Expression), Expression);
        writer.WriteEndElement();
    }
}