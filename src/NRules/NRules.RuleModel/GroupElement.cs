namespace NRules.RuleModel;

/// <summary>
/// Grouping element that logically combines the patterns or other grouping elements.
/// </summary>
public abstract class GroupElement : RuleElement
{
    internal GroupElement(IEnumerable<RuleElement> childElements)
    {
        ChildElements = childElements.ToArray();

        AddExports(ChildElements);
        AddImports(ChildElements);
    }

    /// <summary>
    /// List of child elements in the grouping.
    /// </summary>
    public IReadOnlyCollection<RuleElement> ChildElements { get; }
}