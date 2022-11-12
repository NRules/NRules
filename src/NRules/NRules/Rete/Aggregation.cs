using NRules.Aggregators;

namespace NRules.Rete;

internal class Aggregation
{
    private readonly List<AggregateList> _aggregateLists = new();
    private AggregateList? _currentList;

    public IEnumerable<AggregateList> AggregateLists => _aggregateLists;
    public int Count { get; private set; }

    public void Add(Tuple tuple, Fact aggregateFact)
    {
        var list = GetList(AggregationAction.Added);
        list.Add(tuple, aggregateFact);
        Count++;
    }

    public void Modify(Tuple tuple, Fact aggregateFact)
    {
        var list = GetList(AggregationAction.Modified);
        list.Add(tuple, aggregateFact);
        Count++;
    }

    public void Remove(Tuple tuple, Fact aggregateFact)
    {
        var list = GetList(AggregationAction.Removed);
        list.Add(tuple, aggregateFact);
        Count++;
    }

    public void Remove(Tuple tuple, IReadOnlyCollection<Fact> aggregateFacts)
    {
        if (aggregateFacts.Count == 0)
            return;

        var list = GetList(AggregationAction.Removed);
        foreach (var aggregateFact in aggregateFacts)
        {
            list.Add(tuple, aggregateFact);
            Count++;
        }
    }

    private AggregateList GetList(AggregationAction action)
    {
        if (_currentList?.Action == action)
            return _currentList;

        _currentList = new AggregateList(action);
        _aggregateLists.Add(_currentList);
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