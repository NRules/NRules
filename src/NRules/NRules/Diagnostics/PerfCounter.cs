using System;
using System.Diagnostics;
using NRules.Rete;

namespace NRules.Diagnostics
{
    internal static class PerfCounter
    {
        public static AssertPerfCounter Assert(IExecutionContext context, INode node)
        {
            var nodeMetrics = context.MetricsAggregator.GetMetrics(node);
            return new AssertPerfCounter(nodeMetrics, context.MetricsAggregator.Stopwatch);
        }
        
        public static UpdatePerfCounter Update(IExecutionContext context, INode node)
        {
            var nodeMetrics = context.MetricsAggregator.GetMetrics(node);
            return new UpdatePerfCounter(nodeMetrics, context.MetricsAggregator.Stopwatch);
        }
        
        public static RetractPerfCounter Retract(IExecutionContext context, INode node)
        {
            var nodeMetrics = context.MetricsAggregator.GetMetrics(node);
            return new RetractPerfCounter(nodeMetrics, context.MetricsAggregator.Stopwatch);
        }
    }

    internal readonly struct AssertPerfCounter : IDisposable
    {
        private readonly NodeMetrics _nodeMetrics;
        private readonly Stopwatch _stopwatch;

        public AssertPerfCounter(NodeMetrics nodeMetrics, Stopwatch stopwatch)
        {
            _nodeMetrics = nodeMetrics;
            _stopwatch = stopwatch;
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _nodeMetrics.InsertDurationMilliseconds += _stopwatch.ElapsedMilliseconds;
            _stopwatch.Reset();
        }

        public void AddItems(int count) => _nodeMetrics.InsertCount += count;
        public void SetCount(int count) => _nodeMetrics.ElementCount = count;
    }

    internal readonly struct UpdatePerfCounter : IDisposable
    {
        private readonly NodeMetrics _nodeMetrics;
        private readonly Stopwatch _stopwatch;

        public UpdatePerfCounter(NodeMetrics nodeMetrics, Stopwatch stopwatch)
        {
            _nodeMetrics = nodeMetrics;
            _stopwatch = stopwatch;
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _nodeMetrics.UpdateDurationMilliseconds += _stopwatch.ElapsedMilliseconds;
            _stopwatch.Reset();
        }

        public void AddItems(int count) => _nodeMetrics.UpdateCount += count;
        public void SetCount(int count) => _nodeMetrics.ElementCount = count;
    }

    internal readonly struct RetractPerfCounter : IDisposable
    {
        private readonly NodeMetrics _nodeMetrics;
        private readonly Stopwatch _stopwatch;

        public RetractPerfCounter(NodeMetrics nodeMetrics, Stopwatch stopwatch)
        {
            _nodeMetrics = nodeMetrics;
            _stopwatch = stopwatch;
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _nodeMetrics.RetractDurationMilliseconds += _stopwatch.ElapsedMilliseconds;
            _stopwatch.Reset();
        }

        public void AddItems(int count) => _nodeMetrics.RetractCount += count;
        public void SetCount(int count) => _nodeMetrics.ElementCount = count;
    }
}