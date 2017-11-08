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

        public static Quantifier CreateQuantifier(this Tuple tuple, INode node)
        {
            var quantifier = new Quantifier();
            tuple.SetState(node, quantifier);
            return quantifier;
        }

        public static Quantifier GetQuantifier(this Tuple tuple, INode node)
        {
            var quantifier = tuple.GetStateOrThrow<Quantifier>(node);
            return quantifier;
        }

        public static Quantifier RemoveQuantifier(this Tuple tuple, INode node)
        {
            var quantifier = tuple.RemoveStateOrThrow<Quantifier>(node);
            return quantifier;
        }
    }
}