namespace NRules.Rete
{
    internal static class TupleExtensions
    {
        public static Quantifier Quantifier(this Tuple tuple)
        {
            var quantifier = tuple.GetState<Quantifier>();
            if (quantifier == null)
            {
                quantifier = new Quantifier();
                tuple.SetState(quantifier);
            }
            return quantifier;
        }
    }
}