using System.Collections.Generic;
using System.Xml;

namespace NRules.Diagnostics.Dgml
{
    internal class Node
    {
        public Node(string id)
        {
            Id = id;
        }

        public string Id { get; }
        public string Label { get; set; }
        public string Category { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public string Background { get; set; }
        public string Foreground { get; set; }
        public string FontSize { get; set; }
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Node));
            writer.WriteAttributeString(nameof(Id), Id);
            writer.WriteAttributeIfNotNull(nameof(Label), Label);
            writer.WriteAttributeIfNotNull(nameof(Category), Category);
            writer.WriteAttributeIfNotNull(nameof(Group), Group);
            writer.WriteAttributeIfNotNull(nameof(Description), Description);
            writer.WriteAttributeIfNotNull(nameof(Reference), Reference);
            writer.WriteAttributeIfNotNull(nameof(Background), Background);
            writer.WriteAttributeIfNotNull(nameof(Foreground), Foreground);
            writer.WriteAttributeIfNotNull(nameof(FontSize), FontSize);
            writer.WriteProperties(Properties);
            writer.WriteEndElement();
        }
    }
}