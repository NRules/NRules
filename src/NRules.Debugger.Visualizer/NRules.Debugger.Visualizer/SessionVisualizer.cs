using System.Reflection;
using Microsoft.VisualStudio.DebuggerVisualizers;
using NRules.Debugger.Visualizer.Model;
using NRules.Diagnostics;

namespace NRules.Debugger.Visualizer
{
    public class SessionVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            //Preload WPFExtensions
            Assembly.Load(typeof (WPFExtensions.Controls.ZoomControl).Assembly.GetName());

            var snapshot = (SessionSnapshot) objectProvider.GetObject();
            var graph = ReteGraph.Create(snapshot);
            var viewModel = new VisualizerViewModel(graph);
            var visualizer = new VisualizerWindow{DataContext = viewModel};
            visualizer.ShowDialog();
        }
    }
}
