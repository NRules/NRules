using System;
using System.Collections.Generic;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent;

/// <summary>
/// Creates instances of <see cref="IRuleDefinition"/> from the fluent DSL <see cref="Rule"/> instances.
/// </summary>
public class RuleDefinitionFactory
{
    /// <summary>
    /// Creates instances of <see cref="IRuleDefinition"/> from the fluent DSL <see cref="Rule"/> instances.
    /// </summary>
    /// <param name="rules">Fluent DSL <see cref="Rule"/> instances.</param>
    /// <returns>Corresponding instances of <see cref="IRuleDefinition"/>.</returns>
    public IReadOnlyCollection<IRuleDefinition> Create(IEnumerable<Rule> rules)
    {
        var ruleDefinitions = new List<IRuleDefinition>();
        foreach (var rule in rules)
        {
            var ruleDefinition = Create(rule);
            ruleDefinitions.Add(ruleDefinition);
        }

        return ruleDefinitions;
    }

    /// <summary>
    /// Creates a <see cref="IRuleDefinition"/> for an instance of a fluent DSL <see cref="Rule"/>.
    /// </summary>
    /// <param name="rule">Fluent DSL <see cref="Rule"/> instance.</param>
    /// <returns>Corresponding instance of <see cref="IRuleDefinition"/>.</returns>
    public IRuleDefinition Create(Rule rule)
    {
        try
        {
            return BuildDefinition(rule);
        }
        catch (Exception e)
        {
            throw new RuleDefinitionException("Failed to build rule definition", rule.GetType(), e);
        }
    }

    private IRuleDefinition BuildDefinition(Rule rule)
    {
        var metadata = new RuleMetadata(rule.GetType());

        var builder = new RuleBuilder();

        ApplyMetadata(builder, metadata);

        rule.Define(builder);

        return builder.Build();
    }

    private static void ApplyMetadata(RuleBuilder builder, RuleMetadata metadata)
    {
        builder.Name(metadata.Name);
        builder.Description(metadata.Description);
        builder.Tags(metadata.Tags);
        builder.Property(RuleProperties.ClrType, metadata.RuleType);

        if (metadata.Priority.HasValue)
        {
            builder.Priority(metadata.Priority.Value);
        }
        if (metadata.Repeatability.HasValue)
        {
            builder.Repeatability(metadata.Repeatability.Value);
        }
    }
}
