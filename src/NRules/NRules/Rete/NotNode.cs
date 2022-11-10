using System;
using System.Collections.Generic;
using NRules.Diagnostics;

namespace NRules.Rete;

internal class NotNode : BinaryBetaNode
{
    public NotNode(int id, ITupleSource leftSource, IObjectSource rightSource)
        : base(id, null, leftSource, rightSource, true)
    {
    }

    public override void PropagateAssert(IExecutionContext context, IReadOnlyCollection<Tuple> tuples)
    {
        if (MemoryNode is null)
        {
            throw new InvalidOperationException($"{nameof(MemoryNode)} is null");
        }

        var toAssert = new TupleFactList();
        using (var counter = PerfCounter.Assert(context, this))
        {
            var joinedSets = JoinedSets(context, tuples);
            foreach (var set in joinedSets)
            {
                var quantifier = context.CreateQuantifier(this, set.Tuple);
                quantifier.Value += set.Facts.Count;
                if (quantifier.Value == 0)
                {
                    toAssert.Add(set.Tuple);
                }
            }

            counter.AddInputs(tuples.Count);
            counter.AddOutputs(toAssert.Count);
        }

        MemoryNode.PropagateAssert(context, toAssert);
    }

    public override void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Tuple> tuples)
    {
        if (MemoryNode is null)
        {
            throw new InvalidOperationException($"{nameof(MemoryNode)} is null");
        }

        var toUpdate = new TupleFactList();
        using (var counter = PerfCounter.Update(context, this))
        {
            foreach (var tuple in tuples)
            {
                if (context.GetQuantifier(this, tuple).Value == 0)
                {
                    toUpdate.Add(tuple);
                }
            }

            counter.AddInputs(tuples.Count);
            counter.AddOutputs(toUpdate.Count);
        }

        MemoryNode.PropagateUpdate(context, toUpdate);
    }

    public override void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Tuple> tuples)
    {
        if (MemoryNode is null)
        {
            throw new InvalidOperationException($"{nameof(MemoryNode)} is null");
        }

        var toRetract = new TupleFactList();
        using (var counter = PerfCounter.Retract(context, this))
        {
            foreach (var tuple in tuples)
            {
                if (context.RemoveQuantifier(this, tuple).Value == 0)
                {
                    toRetract.Add(tuple);
                }
            }

            counter.AddInputs(tuples.Count);
            counter.AddOutputs(toRetract.Count);
        }

        MemoryNode.PropagateRetract(context, toRetract);
    }

    public override void PropagateAssert(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        if (MemoryNode is null)
        {
            throw new InvalidOperationException($"{nameof(MemoryNode)} is null");
        }

        var toRetract = new TupleFactList();
        using (var counter = PerfCounter.Assert(context, this))
        {
            var joinedSets = JoinedSets(context, facts);
            foreach (var set in joinedSets)
            {
                var quantifier = context.GetQuantifier(this, set.Tuple);
                var startingCount = quantifier.Value;
                quantifier.Value += set.Facts.Count;
                if (startingCount == 0 && quantifier.Value > 0)
                {
                    toRetract.Add(set.Tuple);
                }
            }

            counter.AddInputs(facts.Count);
            counter.AddOutputs(toRetract.Count);
        }

        MemoryNode.PropagateRetract(context, toRetract);
    }

    public override void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        //Do nothing
    }

    public override void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        if (MemoryNode is null)
        {
            throw new InvalidOperationException($"{nameof(MemoryNode)} is null");
        }

        var toAssert = new TupleFactList();
        using (var counter = PerfCounter.Retract(context, this))
        {
            var joinedSets = JoinedSets(context, facts);
            foreach (var set in joinedSets)
            {
                var quantifier = context.GetQuantifier(this, set.Tuple);
                var startingCount = quantifier.Value;
                quantifier.Value -= set.Facts.Count;
                if (startingCount > 0 && quantifier.Value == 0)
                {
                    toAssert.Add(set.Tuple);
                }
            }

            counter.AddInputs(facts.Count);
            counter.AddOutputs(toAssert.Count);
        }

        MemoryNode.PropagateAssert(context, toAssert);
    }

    public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
    {
        visitor.VisitNotNode(context, this);
    }
}