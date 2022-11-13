namespace NRules.RuleModel;

/// <summary>
/// Rule element that groups dependencies that the rule uses when its actions runs.
/// </summary>
public class DependencyGroupElement : RuleElement
{
    private readonly IReadOnlyCollection<DependencyElement> _dependencies;

    internal DependencyGroupElement(IReadOnlyCollection<DependencyElement> dependencies)
    {
        _dependencies = dependencies;

        AddExports(_dependencies);
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.DependencyGroup;

    /// <summary>
    /// List of dependencies the group element contains.
    /// </summary>
    public IReadOnlyCollection<DependencyElement> Dependencies => _dependencies;

    internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        visitor.VisitDependencyGroup(context, this);
    }
}