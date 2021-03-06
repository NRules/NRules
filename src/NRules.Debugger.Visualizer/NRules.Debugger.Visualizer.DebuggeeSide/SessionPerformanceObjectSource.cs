using System.IO;
using Microsoft.VisualStudio.DebuggerVisualizers;
using NRules.Diagnostics.Dgml;

namespace NRules.Debugger.Visualizer
{
    public class SessionPerformanceObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            var session = (ISession) target;
            var schema = session.GetSchema();
            var dgmlWriter = new DgmlWriter(schema);
            dgmlWriter.SetMetricsProvider(session.Metrics);
            var contents = dgmlWriter.GetContents();
            base.GetData(contents, outgoingData);
        }
    }
}