namespace NRules.Rete
{
    internal static class TupleExtensions
    {
        public static Quantifier Quantifier(this Tuple tuple, INode node)
        {
            var quantifier = tuple.GetState<Quantifier>(node);
            if (quantifier == null)
            {
                quantifier = new Quantifier();
                tuple.SetState(node, quantifier);
            }
            return quantifier;
        }
    }
}