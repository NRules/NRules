using NRules.Diagnostics;

namespace NRules.Rete
{
    internal interface INode
    {
        int Id { get; }
        NodeInfo NodeInfo { get; }
        void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
    }
}
