﻿using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.DebuggerVisualizers;
using NRules.Debugger.Visualizer;

[assembly: DebuggerVisualizer(
    typeof(SessionVisualizer),
    "NRules.Debugger.Visualizer.SessionObjectSource, NRules.Debugger.Visualizer.DebuggeeSide",
    TargetTypeName = "NRules.SessionFactory, NRules",
    Description = "NRules Session Factory Visualizer")]
[assembly: DebuggerVisualizer(
    typeof(SessionVisualizer),
    "NRules.Debugger.Visualizer.SessionObjectSource, NRules.Debugger.Visualizer.DebuggeeSide",
    TargetTypeName = "NRules.Session, NRules",
    Description = "NRules Session Visualizer")]
[assembly: DebuggerVisualizer(
    typeof(SessionVisualizer),
    "NRules.Debugger.Visualizer.SessionPerformanceObjectSource, NRules.Debugger.Visualizer.DebuggeeSide",
    TargetTypeName = "NRules.Session, NRules",
    Description = "NRules Session Performance Visualizer")]

namespace NRules.Debugger.Visualizer;

public class SessionVisualizer : DialogDebuggerVisualizer
{
    public SessionVisualizer()
        : base(FormatterPolicy.Legacy)
    {
    }

    protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
    {
        var stream = objectProvider.GetData();
        using var streamReader = new StreamReader(stream);
        var snapshot = streamReader.ReadToEnd();
        string fileName = Path.Combine(Path.GetTempPath(), "session.dgml");
        File.WriteAllText(fileName, snapshot);
        Process.Start(fileName);
    }
}
