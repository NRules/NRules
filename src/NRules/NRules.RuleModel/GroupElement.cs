using System.Collections.Generic;

namespace NRules.RuleModel;

/// <summary>
/// Grouping element that logically combines the patterns or other grouping elements.
/// </summary>
public abstract class GroupElement : RuleElement
{
    private readonly RuleElement[] _childElements;

    private protected GroupElement(RuleElement[] childElements)
    {
        _childElements = childElements;

        AddExports(_childElements);
        AddImports(_childElements);
    }

    /// <summary>
    /// List of child elements in the grouping.
    /// </summary>
    public IReadOnlyList<RuleElement> ChildElements => _childElements;
}