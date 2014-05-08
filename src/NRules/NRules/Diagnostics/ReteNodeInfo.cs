using System;

namespace NRules.Diagnostics
{
    public enum ReteNodeType
    {
        Root,
        Type,
        Selection,
        AlphaMemory,
        Dummy,
        Join,
        Adapter,
        Exists,
        Aggregate,
        Not,
        BetaMemory,
        Terminal,
        Rule,
    }

    [Serializable]
    public class ReteNodeInfo
    {
        internal ReteNodeInfo(int id, ReteNodeType nodeType, string details)
        {
            Id = id;
            NodeType = nodeType;
            Details = details;
        }

        public int Id { get; private set; }
        public ReteNodeType NodeType { get; private set; }
        public string Details { get; private set; }
    }
}