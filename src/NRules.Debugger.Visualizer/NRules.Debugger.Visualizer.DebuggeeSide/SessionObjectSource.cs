using System.IO;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.DebuggerVisualizers;
using NRules.Diagnostics;

namespace NRules.Debugger.Visualizer
{
    public class SessionObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            var session = (ISessionSnapshotProvider) target;
            var snapshot = session.GetSnapshot();
            var dgmlWriter = new DgmlWriter(snapshot);
            using (var stringWriter = new Utf8StringWriter())
            {
                var xmlWriter = new XmlTextWriter(stringWriter);
                xmlWriter.Formatting = Formatting.Indented;
                dgmlWriter.WriteTo(xmlWriter);
                var result = stringWriter.ToString();
                base.GetData(result, outgoingData);
            }
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}