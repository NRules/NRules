using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal static class TupleExtensions
    {
        /// <summary>
        /// Facts contained in the tuple in correct order.
        /// </summary>
        /// <remarks>This method has to reverse the linked list and is slow.</remarks>
        public static IEnumerable<IFact> OrderedFacts(this ITuple tuple)
        {
            return tuple.Facts.Reverse();
        }

        public static Quantifier CreateQuantifier(this IExecutionContext context, INode node, Tuple tuple)
        {
            var quantifier = new Quantifier();
            context.WorkingMemory.SetState(node, tuple, quantifier);
            return quantifier;
        }

        public static Quantifier GetQuantifier(this IExecutionContext context, INode node, Tuple tuple)
        {
            var quantifier = context.WorkingMemory.GetStateOrThrow<Quantifier>(node, tuple);
            return quantifier;
        }

        public static Quantifier RemoveQuantifier(this IExecutionContext context, INode node, Tuple tuple)
        {
            var quantifier = context.WorkingMemory.RemoveStateOrThrow<Quantifier>(node, tuple);
            return quantifier;
        }
    }
}