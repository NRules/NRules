namespace NRules.Diagnostics.Dgml;

internal static class ReteGraphCoverter
{
    private static readonly IReadOnlyCollection<Category> _categories = Enum.GetNames(typeof(NodeType)).Select(name => new Category(name)).ToArray();
    private static readonly IReadOnlyCollection<Style> _defaultStyles = new[]
    {
        new Style(nameof(Node)).HasCategory($"{NodeType.Root}").SetBackground("Black"),
        new Style(nameof(Node)).HasCategory($"{NodeType.Dummy}").SetBackground("Silver"),
        new Style(nameof(Node)).HasCategory($"{NodeType.Type}").SetBackground("Orange"),
        new Style(nameof(Node)).HasCategory($"{NodeType.Selection}").SetBackground("Blue"),
        new Style(nameof(Node)).HasCategory($"{NodeType.AlphaMemory}").SetBackground("Red"),
        new Style(nameof(Node)).HasCategory($"{NodeType.Join}").SetBackground("Blue"),
        new Style(nameof(Node)).HasCategory($"{NodeType.Not}").SetBackground("Brown"),
        new Style(nameof(Node)).HasCategory($"{NodeType.Exists}").SetBackground("Brown"),
        new Style(nameof(Node)).HasCategory($"{NodeType.Aggregate}").SetBackground("Brown"),
        new Style(nameof(Node)).HasCategory($"{NodeType.Adapter}").SetBackground("Silver"),
        new Style(nameof(Node)).HasCategory($"{NodeType.BetaMemory}").SetBackground("Green"),
        new Style(nameof(Node)).HasCategory($"{NodeType.Binding}").SetBackground("LightBlue"),
        new Style(nameof(Node)).HasCategory($"{NodeType.Rule}").SetBackground("Purple"),
    };

    private static readonly IReadOnlyCollection<Property> _performanceProperties = new Property[]
    {
        new("PerfElementCount", "System.Int32"),
        new("PerfInsertInputCount", "System.Int32"),
        new("PerfInsertOutputCount", "System.Int32"),
        new("PerfUpdateInputCount", "System.Int32"),
        new("PerfUpdateOutputCount", "System.Int32"),
        new("PerfRetractInputCount", "System.Int32"),
        new("PerfRetractOutputCount", "System.Int32"),
        new("PerfInsertDurationMilliseconds", "System.Int64"),
        new("PerfUpdateDurationMilliseconds", "System.Int64"),
        new("PerfRetractDurationMilliseconds", "System.Int64"),
        new("PerfTotalInputCount", "System.Int32"),
        new("PerfTotalOutputCount", "System.Int32"),
        new("PerfTotalDurationMilliseconds", "System.Int64"),
    };

    public static DirectedGraph ConvertToDirectedGraph(this ReteGraph schema, ICollection<string>? ruleNameFilter = null, IMetricsProvider? metricsProvider = null)
    {
        var filteredSchema = schema.Filter(ruleNameFilter);

        var graph = new DirectedGraph("ReteNetwork");
        graph.AddRange(CreateNodes(filteredSchema.Nodes));
        graph.AddRange(CreateLinks(filteredSchema.Links));
        graph.AddRange(_categories);

        if (metricsProvider is null)
        {
            graph.AddRange(_defaultStyles);
        }
        else
        {
            graph.AddRange(_performanceProperties);
            graph.AddRange(GetPerformanceMetricsStyles(schema.Nodes, graph.Nodes, metricsProvider));
        }

        return graph;
    }

    private static ReteGraph Filter(this ReteGraph source, ICollection<string>? ruleNameFilter = null)
    {
        if (ruleNameFilter is null)
            return source;

        var reteNodes = source.Nodes.Filter(ruleNameFilter).ToArray();
        var reteLinks = source.Links.Filter(reteNodes);

        return new ReteGraph(reteNodes, reteLinks);
    }

    private static IEnumerable<ReteNode> Filter(this IEnumerable<ReteNode> reteNodes, ICollection<string> ruleNameFilter) =>
        reteNodes.Where(reteNode =>
            reteNode.NodeType is NodeType.Root or NodeType.Dummy ||
            reteNode.Rules.Any(r => ruleNameFilter.Contains(r.Name)));


    private static IEnumerable<ReteLink> Filter(this IEnumerable<ReteLink> reteLinks, IEnumerable<ReteNode> reteNodes)
    {
        var reteNodeIds = new HashSet<int>(reteNodes.Select(x => x.Id));
        foreach (var reteLink in reteLinks)
        {
            if (reteNodeIds.Contains(reteLink.Source.Id) &&
                reteNodeIds.Contains(reteLink.Target.Id))
                yield return reteLink;
        }
    }

    private static IEnumerable<Node> CreateNodes(IEnumerable<ReteNode> reteNodes) => reteNodes.Select(CreateNode);

    private static Node CreateNode(ReteNode reteNode)
    {
        var node = new Node(reteNode.Id(), reteNode.GetNodeLabel(), reteNode.NodeType.ToString());

        node.AddProperty("OutputType", reteNode.OutputType);

        foreach (var valueGroup in reteNode.Properties.GroupBy(x => x.Key, x => x.Value))
        {
            var key = valueGroup.Key;
            var value = string.Join("; ", valueGroup);
            node.AddProperty($"{reteNode.NodeType}{key}", value);
        }

        foreach (var expressionGroup in reteNode.Expressions.GroupBy(x => x.Key, x => x.Value))
        {
            var key = expressionGroup.Key;
            var value = string.Join("; ", expressionGroup);
            node.AddProperty($"{reteNode.NodeType}{key}", value);
        }

        if (reteNode.Rules.Length > 0)
        {
            var value = string.Join("; ", reteNode.Rules.Select(x => x.Name));
            node.AddProperty("Rule", value);
        }

        if (reteNode.Rules.Length > 1)
        {
            node.AddProperty("Shared", true);
        }

        return node;
    }

    private static string Id(this ReteNode reteNode) => reteNode.Id.ToString();

    private static string GetNodeLabel(this ReteNode reteNode)
    {
        var labelParts = new List<object> { reteNode.NodeType.ToString() };
        switch (reteNode.NodeType)
        {
            case NodeType.Type:
                labelParts.Add(reteNode.OutputType?.Name ?? throw new ArgumentException($"{nameof(reteNode.OutputType)} is null", nameof(reteNode)));
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

        return string.Join("\n", labelParts);
    }

    private static IEnumerable<Link> CreateLinks(IEnumerable<ReteLink> reteLinks) => reteLinks.Select(linkInfo => new Link(Id(linkInfo.Source), Id(linkInfo.Target)));

    private static IEnumerable<Style> GetPerformanceMetricsStyles(IEnumerable<ReteNode> reteNodes, IEnumerable<Node> nodes, IMetricsProvider provider)
    {
        var maxDuration = 0L;
        var minDuration = long.MaxValue;
        var reteNodeLookup = reteNodes.ToDictionary(Id);
        foreach (var node in nodes)
        {
            var reteNode = reteNodeLookup[node.Id];
            var nodeMetrics = provider.FindByNodeId(reteNode.Id);
            if (nodeMetrics == null)
                continue;

            var totalDuration = nodeMetrics.TotalDuration();
            maxDuration = Math.Max(maxDuration, totalDuration);
            minDuration = Math.Min(minDuration, totalDuration);
            node.AddPerformanceMetricsProperties(nodeMetrics);
        }

        minDuration = Math.Min(minDuration, maxDuration);

        return CreatePerformanceStyles(minDuration, maxDuration);
    }

    private static void AddPerformanceMetricsProperties(this Node node, INodeMetrics nodeMetrics)
    {
        if (nodeMetrics.ElementCount.HasValue)
            node.AddProperty("PerfElementCount", nodeMetrics.ElementCount.Value);
        node.AddProperty("PerfInsertInputCount", nodeMetrics.InsertInputCount);
        node.AddProperty("PerfInsertOutputCount", nodeMetrics.InsertOutputCount);
        node.AddProperty("PerfUpdateInputCount", nodeMetrics.UpdateInputCount);
        node.AddProperty("PerfUpdateOutputCount", nodeMetrics.UpdateOutputCount);
        node.AddProperty("PerfRetractInputCount", nodeMetrics.RetractInputCount);
        node.AddProperty("PerfRetractOutputCount", nodeMetrics.RetractOutputCount);
        node.AddProperty("PerfInsertDurationMilliseconds", nodeMetrics.InsertDurationMilliseconds);
        node.AddProperty("PerfUpdateDurationMilliseconds", nodeMetrics.UpdateDurationMilliseconds);
        node.AddProperty("PerfRetractDurationMilliseconds", nodeMetrics.RetractDurationMilliseconds);
        node.AddProperty("PerfTotalInputCount", nodeMetrics.TotalInputCount());
        node.AddProperty("PerfTotalOutputCount", nodeMetrics.TotalOutputCount());
        node.AddProperty("PerfTotalDurationMilliseconds", nodeMetrics.TotalDuration());
    }

    private static long TotalDuration(this INodeMetrics nodeMetrics) =>
        nodeMetrics.InsertDurationMilliseconds +
        nodeMetrics.UpdateDurationMilliseconds +
        nodeMetrics.RetractDurationMilliseconds;

    private static long TotalInputCount(this INodeMetrics nodeMetrics) =>
        nodeMetrics.InsertInputCount +
        nodeMetrics.UpdateInputCount +
        nodeMetrics.RetractInputCount;

    private static long TotalOutputCount(this INodeMetrics nodeMetrics) =>
        nodeMetrics.InsertOutputCount +
        nodeMetrics.UpdateOutputCount +
        nodeMetrics.RetractOutputCount;

    private static IEnumerable<Style> CreatePerformanceStyles(long minDuration, long maxDuration)
    {
        const string flowProperty = "Source.PerfTotalOutputCount";
        yield return new Style(nameof(Link))
            .Condition($"{flowProperty} > 0")
            .Expression(nameof(Link.StrokeThickness), $"Math.Min(25,Math.Max(1,Math.Log({flowProperty},2)))");

        const string countProperty = "PerfElementCount";
        yield return new Style(nameof(Node))
            .Condition($"HasCategory('{NodeType.AlphaMemory}') or HasCategory('{NodeType.BetaMemory}')")
            .Condition($"{countProperty} > 0")
            .Expression(nameof(Node.FontSize), $"Math.Min(72,Math.Max(8,8+4*Math.Log({countProperty},2)))");

        const string durationProperty = "PerfTotalDurationMilliseconds";
        maxDuration = Math.Max(50, maxDuration);
        var basis = maxDuration - minDuration;
        const int maxRed = 200;
        const int maxGreen = 200;
        const int maxBlue = 200;
        yield return new Style(nameof(Node))
            .Condition($"{durationProperty} > 0")
            .Setter(nameof(Node.Foreground), "Black")
            .Expression(nameof(Node.Background), $"Color.FromRgb({maxRed},({maxGreen}*({maxDuration}-{durationProperty}))/{basis},({maxBlue}*({maxDuration}-{durationProperty}))/{basis})");
        yield return new Style(nameof(Node))
            .Setter(nameof(Node.Foreground), "Black")
            .Expression(nameof(Node.Background), $"Color.FromRgb({maxRed},{maxGreen},{maxBlue})");
    }
}
