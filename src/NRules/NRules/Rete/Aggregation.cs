using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class Aggregation
    {
        private readonly List<AggregationEntry> _asserts = new List<AggregationEntry>();
        private readonly List<AggregationEntry> _updates = new List<AggregationEntry>();
        private readonly List<AggregationEntry> _retracts = new List<AggregationEntry>();

        public IList<AggregationEntry> Asserts
        {
            get { return _asserts; }
        }

        public IList<AggregationEntry> Updates
        {
            get { return _updates; }
        }

        public IList<AggregationEntry> Retracts
        {
            get { return _retracts; }
        }

        public void Add(Tuple tuple, IEnumerable<AggregationResult> results)
        {
            foreach (var result in results)
            {
                Add(tuple, result);
            }
        }

        public void Add(Tuple tuple, AggregationResult result)
        {
            switch (result.Action)
            {
                case AggregationAction.Added:
                    _asserts.Add(new AggregationEntry(tuple, result.Aggregate));
                    break;
                case AggregationAction.Modified:
                    _updates.Add(new AggregationEntry(tuple, result.Aggregate));
                    break;
                case AggregationAction.Removed:
                    _retracts.Add(new AggregationEntry(tuple, result.Aggregate));
                    break;
            }
        }
    }

    internal class AggregationEntry
    {
        public Tuple Tuple { get; private set; }
        public object ResultObject { get; private set; }

        public AggregationEntry(Tuple tuple, object resultObject)
        {
            Tuple = tuple;
            ResultObject = resultObject;
        }
    }
}