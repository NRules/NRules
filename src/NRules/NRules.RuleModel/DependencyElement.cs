﻿using System;

namespace NRules.RuleModel;

/// <summary>
/// Dependency that the rule uses when its actions run.
/// </summary>
public class DependencyElement : RuleElement
{
    internal DependencyElement(Declaration declaration, Type serviceType)
    {
        Declaration = declaration;
        ServiceType = serviceType;

        AddExport(declaration);
    }

    /// <inheritdoc cref="RuleElement.ElementType"/>
    public override ElementType ElementType => ElementType.Dependency;

    /// <summary>
    /// Declaration that references the dependency.
    /// </summary>
    public Declaration Declaration { get; }

    /// <summary>
    /// Type of service that this dependency configures.
    /// </summary>
    public Type ServiceType { get; }

    internal override RuleElement Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
    {
        return visitor.VisitDependency(context, this);
    }
}