using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NRules.Debug.Visualizer")]
[assembly: AssemblyDescription("")]
[assembly: ComVisible(false)]
//[assembly: CLSCompliant(true)]
//[assembly: AllowPartiallyTrustedCallers]

[assembly: DebuggerVisualizer(
    typeof(NRules.Debug.Visualizer.SessionVisualizer),
    typeof(NRules.Debug.Visualizer.SessionObjectSource),
    Target = typeof (NRules.Session),
    Description = "NRules Session Visualizer")]