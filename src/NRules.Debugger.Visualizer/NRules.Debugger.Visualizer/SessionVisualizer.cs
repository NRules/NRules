using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.DebuggerVisualizers;
using NRules.Diagnostics;

namespace NRules.Debugger.Visualizer
{
    public class SessionVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var snapshot = (SessionSnapshot) objectProvider.GetObject();
            var dgmlWriter = new DgmlWriter(snapshot);
            string fileName = Path.Combine(Path.GetTempPath(), "session.dgml");
            using (var xmlWriter = XmlWriter.Create(fileName))
            {
                dgmlWriter.WriteTo(xmlWriter);
            }
            Process.Start(fileName);
        }
    }
}
