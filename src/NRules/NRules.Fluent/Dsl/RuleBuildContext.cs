using NRules.Fluent.Expressions;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Dsl;

internal readonly struct RuleBuildContext
{
    private RuleBuildContext(RuleBuilder builder)
    {
        Builder = builder;
    }

    public static RuleBuildContext CreateNew(RuleMetadata metadata)
    {
        var context = new RuleBuildContext(new RuleBuilder());
        context.ApplyMetadata(metadata);
        return context;
    }

    public static RuleBuildContext Empty = new();

    public SymbolStack SymbolStack { get; } = new();

    public RuleBuilder Builder { get; }

    public bool IsEmpty => Builder is null;

    private void ApplyMetadata(RuleMetadata metadata)
    {
        Builder.Name(metadata.Name);
        Builder.Description(metadata.Description);
        Builder.Tags(metadata.Tags);
        Builder.Property(RuleProperties.ClrType, metadata.RuleType);

        if (metadata.Priority.HasValue)
        {
            Builder.Priority(metadata.Priority.Value);
        }
        if (metadata.Repeatability.HasValue)
        {
            Builder.Repeatability(metadata.Repeatability.Value);
        }
    }
}
