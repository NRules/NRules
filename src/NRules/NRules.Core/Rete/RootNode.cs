namespace NRules.Core.Rete
{
    internal class RootNode : AlphaNode
    {
        public override void PropagateAssert(Fact fact)
        {
            foreach (TypeNode typeNode in ChildNodes)
            {
                typeNode.PropagateAssert(fact);
            }
        }
    }
}