using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel;

/// <summary>
/// Rule element that groups dependencies that the rule uses when its actions runs.
/// </summary>
public class DependencyGroupElement : RuleElement
{
    private readonly DependencyElement[] _dependencies;

    internal DependencyGroupElement(IEnumerable<DependencyElement> dependencies)
    {
        _dependencies = dependencies.ToArray();

        AddExports(_dependencies);
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.DependencyGroup;

    /// <summary>
    /// List of dependencies the group element contains.
    /// </summary>
    public IReadOnlyCollection<DependencyElement> Dependencies => _dependencies;

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitDependencyGroup(context, this);
    }

    internal DependencyGroupElement Update(IReadOnlyCollection<DependencyElement> dependencies)
    {
        if (ReferenceEquals(dependencies, _dependencies)) return this;
        return new DependencyGroupElement(dependencies);
    }
}