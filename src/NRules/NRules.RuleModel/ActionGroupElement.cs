using System.Collections.Generic;

namespace NRules.RuleModel;

/// <summary>
/// Rule element that groups actions that run when the rule fires.
/// </summary>
public class ActionGroupElement : RuleElement
{
    private readonly List<ActionElement> _actions;

    internal ActionGroupElement(IEnumerable<ActionElement> actions)
    {
        _actions = new List<ActionElement>(actions);

        AddImports(_actions);
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.ActionGroup;

    /// <summary>
    /// List of actions the group element contains.
    /// </summary>
    public IEnumerable<ActionElement> Actions => _actions;

    internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        visitor.VisitActionGroup(context, this);
    }
}