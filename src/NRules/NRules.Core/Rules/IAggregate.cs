namespace NRules.Core.Rules
{
    internal enum AggregationResults
    {
        None = 0,
        Added = 1,
        Modified = 2,
        Removed = 3,
    }

    internal interface IAggregate
    {
        AggregationResults Add(object fact);
        AggregationResults Modify(object fact);
        AggregationResults Remove(object fact);
        object Result { get; }
    }
}