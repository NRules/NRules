using System.Xml;

namespace NRules.Diagnostics.Dgml
{
    internal class Condition
    {
        public string Expression { get; set; }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Condition));
            writer.WriteAttributeString(nameof(Expression), Expression);
            writer.WriteEndElement();
        }
    }
}