using System.Collections.Generic;
using System.Xml;

namespace NRules.Diagnostics.Dgml
{
    internal static class XmlWriterExtensions
    {
        public static void WriteAttributeIfNotNull(this XmlWriter writer, string localName, object value)
        {
            if (value != null)
                writer.WriteAttributeString(localName, value.ToString());
        }

        public static void WriteProperties(this XmlWriter writer, Dictionary<string, object> properties)
        {
            foreach (var property in properties)
            {
                if (property.Value != null)
                    writer.WriteAttributeString(property.Key, property.Value.ToString());
            }
        }
    }
}
