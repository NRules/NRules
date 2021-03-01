using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Types of nodes in the Rete network.
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
        Binding,
        BetaMemory,
        Rule,
    }

    /// <summary>
    /// Node in the Rete network graph.
    /// </summary>
    public class ReteNode
    {
        /// <summary>
        /// Node id, unique within a given instance of <see cref="ISessionFactory"/>.
        /// This id is stable for a given set of rules (same rules will get compiled
        /// into the same Rete network, and produce nodes with the same ids).
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Type of the node in the Rete network.
        /// </summary>
        public NodeType NodeType { get; }

        /// <summary>
        /// Type of element this node produces as output.
        /// </summary>
        public Type ElementType { get; }

        /// <summary>
        /// Properties associated with the node.
        /// </summary>
        public KeyValuePair<string, object>[] Properties { get; }

        /// <summary>
        /// Expressions associated with the node.
        /// </summary>
        public KeyValuePair<string, LambdaExpression>[] Expressions { get; }

        /// <summary>
        /// Rules that this node participates in.
        /// </summary>
        public IRuleDefinition[] Rules { get; }

        internal static ReteNode Create(RootNode node)
        {
            return new ReteNode(node.Id, NodeType.Root);
        }
        
        internal static ReteNode Create(TypeNode node)
        {
            return new ReteNode(node.Id, NodeType.Type, node.FilterType, rules: node.NodeInfo.Rules);
        }
        
        internal static ReteNode Create(SelectionNode node)
        {
            var conditions = new[]
            {
                new KeyValuePair<string, LambdaExpression>("Condition", node.ExpressionElement.Expression)
            };
            var elementType = node.ExpressionElement.Expression.Parameters.Single().Type;
            return new ReteNode(node.Id, NodeType.Selection, elementType, null, conditions, node.NodeInfo.Rules);
        }

        internal static ReteNode Create(AlphaMemoryNode node)
        {
            return new ReteNode(node.Id, NodeType.AlphaMemory, rules: node.NodeInfo.Rules);
        }

        internal static ReteNode Create(JoinNode node)
        {
            var conditions = node.ExpressionElements.Select(c => 
                new KeyValuePair<string, LambdaExpression>("Expression", c.Expression));
            return new ReteNode(node.Id, NodeType.Join, null, null, conditions, node.NodeInfo.Rules);
        }

        internal static ReteNode Create(NotNode node)
        {
            return new ReteNode(node.Id, NodeType.Not, rules: node.NodeInfo.Rules);
        }

        internal static ReteNode Create(ExistsNode node)
        {
            return new ReteNode(node.Id, NodeType.Exists, rules: node.NodeInfo.Rules);
        }

        internal static ReteNode Create(AggregateNode node)
        {
            var values = new[] {new KeyValuePair<string, object>("AggregateName", node.Name)};
            var expressions = node.Expressions.Select(e =>
                new KeyValuePair<string, LambdaExpression>(e.Name, e.Expression));
            return new ReteNode(node.Id, NodeType.Aggregate, null, values, expressions, node.NodeInfo.Rules);
        }

        internal static ReteNode Create(ObjectInputAdapter node)
        {
            return new ReteNode(node.Id, NodeType.Adapter, rules: node.NodeInfo.Rules);
        }

        internal static ReteNode Create(BindingNode node)
        {
            var expressions = new[]
            {
                new KeyValuePair<string, LambdaExpression>("Expression", node.ExpressionElement.Expression)
            };
            return new ReteNode(node.Id, NodeType.Binding, node.ResultType, null, expressions, node.NodeInfo.Rules);
        }

        internal static ReteNode Create(BetaMemoryNode node)
        {
            return new ReteNode(node.Id, NodeType.BetaMemory, rules: node.NodeInfo.Rules);
        }

        internal static ReteNode Create(RuleNode node)
        {
            return new ReteNode(node.Id, NodeType.Rule, rules: node.NodeInfo.Rules);
        }

        internal static ReteNode Create(DummyNode node)
        {
            return new ReteNode(node.Id, NodeType.Dummy);
        }

        internal ReteNode(int id, 
            NodeType nodeType, 
            Type elementType = null, 
            IEnumerable<KeyValuePair<string, object>> values = null,
            IEnumerable<KeyValuePair<string, LambdaExpression>> expressions = null, 
            IEnumerable<IRuleDefinition> rules = null)
        {
            Id = id;
            NodeType = nodeType;
            ElementType = elementType;
            Properties = values?.ToArray() ?? Array.Empty<KeyValuePair<string, object>>();
            Expressions = expressions?.ToArray() ?? Array.Empty<KeyValuePair<string, LambdaExpression>>();
            Rules = rules?.ToArray() ?? Array.Empty<IRuleDefinition>();
        }
    }
}