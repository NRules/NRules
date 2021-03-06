using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace NRules.Diagnostics.Dgml
{
    /// <summary>
    /// Creates a <see href="https://en.wikipedia.org/wiki/DGML">DGML</see> document writer
    /// that can be used to serialize a Rete graph for a given rules session into XML.
    /// </summary>
    public class DgmlWriter
    {
        private readonly ReteGraph _schema;
        private readonly XNamespace _namespace = XNamespace.Get("http://schemas.microsoft.com/vs/2009/dgml");

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
            var document = GetDocument();
            document.WriteTo(writer);
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

        private XDocument GetDocument()
        {
            var document = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var root = new XElement(Name("DirectedGraph"), new XAttribute("Title", "ReteNetwork"));
            var nodes = new XElement(Name("Nodes"));
            var links = new XElement(Name("Links"));
            var categories = new XElement(Name("Categories"));

            WriteNodes(nodes);
            WriteLinks(links);
            WriteCategories(categories);

            root.Add(nodes, links, categories);
            document.Add(root);
            return document;
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

        private void WriteNodes(XElement nodes)
        {
            foreach (var reteNode in Filter(_schema.Nodes))
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
                var node = new XElement(Name("Node"),
                                        new XAttribute("Id", Id(reteNode)),
                                        new XAttribute("Category", reteNode.NodeType),
                                        new XAttribute("Label", label));

                if (reteNode.ElementType?.FullName != null)
                {
                    node.Add(new XAttribute("ElementType", reteNode.ElementType.FullName));
                }

                foreach (var valueGroup in reteNode.Properties.GroupBy(x => x.Key, x => x.Value))
                {
                    var key = valueGroup.Key;
                    var value = string.Join("; ", valueGroup);
                    node.Add(new XAttribute(key, value));
                }
                
                foreach (var expressionGroup in reteNode.Expressions.GroupBy(x => x.Key, x => x.Value))
                {
                    var key = expressionGroup.Key;
                    var value = string.Join("; ", expressionGroup);
                    node.Add(new XAttribute(key, value));
                }

                if (reteNode.Rules.Length > 0)
                {
                    var value = string.Join("; ", reteNode.Rules.Select(x => x.Name));
                    node.Add(new XAttribute("Rule", value));
                }

                WritePerformanceMetrics(node, reteNode);

                nodes.Add(node);
            }
        }

        private void WritePerformanceMetrics(XElement node, ReteNode reteNode)
        {
            INodeMetrics nodeMetrics = _metricsProvider?.FindByNodeId(reteNode.Id);
            if (nodeMetrics == null) return;

            if (nodeMetrics.ElementCount.HasValue)
                node.Add(new XAttribute("Perf_ElementCount", nodeMetrics.ElementCount.Value));
            node.Add(new XAttribute("Perf_InsertCount", nodeMetrics.InsertCount));
            node.Add(new XAttribute("Perf_UpdateCount", nodeMetrics.UpdateCount));
            node.Add(new XAttribute("Perf_RetractCount", nodeMetrics.RetractCount));
            node.Add(new XAttribute("Perf_InsertDurationMilliseconds", nodeMetrics.InsertDurationMilliseconds));
            node.Add(new XAttribute("Perf_UpdateDurationMilliseconds", nodeMetrics.UpdateDurationMilliseconds));
            node.Add(new XAttribute("Perf_RetractDurationMilliseconds", nodeMetrics.RetractDurationMilliseconds));
        }

        private void WriteLinks(XElement links)
        {
            foreach (var linkInfo in Filter(_schema.Links))
            {
                var link = new XElement(Name("Link"),
                    new XAttribute("Source", Id(linkInfo.Source)),
                    new XAttribute("Target", Id(linkInfo.Target)));
                links.Add(link);
            }
        }

        private void WriteCategories(XElement categories)
        {
            categories.Add(Category(NodeType.Root, "Black"));
            categories.Add(Category(NodeType.Type, "Orange"));
            categories.Add(Category(NodeType.Selection, "Blue"));
            categories.Add(Category(NodeType.AlphaMemory, "Red"));
            categories.Add(Category(NodeType.Dummy, "Silver"));
            categories.Add(Category(NodeType.Join, "Blue"));
            categories.Add(Category(NodeType.Not, "Brown"));
            categories.Add(Category(NodeType.Exists, "Brown"));
            categories.Add(Category(NodeType.Aggregate, "Brown"));
            categories.Add(Category(NodeType.BetaMemory, "Green"));
            categories.Add(Category(NodeType.Adapter, "Silver"));
            categories.Add(Category(NodeType.Binding, "LightBlue"));
            categories.Add(Category(NodeType.Rule, "Purple"));
        }

        private XElement Category(NodeType category, string background)
        {
            return new XElement(Name("Category"),
                new XAttribute("Id", category.ToString()),
                new XAttribute("Background", background));
        }

        private XName Name(string name)
        {
            return _namespace + name;
        }

        private int Id(ReteNode reteNode)
        {
            return reteNode.Id;
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
