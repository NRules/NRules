using System.IO;
using Microsoft.VisualStudio.DebuggerVisualizers;
using NRules.Diagnostics;
using NRules.Diagnostics.Dgml;

namespace NRules.Debugger.Visualizer
{
    public class SessionObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            var session = (ISessionSchemaProvider) target;
            var schema = session.GetSchema();
            var dgmlWriter = new DgmlWriter(schema);
            var contents = dgmlWriter.GetContents();
            base.GetData(contents, outgoingData);
        }
    }
}