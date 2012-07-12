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
            foreach (TypeNode typeNode in ChildNodes)
            {
                typeNode.PropagateUpdate(fact);
            }
        }

        public override void PropagateRetract(Fact fact)
        {
            foreach (TypeNode typeNode in ChildNodes)
            {
                typeNode.PropagateRetract(fact);
            }
        }

        public override void ForceRetract(Fact fact)
        {
            PropagateRetract(fact);
        }
    }
}