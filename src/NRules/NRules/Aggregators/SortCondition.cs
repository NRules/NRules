using NRules.RuleModel;

namespace NRules.Aggregators;

internal class SortCondition
{
    public SortCondition(string name, SortDirection direction, IAggregateExpression expression)
    {
        Name = name;
        Direction = direction;
        Expression = expression;
    }

    public string Name { get; }
    public SortDirection Direction { get; }
    public IAggregateExpression Expression { get; }
}