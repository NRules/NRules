using System.Xml;

namespace NRules.Diagnostics.Dgml;

internal interface ICanWriteXml
{
    void WriteXml(XmlWriter writer);
}
