using System.Collections.Generic;
using System.Xml;

namespace NRules.Diagnostics.Dgml
{
    internal class Link
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public string Category { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Link));
            writer.WriteAttributeString(nameof(Source), Source);
            writer.WriteAttributeString(nameof(Target), Target);
            writer.WriteAttributeIfNotNull(nameof(Category), Category);
            writer.WriteAttributeIfNotNull(nameof(Label), Label);
            writer.WriteAttributeIfNotNull(nameof(Description), Description);
            writer.WriteProperties(Properties);
            writer.WriteEndElement();
        }
    }
}