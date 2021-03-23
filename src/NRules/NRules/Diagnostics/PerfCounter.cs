using System;
using NRules.Rete;

namespace NRules.Diagnostics
{
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

    internal readonly struct AssertPerfCounter : IDisposable
    {
        private readonly NodeMetrics _nodeMetrics;
        private readonly long _startTicks;

        public AssertPerfCounter(NodeMetrics nodeMetrics)
        {
            _nodeMetrics = nodeMetrics;
            _startTicks = DateTime.UtcNow.Ticks;
        }

        public void Dispose()
        {
            _nodeMetrics.InsertDurationMilliseconds += (DateTime.UtcNow.Ticks - _startTicks) / TimeSpan.TicksPerMillisecond;
        }

        public void AddItems(int count)
        {
            AddInputs(count);
            AddOutputs(count);
        }

        public void AddInputs(int count) => _nodeMetrics.InsertInputCount += count;
        public void AddOutputs(int count) => _nodeMetrics.InsertOutputCount += count;
        public void SetCount(int count) => _nodeMetrics.ElementCount = count;
    }

    internal readonly struct UpdatePerfCounter : IDisposable
    {
        private readonly NodeMetrics _nodeMetrics;
        private readonly long _startTicks;

        public UpdatePerfCounter(NodeMetrics nodeMetrics)
        {
            _nodeMetrics = nodeMetrics;
            _startTicks = DateTime.UtcNow.Ticks;
        }

        public void Dispose()
        {
            _nodeMetrics.UpdateDurationMilliseconds += (DateTime.UtcNow.Ticks - _startTicks) / TimeSpan.TicksPerMillisecond;
        }

        public void AddItems(int count)
        {
            AddInputs(count);
            AddOutputs(count);
        }

        public void AddInputs(int count) => _nodeMetrics.UpdateInputCount += count;
        public void AddOutputs(int count) => _nodeMetrics.UpdateOutputCount += count;
        public void SetCount(int count) => _nodeMetrics.ElementCount = count;
    }

    internal readonly struct RetractPerfCounter : IDisposable
    {
        private readonly NodeMetrics _nodeMetrics;
        private readonly long _startTicks;

        public RetractPerfCounter(NodeMetrics nodeMetrics)
        {
            _nodeMetrics = nodeMetrics;
            _startTicks = DateTime.UtcNow.Ticks;
        }

        public void Dispose()
        {
            _nodeMetrics.RetractDurationMilliseconds += (DateTime.UtcNow.Ticks - _startTicks) / TimeSpan.TicksPerMillisecond;
        }

        public void AddItems(int count)
        {
            AddInputs(count);
            AddOutputs(count);
        }

        public void AddInputs(int count) => _nodeMetrics.RetractInputCount += count;
        public void AddOutputs(int count) => _nodeMetrics.RetractOutputCount += count;
        public void SetCount(int count) => _nodeMetrics.ElementCount = count;
    }
}