using NRules.RuleModel;

namespace NRules.Aggregators;

internal class SortCondition(string name, SortDirection direction, IAggregateExpression expression)
{
    public string Name { get; } = name;
    public SortDirection Direction { get; } = direction;
    public IAggregateExpression Expression { get; } = expression;
}