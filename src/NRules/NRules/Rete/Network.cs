﻿using System.Collections.Generic;
using NRules.Diagnostics;

namespace NRules.Rete;

internal interface INetwork
{
    void PropagateAssert(IExecutionContext context, List<Fact> factObjects);
    void PropagateUpdate(IExecutionContext context, List<Fact> factObjects);
    void PropagateRetract(IExecutionContext context, List<Fact> factObjects);
    void Activate(IExecutionContext context);
    void Visit<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
    ReteGraph GetSchema();
}

internal class Network(RootNode root, DummyNode dummyNode) : INetwork
{
    public void PropagateAssert(IExecutionContext context, List<Fact> facts)
    {
        foreach (var fact in facts)
        {
            context.EventAggregator.RaiseFactInserting(context.Session, fact);
        }

        root.PropagateAssert(context, facts);

        foreach (var fact in facts)
        {
            context.EventAggregator.RaiseFactInserted(context.Session, fact);
        }
    }

    public void PropagateUpdate(IExecutionContext context, List<Fact> facts)
    {
        foreach (var fact in facts)
        {
            context.EventAggregator.RaiseFactUpdating(context.Session, fact);
        }

        root.PropagateUpdate(context, facts);

        foreach (var fact in facts)
        {
            context.EventAggregator.RaiseFactUpdated(context.Session, fact);
        }
    }

    public void PropagateRetract(IExecutionContext context, List<Fact> facts)
    {
        foreach (var fact in facts)
        {
            context.EventAggregator.RaiseFactRetracting(context.Session, fact);
        }

        root.PropagateRetract(context, facts);

        foreach (var fact in facts)
        {
            context.EventAggregator.RaiseFactRetracted(context.Session, fact);
        }
    }

    public void Activate(IExecutionContext context)
    {
        dummyNode.Activate(context);
    }

    public void Visit<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
    {
        visitor.Visit(context, root);
        visitor.Visit(context, dummyNode);
    }

    public ReteGraph GetSchema()
    {
        var builder = new SchemaBuilder();
        var visitor = new SchemaReteVisitor();
        Visit(builder, visitor);
        return builder.Build();
    }
}