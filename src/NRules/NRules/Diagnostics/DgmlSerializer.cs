using System.Xml.Linq;

namespace NRules.Diagnostics
{
    public class DgmlSerializer
    {
        private readonly SessionSnapshot _snapshot;
        private readonly XNamespace _namespace = XNamespace.Get("http://schemas.microsoft.com/vs/2009/dgml");

        public DgmlSerializer(SessionSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public string Serialize()
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

            return document.ToString();
        }

        private void WriteNodes(XElement nodes)
        {
            foreach (ReteNodeInfo nodeInfo in _snapshot.Nodes)
            {
                var node = new XElement(Name("Node"),
                                        new XAttribute("Id", nodeInfo.Id),
                                        new XAttribute("Category", nodeInfo.NodeType),
                                        new XAttribute("Label", string.Format("{0}\n{1}", nodeInfo.NodeType, nodeInfo.Details)));
                nodes.Add(node);
            }
        }

        private void WriteLinks(XElement links)
        {
            foreach (var linkInfo in _snapshot.Links)
            {
                var link = new XElement(Name("Link"),
                                        new XAttribute("Source", linkInfo.Item1.Id),
                                        new XAttribute("Target", linkInfo.Item2.Id));
                links.Add(link);
            }
        }

        private void WriteCategories(XElement categories)
        {
            categories.Add(Category(ReteNodeType.Root, "Black"));
            categories.Add(Category(ReteNodeType.Type, "Yellow"));
            categories.Add(Category(ReteNodeType.Selection, "Blue"));
            categories.Add(Category(ReteNodeType.AlphaMemory, "Red"));
            categories.Add(Category(ReteNodeType.Dummy, "Grey"));
            categories.Add(Category(ReteNodeType.Join, "Blue"));
            categories.Add(Category(ReteNodeType.Not, "Brown"));
            categories.Add(Category(ReteNodeType.Exists, "Brown"));
            categories.Add(Category(ReteNodeType.Aggregate, "Brown"));
            categories.Add(Category(ReteNodeType.BetaMemory, "Green"));
            categories.Add(Category(ReteNodeType.Adapter, "Grey"));
            categories.Add(Category(ReteNodeType.Terminal, "Grey"));
            categories.Add(Category(ReteNodeType.Rule, "Purple"));
        }

        private XElement Category(ReteNodeType category, string background)
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
    }
}