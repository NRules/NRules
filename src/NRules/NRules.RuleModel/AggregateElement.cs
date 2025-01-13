using System;
using System.Linq;

namespace NRules.RuleModel;

/// <summary>
/// Rule element that creates new facts (aggregates) based on matching facts it receives as input.
/// </summary>
public class AggregateElement : RuleElement
{
    public const string CollectName = "Collect";
    public const string GroupByName = "GroupBy";
    public const string ProjectName = "Project";
    public const string FlattenName = "Flatten";

    public const string SelectorName = "Selector";
    public const string ElementSelectorName = "ElementSelector";
    public const string KeySelectorName = "KeySelector";
    public const string KeySelectorAscendingName = "KeySelectorAscending";
    public const string KeySelectorDescendingName = "KeySelectorDescending";

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.Aggregate;

    /// <summary>
    /// Type of the result that this rule element yields.
    /// </summary>
    public Type ResultType { get; }

    /// <summary>
    /// Fact source of the aggregate.
    /// </summary>
    public PatternElement Source { get; }

    /// <summary>
    /// Aggregate name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The type of custom aggregator factory.
    /// </summary>
    public Type? CustomFactoryType { get; }

    /// <summary>
    /// Expressions used by the aggregate.
    /// </summary>
    public ExpressionCollection Expressions { get; }

    internal AggregateElement(Type resultType, string name, ExpressionCollection expressions, PatternElement source, Type? customFactoryType)
    {
        ResultType = resultType;
        Name = name;
        Expressions = expressions;
        Source = source;
        CustomFactoryType = customFactoryType;

        AddImports(source);
        AddImports(expressions.SelectMany(x => x.Imports.Except(source.Exports)));
    }

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitAggregate(context, this);
    }

    internal AggregateElement Update(ExpressionCollection expressions, PatternElement source)
    {
        if (ReferenceEquals(expressions, Expressions) && ReferenceEquals(source, Source)) return this;
        return new AggregateElement(ResultType, Name, expressions, source, CustomFactoryType);
    }
}