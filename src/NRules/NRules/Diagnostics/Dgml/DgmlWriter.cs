using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace NRules.Diagnostics.Dgml
{
    /// <summary>
    /// Creates a <see href="https://en.wikipedia.org/wiki/DGML">DGML</see> document writer
    /// that can be used to serialize a Rete graph for a given rules session into XML.
    /// </summary>
    public class DgmlWriter
    {
        private readonly ReteGraph _schema;

        private HashSet<string> _ruleNameFilter;
        private IMetricsProvider _metricsProvider;

        /// <summary>
        /// Creates an instance of a <c>DgmlWriter</c> for a given session schema.
        /// </summary>
        /// <param name="schema">Rules session schema.</param>
        public DgmlWriter(ReteGraph schema)
        {
            _schema = schema;
        }

        /// <summary>
        /// Writes DGML graph representing a given rules session to a file.
        /// </summary>
        /// <param name="fileName">File to write the session to.</param>
        public void WriteAllText(string fileName)
        {
            string contents = GetContents();
            File.WriteAllText(fileName, contents);
        }

        /// <summary>
        /// Writes DGML graph representing a given rules session to the provided
        /// <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer"><see cref="XmlWriter"/> to write the session to.</param>
        public void WriteXml(XmlWriter writer)
        {
            var graph = CreateGraph();
            writer.WriteStartDocument();
            graph.WriteXml(writer);
            writer.WriteEndDocument();
        }

        /// <summary>
        /// Retrieves a serialized DGML graph as an XML string.
        /// </summary>
        /// <returns>Contents of the serialized DGML graph as an XML string.</returns>
        public string GetContents()
        {
            using var stringWriter = new Utf8StringWriter();
            var xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.Formatting = Formatting.Indented;
            WriteXml(xmlWriter);
            var contents = stringWriter.ToString();
            return contents;
        }

        private DirectedGraph CreateGraph()
        {
            var graph = new DirectedGraph {Title = "ReteNetwork"};

            foreach (var reteNode in Filter(_schema.Nodes))
            {
                var node = new Node
                {
                    Id = Id(reteNode),
                    Label = GetNodeLabel(reteNode),
                    Category = reteNode.NodeType.ToString()
                };
                graph.Nodes.Add(node);

                node.Properties.Add("ElementType", reteNode.ElementType?.FullName);

                foreach (var valueGroup in reteNode.Properties.GroupBy(x => x.Key, x => x.Value))
                {
                    var key = valueGroup.Key;
                    var value = string.Join("; ", valueGroup);
                    node.Properties.Add(key, value);
                }

                foreach (var expressionGroup in reteNode.Expressions.GroupBy(x => x.Key, x => x.Value))
                {
                    var key = expressionGroup.Key;
                    var value = string.Join("; ", expressionGroup);
                    node.Properties.Add(key, value);
                }

                if (reteNode.Rules.Length > 0)
                {
                    var value = string.Join("; ", reteNode.Rules.Select(x => x.Name));
                    node.Properties.Add("Rule", value);
                }

                AddPerformanceMetrics(node, reteNode);
            }

            foreach (var linkInfo in Filter(_schema.Links))
            {
                var link = new Link
                {
                    Source = Id(linkInfo.Source),
                    Target = Id(linkInfo.Target)
                };
                graph.Links.Add(link);
            }

            graph.Categories.Add(new Category {Id = NodeType.Root.ToString(), Background = "Black"});
            graph.Categories.Add(new Category {Id = NodeType.Type.ToString(), Background = "Orange"});
            graph.Categories.Add(new Category {Id = NodeType.Selection.ToString(), Background = "Blue"});
            graph.Categories.Add(new Category {Id = NodeType.AlphaMemory.ToString(), Background = "Red"});
            graph.Categories.Add(new Category {Id = NodeType.Dummy.ToString(), Background = "Silver"});
            graph.Categories.Add(new Category {Id = NodeType.Join.ToString(), Background = "Blue"});
            graph.Categories.Add(new Category {Id = NodeType.Not.ToString(), Background = "Brown"});
            graph.Categories.Add(new Category {Id = NodeType.Exists.ToString(), Background = "Brown"});
            graph.Categories.Add(new Category {Id = NodeType.Aggregate.ToString(), Background = "Brown"});
            graph.Categories.Add(new Category {Id = NodeType.BetaMemory.ToString(), Background = "Green"});
            graph.Categories.Add(new Category {Id = NodeType.Adapter.ToString(), Background = "Silver"});
            graph.Categories.Add(new Category {Id = NodeType.Binding.ToString(), Background = "LightBlue"});
            graph.Categories.Add(new Category {Id = NodeType.Rule.ToString(), Background = "Purple"});

            return graph;
        }

        /// <summary>
        /// Sets a filter for Rete graph nodes, such that only nodes that belong to the given
        /// set of rules is serialized, along with the connecting graph edges.
        /// </summary>
        /// <param name="ruleNames">Set of rules to use as a filter, or <c>null</c> to remove the filter.</param>
        public void SetRuleFilter(IEnumerable<string> ruleNames)
        {
            _ruleNameFilter = ruleNames == null ? null : new HashSet<string>(ruleNames);
        }

        /// <summary>
        /// Sets the <see cref="IMetricsProvider"/> to retrieve performance metrics for serialized nodes,
        /// so that performance metrics are included in the output.
        /// </summary>
        /// <param name="metricsProvider">Performance metrics provider or <c>null</c> to exclude performance metrics from the output.</param>
        public void SetMetricsProvider(IMetricsProvider metricsProvider)
        {
            _metricsProvider = metricsProvider;
        }
        
        private static string GetNodeLabel(ReteNode reteNode)
        {
            var labelParts = new List<object>();
            labelParts.Add(reteNode.NodeType.ToString());
            switch (reteNode.NodeType)
            {
                case NodeType.Type:
                    labelParts.Add(reteNode.ElementType.Name);
                    break;
                case NodeType.Selection:
                    labelParts.AddRange(reteNode.Expressions.Select(x => $"{x.Value.Body}"));
                    break;
                case NodeType.Join:
                    labelParts.AddRange(reteNode.Expressions.Select(x => $"{x.Value.Body}"));
                    break;
                case NodeType.Aggregate:
                    labelParts.Add(reteNode.Properties.Single(x => x.Key == "AggregateName").Value);
                    labelParts.AddRange(reteNode.Expressions.Select(x => $"{x.Key}={x.Value.Body}"));
                    break;
                case NodeType.Binding:
                    labelParts.AddRange(reteNode.Expressions.Select(x => $"{x.Value.Body}"));
                    break;
                case NodeType.Rule:
                    labelParts.Add(reteNode.Rules.Single().Name);
                    break;
            }

            var label = string.Join("\n", labelParts);
            return label;
        }

        private void AddPerformanceMetrics(Node node, ReteNode reteNode)
        {
            INodeMetrics nodeMetrics = _metricsProvider?.FindByNodeId(reteNode.Id);
            if (nodeMetrics == null) return;

            if (nodeMetrics.ElementCount.HasValue)
                node.Properties.Add("Perf_ElementCount", nodeMetrics.ElementCount.Value);
            node.Properties.Add("Perf_InsertCount", nodeMetrics.InsertCount);
            node.Properties.Add("Perf_UpdateCount", nodeMetrics.UpdateCount);
            node.Properties.Add("Perf_RetractCount", nodeMetrics.RetractCount);
            node.Properties.Add("Perf_InsertDurationMilliseconds", nodeMetrics.InsertDurationMilliseconds);
            node.Properties.Add("Perf_UpdateDurationMilliseconds", nodeMetrics.UpdateDurationMilliseconds);
            node.Properties.Add("Perf_RetractDurationMilliseconds", nodeMetrics.RetractDurationMilliseconds);
        }

        private string Id(ReteNode reteNode)
        {
            return reteNode.Id.ToString();
        }
        
        private IEnumerable<ReteNode> Filter(ReteNode[] reteNodes)
        {
            foreach (var reteNode in reteNodes)
            {
                if (reteNode.NodeType == NodeType.Root) yield return reteNode;
                else if (reteNode.NodeType == NodeType.Dummy) yield return reteNode;
                else if (Accept(reteNode)) yield return reteNode;
            }
        }
        
        private IEnumerable<ReteLink> Filter(ReteLink[] reteLinks)
        {
            foreach (var reteLink in reteLinks)
            {
                if (Accept(reteLink.Source)) yield return reteLink;
                else if (Accept(reteLink.Target)) yield return reteLink;
            }
        }

        private bool Accept(ReteNode reteNode)
        {
            if (_ruleNameFilter == null)
                return true;
            if (reteNode.Rules.Any(r => _ruleNameFilter.Contains(r.Name)))
                return true;
            return false;
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
