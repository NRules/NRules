namespace NRules.Rule
{
    public enum AggregationResults
    {
        None = 0,
        Added = 1,
        Modified = 2,
        Removed = 3,
    }

    public interface IAggregate
    {
        AggregationResults Add(object fact);
        AggregationResults Modify(object fact);
        AggregationResults Remove(object fact);
        object Result { get; }
    }
}