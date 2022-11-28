using System;
using NRules.Fluent.Expressions;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Dsl;

/// <summary>
/// Base class for inline rule definitions.
/// To create a rule using internal DSL, create a class that inherits from <c>NRules.Fluent.Dsl.Rule</c>
/// and override <see cref="Define()"/> method.
/// Use <see cref="When()"/> and <see cref="Then()"/> methods to define rule's conditions and actions correspondingly.
/// A rule can also be decorated with attributes to add relevant metadata:
/// <see cref="NameAttribute"/>, <see cref="DescriptionAttribute"/>, <see cref="TagAttribute"/>, 
/// <see cref="PriorityAttribute"/>, <see cref="RepeatabilityAttribute"/>.
/// </summary>
public abstract class Rule
{
    private RuleBuildContext _context = RuleBuildContext.Empty;

    protected Rule()
    {
    }

    /// <summary>
    /// Sets rule's name.
    /// Name value set at this level overrides the values specified via <see cref="NameAttribute"/> attribute.
    /// </summary>
    /// <param name="value">Rule name value.</param>
    protected void Name(string value)
    {
        ValidateContext();
        _context.Builder.Name(value);
    }

    /// <summary>
    /// Sets rule's priority.
    /// Priority value set at this level overrides the value specified via <see cref="PriorityAttribute"/> attribute.
    /// </summary>
    /// <param name="value">Priority value.</param>
    protected void Priority(int value)
    {
        ValidateContext();
        _context.Builder.Priority(value);
    }

    /// <summary>
    /// Returns expression builder for rule's dependencies.
    /// </summary>
    /// <returns>Dependencies expression builder.</returns>
    protected IDependencyExpression Dependency()
    {
        ValidateContext();
        var builder = _context.Builder.Dependencies();
        var expression = new DependencyExpression(builder, _context.SymbolStack);
        return expression;
    }

    /// <summary>
    /// Returns expression builder for rule's filters.
    /// </summary>
    /// <returns>Filters expression builder.</returns>
    protected IFilterExpression Filter()
    {
        ValidateContext();
        var builder = _context.Builder.Filters();
        var expression = new FilterExpression(builder, _context.SymbolStack);
        return expression;
    }

    /// <summary>
    /// Returns expression builder for rule's left-hand side (conditions).
    /// </summary>
    /// <returns>Left hand side expression builder.</returns>
    protected ILeftHandSideExpression When()
    {
        ValidateContext();
        var builder = _context.Builder.LeftHandSide();
        var expression = new LeftHandSideExpression(builder, _context.SymbolStack);
        return expression;
    }

    /// <summary>
    /// Returns expression builder for rule's right-hand side (actions).
    /// </summary>
    /// <returns>Right hand side expression builder.</returns>
    protected IRightHandSideExpression Then()
    {
        ValidateContext();
        var builder = _context.Builder.RightHandSide();
        var expression = new RightHandSideExpression(builder, _context.SymbolStack);
        return expression;
    }

    /// <summary>
    /// Method called by the rules engine to define the rule.
    /// </summary>
    public abstract void Define();

    internal void Define(RuleBuilder builder)
    {
        var defaultContext = _context;
        try
        {
            _context = new RuleBuildContext(builder);
            Define();
        }
        finally
        {
            _context = defaultContext;
        }
    }

    private void ValidateContext()
    {
        if (_context.IsEmpty)
            throw new InvalidOperationException("Use Define(RuleBuildContext context) method");
    }

    private readonly struct RuleBuildContext
    {
        public RuleBuildContext(RuleBuilder builder)
        {
            Builder = builder;
            SymbolStack = new();
        }

        public static RuleBuildContext Empty = new();

        public SymbolStack SymbolStack { get; }

        public RuleBuilder Builder { get; }

        public bool IsEmpty => Builder is null;
    }

}