using System;
using NRules.Rete;

namespace NRules.Diagnostics;

internal static class PerfCounter
{
    public static AssertPerfCounter Assert(IExecutionContext context, INode node)
    {
        var nodeMetrics = context.MetricsAggregator.GetMetrics(node);
        return new AssertPerfCounter(nodeMetrics);
    }
    
    public static UpdatePerfCounter Update(IExecutionContext context, INode node)
    {
        var nodeMetrics = context.MetricsAggregator.GetMetrics(node);
        return new UpdatePerfCounter(nodeMetrics);
    }
    
    public static RetractPerfCounter Retract(IExecutionContext context, INode node)
    {
        var nodeMetrics = context.MetricsAggregator.GetMetrics(node);
        return new RetractPerfCounter(nodeMetrics);
    }
}

internal readonly struct AssertPerfCounter(NodeMetrics nodeMetrics) : IDisposable
{
    private readonly long _startTicks = Environment.TickCount;

    public void Dispose()
    {
        nodeMetrics.InsertDurationMilliseconds += unchecked(Environment.TickCount - _startTicks);
    }

    public void AddItems(int count)
    {
        AddInputs(count);
        AddOutputs(count);
    }

    public void AddInputs(int count) => nodeMetrics.InsertInputCount += count;
    public void AddOutputs(int count) => nodeMetrics.InsertOutputCount += count;
    public void SetCount(int count) => nodeMetrics.ElementCount = count;
}

internal readonly struct UpdatePerfCounter(NodeMetrics nodeMetrics) : IDisposable
{
    private readonly long _startTicks = Environment.TickCount;

    public void Dispose()
    {
        nodeMetrics.UpdateDurationMilliseconds += unchecked(Environment.TickCount - _startTicks);
    }

    public void AddItems(int count)
    {
        AddInputs(count);
        AddOutputs(count);
    }

    public void AddInputs(int count) => nodeMetrics.UpdateInputCount += count;
    public void AddOutputs(int count) => nodeMetrics.UpdateOutputCount += count;
    public void SetCount(int count) => nodeMetrics.ElementCount = count;
}

internal readonly struct RetractPerfCounter(NodeMetrics nodeMetrics) : IDisposable
{
    private readonly long _startTicks = Environment.TickCount;

    public void Dispose()
    {
        nodeMetrics.RetractDurationMilliseconds += unchecked(Environment.TickCount - _startTicks);
    }

    public void AddItems(int count)
    {
        AddInputs(count);
        AddOutputs(count);
    }

    public void AddInputs(int count) => nodeMetrics.RetractInputCount += count;
    public void AddOutputs(int count) => nodeMetrics.RetractOutputCount += count;
    public void SetCount(int count) => nodeMetrics.ElementCount = count;
}