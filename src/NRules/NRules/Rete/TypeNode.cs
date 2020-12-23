using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace NRules.Rete
{
    [DebuggerDisplay("Type {FilterType.FullName,nq}")]
    internal class TypeNode : AlphaNode
    {
        public TypeNode(Type filterType)
        {
            FilterType = filterType.GetTypeInfo();
        }

        public TypeInfo FilterType { get; }

        public override bool IsSatisfiedBy(IExecutionContext context, Fact fact)
        {
            bool isMatchingType = FilterType.IsAssignableFrom(fact.FactType);
            return isMatchingType;
        }

        public override void PropagateUpdate(IExecutionContext context, List<Fact> facts)
        {
            var toUpdate = new List<Fact>();
            foreach (var fact in facts)
            {
                if (IsSatisfiedBy(context, fact))
                    toUpdate.Add(fact);
            }
            PropagateUpdateInternal(context, toUpdate);
        }

        public override void PropagateRetract(IExecutionContext context, List<Fact> facts)
        {
            var toRetract = new List<Fact>();
            foreach (var fact in facts)
            {
                if (IsSatisfiedBy(context, fact))
                    toRetract.Add(fact);
            }
            PropagateRetractInternal(context, toRetract);
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitTypeNode(context, this);
        }
    }
}