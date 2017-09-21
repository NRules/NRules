using System.Collections.Generic;
using NRules.Aggregators;

namespace NRules.Rete
{
    internal class Aggregation
    {
        private readonly List<AggregateList> _aggregateLists = new List<AggregateList>();
        private AggregateList _currentList;

        public IList<AggregateList> AggregateLists => _aggregateLists;

        public void Add(AggregationAction action, Tuple tuple, Fact aggregateFact)
        {
            var list = GetList(action);
            list.Add(tuple, aggregateFact);
        }

        public void AddAssert(Tuple tuple, Fact aggregateFact)
        {
            var list = GetList(AggregationAction.Added);
            list.Add(tuple, aggregateFact);
        }

        public void AddUpdate(Tuple tuple, Fact aggregateFact)
        {
            var list = GetList(AggregationAction.Modified);
            list.Add(tuple, aggregateFact);
        }
        
        public void AddRetract(Tuple tuple, Fact aggregateFact)
        {
            var list = GetList(AggregationAction.Removed);
            list.Add(tuple, aggregateFact);
        }

        private AggregateList GetList(AggregationAction action)
        {
            if (_currentList == null || _currentList.Action != action)
            {
                _currentList = new AggregateList(action);
                _aggregateLists.Add(_currentList);
            }
            return _currentList;
        }
    }

    internal class AggregateList : TupleFactList
    {
        public AggregationAction Action { get; }

        public AggregateList(AggregationAction action)
        {
            Action = action;
        }
    }
}