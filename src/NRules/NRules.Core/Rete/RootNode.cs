namespace NRules.Core.Rete
{
    internal class RootNode : AlphaNode
    {
        public override bool IsSatisfiedBy(Fact fact)
        {
            return true;
        }
    }
}