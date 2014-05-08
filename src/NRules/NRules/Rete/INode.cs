namespace NRules.Rete
{
    internal interface INode
    {
        void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
    }
}
