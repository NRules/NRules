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

        public TypeInfo FilterType { get; private set; }

        public override bool IsSatisfiedBy(IExecutionContext context, Fact fact)
        {
            bool isMatchingType = FilterType.IsAssignableFrom(fact.FactType);
            return isMatchingType;
        }

        protected override void UnsatisfiedFactUpdate(IExecutionContext context, IList<Fact> facts)
        {
            //Do nothing, since fact type will never change
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitTypeNode(context, this);
        }
    }
}