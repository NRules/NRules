using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Aggregators;

/// <summary>
/// Expression used by an aggregator, compiled to an executable form.
/// </summary>
public interface IAggregateExpression
{
    /// <summary>
    /// Name of the aggregate expression.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Invokes the expression with the given inputs.
    /// </summary>
    /// <param name="context">Aggregation context.</param>
    /// <param name="tuple">Partial match up to the aggregate element.</param>
    /// <param name="fact">Fact being processed by the aggregate element.</param>
    /// <returns>Result of the expression.</returns>
    object Invoke(AggregationContext context, ITuple tuple, IFact fact);
}

internal class AggregateExpression(string name, ILhsExpression<object> compiledExpression) : IAggregateExpression
{
    public string Name { get; } = name;

    public object Invoke(AggregationContext context, ITuple tuple, IFact fact)
    {
        return compiledExpression.Invoke(context.ExecutionContext, context.NodeInfo, (tuple as Tuple)!, (fact as Fact)!);
    }
}
