using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Rete;

namespace NRules.Diagnostics
{
    public enum NodeType
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
    public class NodeInfo
    {
        internal static NodeInfo Create(RootNode node)
        {
            return new NodeInfo(NodeType.Root, string.Empty);
        }
        
        internal static NodeInfo Create(TypeNode node)
        {
            return new NodeInfo(NodeType.Type, node.FilterType.Name);
        }
        
        internal static NodeInfo Create(SelectionNode node)
        {
            return new NodeInfo(NodeType.Selection, string.Empty, node.Conditions.Select(c => c.ExpressionString));
        }

        internal static NodeInfo Create(AlphaMemoryNode node, IAlphaMemory memory)
        {
            return new NodeInfo(NodeType.AlphaMemory, string.Empty, memory.Facts.Select(f => f.Object.ToString()));
        }

        internal static NodeInfo Create(JoinNode node)
        {
            return new NodeInfo(NodeType.Join, string.Empty, node.Conditions.Select(c => c.ExpressionString));
        }

        internal static NodeInfo Create(NotNode node)
        {
            return new NodeInfo(NodeType.Not, string.Empty);
        }

        internal static NodeInfo Create(ExistsNode node)
        {
            return new NodeInfo(NodeType.Exists, string.Empty);
        }

        internal static NodeInfo Create(AggregateNode node)
        {
            return new NodeInfo(NodeType.Aggregate, string.Empty);
        }

        internal static NodeInfo Create(ObjectInputAdapter node)
        {
            return new NodeInfo(NodeType.Adapter, string.Empty);
        }

        internal static NodeInfo Create(BetaMemoryNode node, IBetaMemory memory)
        {
            var tuples = memory.Tuples.Select(
                t => string.Join(" || ", t.Select(f =>  f.Object).ToArray()));
            return new NodeInfo(NodeType.BetaMemory, string.Empty, tuples);
        }

        internal static NodeInfo Create(TerminalNode node)
        {
            return new NodeInfo(NodeType.Terminal, string.Empty);
        }

        internal static NodeInfo Create(RuleNode node)
        {
            return new NodeInfo(NodeType.Rule, node.Rule.Definition.Name);
        }

        internal NodeInfo(NodeType nodeType, string details)
            : this(nodeType, details, new string[] { })
        {
        }

        internal NodeInfo(NodeType nodeType, string details, IEnumerable<string> items)
        {
            NodeType = nodeType;
            Details = details;
            Items = items.ToArray();
        }

        public NodeType NodeType { get; private set; }
        public string Details { get; private set; }
        public string[] Items { get; private set; }
    }
}