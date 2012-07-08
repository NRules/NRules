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

        public override void PropagateUpdate(Fact fact)
        {
            throw new System.NotImplementedException();
        }

        public override void PropagateRetract(Fact fact)
        {
            throw new System.NotImplementedException();
        }
    }
}