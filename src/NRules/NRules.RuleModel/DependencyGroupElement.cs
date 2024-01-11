using System.Collections.Generic;

namespace NRules.RuleModel;

/// <summary>
/// Rule element that groups dependencies that the rule uses when its actions runs.
/// </summary>
public class DependencyGroupElement : RuleElement
{
    private readonly DependencyElement[] _dependencies;

    internal DependencyGroupElement(DependencyElement[] dependencies)
    {
        _dependencies = dependencies;

        AddExports(_dependencies);
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.DependencyGroup;

    /// <summary>
    /// List of dependencies the group element contains.
    /// </summary>
    public IReadOnlyList<DependencyElement> Dependencies => _dependencies;

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitDependencyGroup(context, this);
    }

    internal DependencyGroupElement Update(IReadOnlyList<DependencyElement> dependencies)
    {
        if (ReferenceEquals(dependencies, _dependencies)) return this;
        return new DependencyGroupElement(dependencies.AsArray());
    }
}