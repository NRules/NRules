using System.IO;
using Microsoft.VisualStudio.DebuggerVisualizers;

namespace NRules.Debug.Visualizer
{
    public class SessionObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            var session = (Session) target;
            var snapshot = session.GetSnapshot();
            base.GetData(snapshot, outgoingData);
        }
    }
}