using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel;

/// <summary>
/// Rule element that groups actions that run when the rule fires.
/// </summary>
public class ActionGroupElement : RuleElement
{
    private readonly ActionElement[] _actions;

    internal ActionGroupElement(IEnumerable<ActionElement> actions)
    {
        _actions = actions.ToArray();

        AddImports(_actions);
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.ActionGroup;

    /// <summary>
    /// List of actions the group element contains.
    /// </summary>
    public IReadOnlyCollection<ActionElement> Actions => _actions;

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitActionGroup(context, this);
    }

    internal ActionGroupElement Update(IReadOnlyCollection<ActionElement> actions)
    {
        if (ReferenceEquals(actions, _actions)) return this;
        return new ActionGroupElement(actions);
    }
}