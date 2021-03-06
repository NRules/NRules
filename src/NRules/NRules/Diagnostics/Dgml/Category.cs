using System.Xml;

namespace NRules.Diagnostics.Dgml
{
    internal class Category
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Background { get; set; }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Category));
            writer.WriteAttributeString(nameof(Id), Id);
            writer.WriteAttributeIfNotNull(nameof(Label), Label);
            writer.WriteAttributeIfNotNull(nameof(Background), Background);
            writer.WriteEndElement();
        }
    }
}