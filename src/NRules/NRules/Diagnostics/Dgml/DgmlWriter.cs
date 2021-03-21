using System;
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
            var reteNodes = FilterNodes(_ruleNameFilter, _schema.Nodes).ToArray();
            var reteLinks = FilterLinks(reteNodes, _schema.Links).ToArray();

            graph.Nodes.AddRange(CreateNodes(reteNodes));
            graph.Links.AddRange(CreateLinks(reteLinks));
            graph.Categories.AddRange(CreateNodeTypeCategories());

            if (_metricsProvider != null)
                AddPerformanceMetrics(graph);
            else
                graph.Styles.AddRange(CreateSchemaStyles());

            return graph;
        }

        private IEnumerable<Node> CreateNodes(IEnumerable<ReteNode> reteNodes)
        {
            foreach (var reteNode in reteNodes)
            {
                var node = new Node(Id(reteNode))
                {
                    Label = GetNodeLabel(reteNode),
                    Category = reteNode.NodeType.ToString()
                };

                node.Properties.Add("OutputType", reteNode.OutputType?.ToString());

                foreach (var valueGroup in reteNode.Properties.GroupBy(x => x.Key, x => x.Value))
                {
                    var key = valueGroup.Key;
                    var value = string.Join("; ", valueGroup);
                    node.Properties.Add($"{reteNode.NodeType}{key}", value);
                }

                foreach (var expressionGroup in reteNode.Expressions.GroupBy(x => x.Key, x => x.Value))
                {
                    var key = expressionGroup.Key;
                    var value = string.Join("; ", expressionGroup);
                    node.Properties.Add($"{reteNode.NodeType}{key}", value);
                }

                if (reteNode.Rules.Length > 0)
                {
                    var value = string.Join("; ", reteNode.Rules.Select(x => x.Name));
                    node.Properties.Add("Rule", value);
                }

                if (reteNode.Rules.Length > 1)
                {
                    node.Properties.Add("Shared", true);
                }

                yield return node;
            }
        }

        private IEnumerable<Link> CreateLinks(IEnumerable<ReteLink> reteLinks)
        {
            foreach (var linkInfo in reteLinks)
            {
                yield return new Link(Id(linkInfo.Source), Id(linkInfo.Target));
            }
        }

        private static IEnumerable<Category> CreateNodeTypeCategories()
        {
            foreach (var nodeType in Enum.GetValues(typeof(NodeType)))
            {
                yield return new Category($"{nodeType}");
            }
        }
        
        private static string GetNodeLabel(ReteNode reteNode)
        {
            var labelParts = new List<object>();
            labelParts.Add(reteNode.NodeType.ToString());
            switch (reteNode.NodeType)
            {
                case NodeType.Type:
                    labelParts.Add(reteNode.OutputType.Name);
                    break;
                case NodeType.Selection:
                    labelParts.AddRange(reteNode.Expressions.Select(x => $"{x.Value.Body}"));
                    break;
                case NodeType.Join:
                    labelParts.AddRange(reteNode.Expressions.Select(x => $"{x.Value.Body}"));
                    break;
                case NodeType.Aggregate:
                    labelParts.Add(reteNode.Properties.Single(x => x.Key == "Name").Value);
                    labelParts.AddRange(reteNode.Expressions.Select(x => $"{x.Key}={x.Value.Body}"));
                    break;
                case NodeType.Binding:
                    labelParts.AddRange(reteNode.Expressions.Select(x => $"{x.Value.Body}"));
                    break;
                case NodeType.Rule:
                    labelParts.Add(reteNode.Rules.Single().Name);
                    labelParts.AddRange(reteNode.Expressions.Select(x => $"{x.Key}={x.Value.Body}"));
                    break;
            }

            var label = string.Join("\n", labelParts);
            return label;
        }

        private void AddPerformanceMetrics(DirectedGraph graph)
        {
            long maxDuration = 0;
            long minDuration = Int64.MaxValue;
            var reteNodeLookup = _schema.Nodes.ToDictionary(Id);
            foreach (var node in graph.Nodes)
            {
                var reteNode = reteNodeLookup[node.Id];
                INodeMetrics nodeMetrics = _metricsProvider?.FindByNodeId(reteNode.Id);
                if (nodeMetrics == null) continue;

                var totalDuration = nodeMetrics.InsertDurationMilliseconds +
                                    nodeMetrics.UpdateDurationMilliseconds +
                                    nodeMetrics.RetractDurationMilliseconds;
                maxDuration = Math.Max(maxDuration, totalDuration);
                minDuration = Math.Min(minDuration, totalDuration);
                AddPerformanceMetrics(node, nodeMetrics);
            }

            minDuration = Math.Min(minDuration, maxDuration);

            graph.Properties.AddRange(
                CreatePerformanceProperties());
            graph.Styles.AddRange(
                CreatePerformanceStyles(minDuration, maxDuration));
        }

        private IEnumerable<Property> CreatePerformanceProperties()
        {
            yield return new Property("PerfElementCount") {DataType = "System.Int32"};
            yield return new Property("PerfInsertInputCount") {DataType = "System.Int32"};
            yield return new Property("PerfInsertOutputCount") {DataType = "System.Int32"};
            yield return new Property("PerfUpdateInputCount") {DataType = "System.Int32"};
            yield return new Property("PerfUpdateOutputCount") {DataType = "System.Int32"};
            yield return new Property("PerfRetractInputCount") {DataType = "System.Int32"};
            yield return new Property("PerfRetractOutputCount") {DataType = "System.Int32"};
            yield return new Property("PerfInsertDurationMilliseconds") {DataType = "System.Int64"};
            yield return new Property("PerfUpdateDurationMilliseconds") {DataType = "System.Int64"};
            yield return new Property("PerfRetractDurationMilliseconds") {DataType = "System.Int64"};
            yield return new Property("PerfTotalInputCount") {DataType = "System.Int32"};
            yield return new Property("PerfTotalOutputCount") {DataType = "System.Int32"};
            yield return new Property("PerfTotalDurationMilliseconds") {DataType = "System.Int64"};
        }

        private void AddPerformanceMetrics(Node node, INodeMetrics nodeMetrics)
        {
            if (nodeMetrics.ElementCount.HasValue)
                node.Properties.Add("PerfElementCount", nodeMetrics.ElementCount.Value);
            node.Properties.Add("PerfInsertInputCount", nodeMetrics.InsertInputCount);
            node.Properties.Add("PerfInsertOutputCount", nodeMetrics.InsertOutputCount);
            node.Properties.Add("PerfUpdateInputCount", nodeMetrics.UpdateInputCount);
            node.Properties.Add("PerfUpdateOutputCount", nodeMetrics.UpdateOutputCount);
            node.Properties.Add("PerfRetractInputCount", nodeMetrics.RetractInputCount);
            node.Properties.Add("PerfRetractOutputCount", nodeMetrics.RetractOutputCount);
            node.Properties.Add("PerfInsertDurationMilliseconds", nodeMetrics.InsertDurationMilliseconds);
            node.Properties.Add("PerfUpdateDurationMilliseconds", nodeMetrics.UpdateDurationMilliseconds);
            node.Properties.Add("PerfRetractDurationMilliseconds", nodeMetrics.RetractDurationMilliseconds);
            node.Properties.Add("PerfTotalInputCount",
                nodeMetrics.InsertInputCount + nodeMetrics.UpdateInputCount + nodeMetrics.RetractInputCount);
            node.Properties.Add("PerfTotalOutputCount",
                nodeMetrics.InsertOutputCount + nodeMetrics.UpdateOutputCount + nodeMetrics.RetractOutputCount);
            node.Properties.Add("PerfTotalDurationMilliseconds",
                nodeMetrics.InsertDurationMilliseconds + nodeMetrics.UpdateDurationMilliseconds + nodeMetrics.RetractDurationMilliseconds);
        }
        
        private IEnumerable<Style> CreatePerformanceStyles(long minDuration, long maxDuration)
        {
            var flowProperty = "Source.PerfTotalOutputCount";
            yield return new Style("Link")
                .Condition($"{flowProperty} > 0")
                .Setter(nameof(Link.StrokeThickness),
                    expression: $"Math.Min(25,Math.Max(1,Math.Log({flowProperty},2)))");

            var countProperty = "PerfElementCount";
            yield return new Style("Node")
                .Condition($"HasCategory('{NodeType.AlphaMemory}') or HasCategory('{NodeType.BetaMemory}')")
                .Condition($"{countProperty} > 0")
                .Setter(nameof(Node.FontSize),
                    expression: $"Math.Min(72,Math.Max(8,8+4*Math.Log({countProperty},2)))");

            var durationProperty = "PerfTotalDurationMilliseconds";
            maxDuration = Math.Max(50, maxDuration);
            long basis = maxDuration - minDuration;
            int maxRed = 200;
            int maxGreen = 200;
            int maxBlue = 200;
            yield return new Style("Node")
                .Condition($"{durationProperty} > 0")
                .Setter(nameof(Node.Foreground), value: "Black")
                .Setter(nameof(Node.Background), 
                    expression: $"Color.FromRgb({maxRed},({maxGreen}*({maxDuration}-{durationProperty}))/{basis},({maxBlue}*({maxDuration}-{durationProperty}))/{basis})");
            yield return new Style("Node")
                .Setter(nameof(Node.Foreground), value: "Black")
                .Setter(nameof(Node.Background),
                    expression: $"Color.FromRgb({maxRed},{maxGreen},{maxBlue})");
        }
        
        private IEnumerable<Style> CreateSchemaStyles()
        {
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Root}")
                .Setter(nameof(Node.Background), value: "Black");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Dummy}")
                .Setter(nameof(Node.Background), value: "Silver");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Type}")
                .Setter(nameof(Node.Background), value: "Orange");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Selection}")
                .Setter(nameof(Node.Background), value: "Blue");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.AlphaMemory}")
                .Setter(nameof(Node.Background), value: "Red");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Join}")
                .Setter(nameof(Node.Background), value: "Blue");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Not}")
                .Setter(nameof(Node.Background), value: "Brown");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Exists}")
                .Setter(nameof(Node.Background), value: "Brown");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Aggregate}")
                .Setter(nameof(Node.Background), value: "Brown");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Adapter}")
                .Setter(nameof(Node.Background), value: "Silver");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.BetaMemory}")
                .Setter(nameof(Node.Background), value: "Green");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Binding}")
                .Setter(nameof(Node.Background), value: "LightBlue");
            yield return new Style(nameof(Node))
                .HasCategory($"{NodeType.Rule}")
                .Setter(nameof(Node.Background), value: "Purple");
        }
        
        private static IEnumerable<ReteNode> FilterNodes(HashSet<string> ruleNameFilter, ReteNode[] reteNodes)
        {
            foreach (var reteNode in reteNodes)
            {
                if (ruleNameFilter == null ||
                    reteNode.NodeType == NodeType.Root ||
                    reteNode.NodeType == NodeType.Dummy ||
                    reteNode.Rules.Any(r => ruleNameFilter.Contains(r.Name)))
                    yield return reteNode;
            }
        }

        private static IEnumerable<ReteLink> FilterLinks(ReteNode[] reteNodes, ReteLink[] reteLinks)
        {
            var reteNodeIds = new HashSet<int>(reteNodes.Select(x => x.Id));
            foreach (var reteLink in reteLinks)
            {
                if (reteNodeIds.Contains(reteLink.Source.Id) &&
                    reteNodeIds.Contains(reteLink.Target.Id))
                    yield return reteLink;
            }
        }

        private string Id(ReteNode reteNode)
        {
            return $"{reteNode.Id}";
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
