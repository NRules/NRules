using System.Collections.Generic;
using System.Linq;
using NRules.Rete;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Types of nodes in the rete network.
    /// </summary>
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

    /// <summary>
    /// Node in the rete network graph.
    /// </summary>
#if NET45
    [System.Serializable]
#endif
    public class NodeInfo
    {
        private static readonly string[] Empty = {};

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
            return new NodeInfo(NodeType.Selection, string.Empty, new[] {node.Condition.ToString()}, Empty, Empty);
        }

        internal static NodeInfo Create(AlphaMemoryNode node, IAlphaMemory memory)
        {
            return new NodeInfo(NodeType.AlphaMemory, string.Empty, Empty, Empty, memory.Facts.Select(f => f.Object.ToString()));
        }

        internal static NodeInfo Create(JoinNode node)
        {
            return new NodeInfo(NodeType.Join, string.Empty, node.Conditions.Select(c => c.ToString()), Empty, Empty);
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
            var expressions = node.ExpressionMap.Select(e => string.Format("{0}={1}", e.Name, e.Expression.ToString()));
            return new NodeInfo(NodeType.Aggregate, node.Name, Empty, expressions, Empty);
        }

        internal static NodeInfo Create(ObjectInputAdapter node)
        {
            return new NodeInfo(NodeType.Adapter, string.Empty);
        }

        internal static NodeInfo Create(BetaMemoryNode node, IBetaMemory memory)
        {
            var tuples = memory.Tuples.Select(
                t => string.Join(" || ", t.OrderedFacts.Select(f => f.Object).ToArray()));
            return new NodeInfo(NodeType.BetaMemory, string.Empty, Empty, Empty, tuples);
        }

        internal static NodeInfo Create(TerminalNode node)
        {
            return new NodeInfo(NodeType.Terminal, string.Empty);
        }

        internal static NodeInfo Create(RuleNode node)
        {
            return new NodeInfo(NodeType.Rule, node.CompiledRule.Definition.Name);
        }

        internal NodeInfo(NodeType nodeType, string details)
            : this(nodeType, details, Empty, Empty, Empty)
        {
        }

        internal NodeInfo(NodeType nodeType, string details, IEnumerable<string> conditions, IEnumerable<string> expressions, IEnumerable<string> items)
        {
            NodeType = nodeType;
            Details = details;
            Conditions = conditions.ToArray();
            Expressions = expressions.ToArray();
            Items = items.ToArray();
        }

        /// <summary>
        /// Type of the node in the rete network.
        /// </summary>
        public NodeType NodeType { get; private set; }

        /// <summary>
        /// Additional node details.
        /// </summary>
        public string Details { get; private set; }

        /// <summary>
        /// Match conditions.
        /// </summary>
        public string[] Conditions { get; private set; }

        /// <summary>
        /// Additional node expressions.
        /// </summary>
        public string[] Expressions { get; private set; }

        /// <summary>
        /// Facts/tuples currently associated with the node.
        /// </summary>
        public string[] Items { get; private set; }
    }
}