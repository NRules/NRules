using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel;

/// <summary>
/// Grouping element that logically combines the patterns or other grouping elements.
/// </summary>
public abstract class GroupElement : RuleElement
{
    private readonly RuleElement[] _childElements;

    internal GroupElement(IEnumerable<RuleElement> childElements)
    {
        _childElements = childElements.ToArray();

        AddExports(_childElements);
        AddImports(_childElements);
    }

    /// <summary>
    /// List of child elements in the grouping.
    /// </summary>
    public IReadOnlyCollection<RuleElement> ChildElements => _childElements;
}