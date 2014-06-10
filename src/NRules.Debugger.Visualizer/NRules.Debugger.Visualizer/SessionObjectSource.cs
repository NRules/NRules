using System.IO;
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
            base.GetData(snapshot, outgoingData);
        }
    }
}