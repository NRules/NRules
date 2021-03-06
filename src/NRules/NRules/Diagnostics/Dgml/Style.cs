using System.Collections.Generic;
using System.Xml;

namespace NRules.Diagnostics.Dgml
{
    internal class Style
    {
        public string TargetType { get; set; }
        public string GroupLabel { get; set; }
        public string ValueLabel { get; set; }
        public List<Condition> Conditions { get; set; }
        public List<Setter> Setters { get; set; }

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
}