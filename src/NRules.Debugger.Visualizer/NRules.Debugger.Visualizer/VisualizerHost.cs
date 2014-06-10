using Microsoft.VisualStudio.DebuggerVisualizers;

namespace NRules.Debugger.Visualizer
{
    public class VisualizerHost : VisualizerDevelopmentHost
    {
        internal VisualizerHost(ISession objectToVisualize)
            : base(objectToVisualize, typeof (SessionVisualizer), typeof (SessionObjectSource))
        {
        }

        public static void Visualize(ISession session)
        {
            var host = new VisualizerHost(session);
            host.ShowVisualizer();
        }
    }
}