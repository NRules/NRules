using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using NRules.Debugger.Visualizer;

[assembly: AssemblyTitle("NRules.Debugger.Visualizer")]
[assembly: AssemblyDescription("Visual Studio Debugger Visualizer for NRules")]
[assembly: ComVisible(false)]

[assembly: DebuggerVisualizer(
    typeof(SessionVisualizer),
    typeof(SessionObjectSource),
    Target = typeof (NRules.Session),
    Description = "NRules Session Visualizer")]
