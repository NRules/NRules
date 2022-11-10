using System.Collections.Generic;

namespace NRules.RuleModel;

/// <summary>
/// Rule element that groups dependencies that the rule uses when its actions runs.
/// </summary>
public class DependencyGroupElement : RuleElement
{
    private readonly List<DependencyElement> _dependencies;

    internal DependencyGroupElement(IEnumerable<DependencyElement> dependencies)
    {
        _dependencies = new List<DependencyElement>(dependencies);

        AddExports(_dependencies);
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.DependencyGroup;

    /// <summary>
    /// List of dependencies the group element contains.
    /// </summary>
    public IEnumerable<DependencyElement> Dependencies => _dependencies;

    internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        visitor.VisitDependencyGroup(context, this);
    }
}