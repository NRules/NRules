namespace NRules.Rete
{
    internal static class TupleExtensions
    {
        public static Quantifier CreateQuantifier(this Tuple tuple, INode node)
        {
            var quantifier = new Quantifier();
            tuple.SetState(node, quantifier);
            return quantifier;
        }

        public static Quantifier GetQuantifier(this Tuple tuple, INode node)
        {
            var quantifier = tuple.GetState<Quantifier>(node);
            return quantifier;
        }

        public static Quantifier RemoveQuantifier(this Tuple tuple, INode node)
        {
            var quantifier = tuple.RemoveState<Quantifier>(node);
            return quantifier;
        }
    }
}