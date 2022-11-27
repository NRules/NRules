﻿using System.Collections.Generic;
using NRules.Diagnostics;

namespace NRules.Rete;

internal class ExistsNode : BinaryBetaNode
{
    public ExistsNode(ITupleSource leftSource, IObjectSource rightSource) : base(leftSource, rightSource, true)
    {
    }

    public override void PropagateAssert(IExecutionContext context, List<Tuple> tuples)
    {
        var toAssert = new TupleFactList();
        using (var counter = PerfCounter.Assert(context, this))
        {
            var joinedSets = JoinedSets(context, tuples);
            foreach (var set in joinedSets)
            {
                var quantifier = context.CreateQuantifier(this, set.Tuple);
                quantifier.Value += set.Facts.Count;
                if (quantifier.Value > 0)
                {
                    toAssert.Add(set.Tuple, null);
                }
            }

            counter.AddInputs(tuples.Count);
            counter.AddOutputs(toAssert.Count);
        }

        EnsureMemoryNode().PropagateAssert(context, toAssert);
    }

    public override void PropagateUpdate(IExecutionContext context, List<Tuple> tuples)
    {
        var toUpdate = new TupleFactList();
        using (var counter = PerfCounter.Update(context, this))
        {
            foreach (var tuple in tuples)
            {
                if (context.GetQuantifier(this, tuple).Value > 0)
                {
                    toUpdate.Add(tuple, null);
                }
            }

            counter.AddInputs(tuples.Count);
            counter.AddOutputs(toUpdate.Count);
        }

        EnsureMemoryNode().PropagateUpdate(context, toUpdate);
    }

    public override void PropagateRetract(IExecutionContext context, List<Tuple> tuples)
    {
        var toRetract = new TupleFactList();
        using (var counter = PerfCounter.Retract(context, this))
        {
            foreach (var tuple in tuples)
            {
                if (context.RemoveQuantifier(this, tuple).Value > 0)
                {
                    toRetract.Add(tuple, null);
                }
            }

            counter.AddInputs(tuples.Count);
            counter.AddOutputs(toRetract.Count);
        }

        EnsureMemoryNode().PropagateRetract(context, toRetract);
    }

    public override void PropagateAssert(IExecutionContext context, List<Fact> facts)
    {
        var toAssert = new TupleFactList();
        using (var counter = PerfCounter.Assert(context, this))
        {
            var joinedSets = JoinedSets(context, facts);
            foreach (var set in joinedSets)
            {
                var quantifier = context.GetQuantifier(this, set.Tuple);
                int startingCount = quantifier.Value;
                quantifier.Value += set.Facts.Count;
                if (startingCount == 0 && quantifier.Value > 0)
                {
                    toAssert.Add(set.Tuple, null);
                }
            }

            counter.AddInputs(facts.Count);
            counter.AddOutputs(toAssert.Count);
        }

        EnsureMemoryNode().PropagateAssert(context, toAssert);
    }

    public override void PropagateUpdate(IExecutionContext context, List<Fact> facts)
    {
        //Do nothing
    }

    public override void PropagateRetract(IExecutionContext context, List<Fact> facts)
    {
        var toRetract = new TupleFactList();
        using (var counter = PerfCounter.Retract(context, this))
        {
            var joinedSets = JoinedSets(context, facts);
            foreach (var set in joinedSets)
            {
                var quantifier = context.GetQuantifier(this, set.Tuple);
                int startingCount = quantifier.Value;
                quantifier.Value -= set.Facts.Count;
                if (startingCount > 0 && quantifier.Value == 0)
                {
                    toRetract.Add(set.Tuple, null);
                }
            }

            counter.AddInputs(facts.Count);
            counter.AddOutputs(toRetract.Count);
        }

        EnsureMemoryNode().PropagateRetract(context, toRetract);
    }

    public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
    {
        visitor.VisitExistsNode(context, this);
    }
}