using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel;

/// <summary>
/// Universal quantifier.
/// </summary>
public class ForAllElement : RuleElement
{
    private readonly PatternElement[] _patterns;

    internal ForAllElement(PatternElement source, IEnumerable<PatternElement> patterns)
    {
        BasePattern = source;
        _patterns = patterns.ToArray();

        AddImports(source);
        AddImports(_patterns);
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.ForAll;

    /// <summary>
    /// Base pattern that determines the universe of facts that the universal quantifier is applied to.
    /// </summary>
    public PatternElement BasePattern { get; }

    /// <summary>
    /// Patterns that must all match for the selected facts.
    /// </summary>
    public IReadOnlyCollection<PatternElement> Patterns => _patterns;

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitForAll(context, this);
    }

    internal ForAllElement Update(PatternElement basePattern, IReadOnlyCollection<PatternElement> patterns)
    {
        if (ReferenceEquals(basePattern, BasePattern) && ReferenceEquals(patterns, Patterns)) return this;
        return new ForAllElement(basePattern, patterns);
    }
}
