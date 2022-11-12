using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal class Condition : ICanWriteXml
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

    public async Task WriteXmlAsync(XmlWriter writer, CancellationToken cancellationToken)
    {
        await writer.WriteStartElementAsync(nameof(Condition));
        await writer.WriteAttributeStringAsync(nameof(Expression), Expression);
        await writer.WriteEndElementAsync();
    }
}