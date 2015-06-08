using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NRules.Diagnostics;

namespace NRules.Debugger.Visualizer
{
    public class DgmlWriter
    {
        private readonly Dictionary<NodeInfo, int> _idMap;
        private readonly SessionSnapshot _snapshot;
        private readonly XNamespace _namespace = XNamespace.Get("http://schemas.microsoft.com/vs/2009/dgml");

        public DgmlWriter(SessionSnapshot snapshot)
        {
            _snapshot = snapshot;

            _idMap = snapshot.Nodes
                .Select((x, i) => new {Node = x, Index = i})
                .ToDictionary(x => x.Node, x => x.Index);
        }

        public void WriteTo(XmlWriter writer)
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

            document.WriteTo(writer);
        }

        private void WriteNodes(XElement nodes)
        {
            foreach (NodeInfo nodeInfo in _snapshot.Nodes)
            {
                var labelComponents = new[] {nodeInfo.NodeType.ToString(), nodeInfo.Details}
                    .Union(nodeInfo.Conditions)
                    .Union(nodeInfo.Expressions)
                    .Where(x => !string.IsNullOrEmpty(x));
                string label = string.Join("\n", labelComponents);
                var node = new XElement(Name("Node"),
                                        new XAttribute("Id", Id(nodeInfo)),
                                        new XAttribute("Category", nodeInfo.NodeType),
                                        new XAttribute("Label", label));
                if (nodeInfo.Items.Length > 0)
                {
                    node.Add(new XAttribute("Group", "Collapsed"));
                }
                nodes.Add(node);

                for (int i = 0; i < nodeInfo.Items.Length; i++)
                {
                    var itemNode = new XElement(Name("Node"),
                        new XAttribute("Id", SubNodeId(nodeInfo, i)),
                        new XAttribute("Label", nodeInfo.Items[i]),
                        new XAttribute("Style", "Plain"));
                    nodes.Add(itemNode);
                }
            }
        }

        private void WriteLinks(XElement links)
        {
            foreach (var linkInfo in _snapshot.Links)
            {
                var link = new XElement(Name("Link"),
                                        new XAttribute("Source", Id(linkInfo.Source)),
                                        new XAttribute("Target", Id(linkInfo.Target)));
                links.Add(link);
            }
            foreach (var nodeInfo in _snapshot.Nodes)
            {
                for (int i = 0; i < nodeInfo.Items.Length; i++)
                {
                    var link = new XElement(Name("Link"),
                                            new XAttribute("Source", Id(nodeInfo)),
                                            new XAttribute("Target", SubNodeId(nodeInfo, i)),
                                            new XAttribute("Category", "Contains"));
                    links.Add(link);
                }
            }
        }

        private void WriteCategories(XElement categories)
        {
            categories.Add(Category(NodeType.Root, "Black"));
            categories.Add(Category(NodeType.Type, "Orange"));
            categories.Add(Category(NodeType.Selection, "Blue"));
            categories.Add(Category(NodeType.AlphaMemory, "Red"));
            categories.Add(Category(NodeType.Dummy, "Grey"));
            categories.Add(Category(NodeType.Join, "Blue"));
            categories.Add(Category(NodeType.Not, "Brown"));
            categories.Add(Category(NodeType.Exists, "Brown"));
            categories.Add(Category(NodeType.Aggregate, "Brown"));
            categories.Add(Category(NodeType.BetaMemory, "Green"));
            categories.Add(Category(NodeType.Adapter, "Grey"));
            categories.Add(Category(NodeType.Terminal, "Grey"));
            categories.Add(Category(NodeType.Rule, "Purple"));
        }

        private XElement Category(NodeType category, string background)
        {
            return new XElement(Name("Category"),
                                new XAttribute("Id", category.ToString()),
                                new XAttribute("Label", category.ToString()),
                                new XAttribute("Background", background));
        }

        private XName Name(string name)
        {
            return _namespace + name;
        }

        private int Id(NodeInfo nodeInfo)
        {
            return _idMap[nodeInfo];
        }

        private string SubNodeId(NodeInfo nodeInfo, int itemIndex)
        {
            return string.Format("{0}_{1}", Id(nodeInfo), itemIndex);
        }
    }
}