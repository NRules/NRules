using System;
using System.Collections.Generic;
using System.Diagnostics;
using NRules.Diagnostics;

namespace NRules.Rete
{
    [DebuggerDisplay("Type {FilterType.FullName,nq}")]
    internal class TypeNode : AlphaNode
    {
        public TypeNode(Type filterType)
        {
            FilterType = filterType;
        }

        public Type FilterType { get; }

        public override bool IsSatisfiedBy(IExecutionContext context, Fact fact)
        {
            bool isMatchingType = FilterType.IsAssignableFrom(fact.FactType);
            return isMatchingType;
        }

        public override void PropagateUpdate(IExecutionContext context, List<Fact> facts)
        {
            var toUpdate = new List<Fact>();
            using (var counter = PerfCounter.Update(context, this))
            {
                foreach (var fact in facts)
                {
                    if (IsSatisfiedBy(context, fact))
                        toUpdate.Add(fact);
                }
                counter.AddInputs(facts.Count);
                counter.AddOutputs(toUpdate.Count);
            }

            PropagateUpdateInternal(context, toUpdate);
        }

        public override void PropagateRetract(IExecutionContext context, List<Fact> facts)
        {
            var toRetract = new List<Fact>();
            using (var counter = PerfCounter.Retract(context, this))
            {
                foreach (var fact in facts)
                {
                    if (IsSatisfiedBy(context, fact))
                        toRetract.Add(fact);
                }
                counter.AddInputs(facts.Count);
                counter.AddOutputs(toRetract.Count);
            }

            PropagateRetractInternal(context, toRetract);
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitTypeNode(context, this);
        }
    }
}