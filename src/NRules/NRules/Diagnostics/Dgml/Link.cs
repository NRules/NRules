using System.Collections.Generic;
using System.Xml;

namespace NRules.Diagnostics.Dgml
{
    internal class Link
    {
        public Link(string source, string target)
        {
            Source = source;
            Target = target;
        }

        public string Source { get; }
        public string Target { get; }
        public string Category { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string StrokeThickness { get; set; }
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Link));
            writer.WriteAttributeString(nameof(Source), Source);
            writer.WriteAttributeString(nameof(Target), Target);
            writer.WriteAttributeIfNotNull(nameof(Category), Category);
            writer.WriteAttributeIfNotNull(nameof(Label), Label);
            writer.WriteAttributeIfNotNull(nameof(Description), Description);
            writer.WriteAttributeIfNotNull(nameof(StrokeThickness), StrokeThickness);
            writer.WriteProperties(Properties);
            writer.WriteEndElement();
        }
    }
}