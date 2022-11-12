using NRules.Diagnostics;
using NRules.Diagnostics.Dgml;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.Rete;
using NRules.RuleModel;
using Xunit;

namespace NRules.Tests.Rete;

public class ReteGraphConverterTests
{
    [Fact]
    public void ConvertToDirectedGraph_WhenProvidedFilter_ShouldFilterNodes()
    {
        var schema = CreateSut();
        string mustInclude = typeof(SampleRule).FullName!;
        string mustExclude = typeof(SampleRule2).FullName!;

        var graph = schema.ConvertToDirectedGraph(new[] { mustInclude });

        Assert.Contains(graph.Nodes, node => node.Category == nameof(NodeType.Root));
        Assert.Contains(graph.Nodes, node => node.Category == nameof(NodeType.Dummy));
        Assert.Contains(graph.Nodes.Where(node => node.Properties.ContainsKey("Rule")), node => node.Properties["Rule"]!.ToString()!.Contains(mustInclude));
        Assert.DoesNotContain(graph.Nodes.Where(node => node.Properties.ContainsKey("Rule")), node => node.Properties["Rule"]!.ToString()!.Contains(mustExclude));
    }

    [Fact]
    public void ConvertToDirectedGraph_WhenProvidedFilter_ShouldFilterLinks()
    {
        var schema = CreateSut();
        string mustInclude = typeof(SampleRule).FullName!;
        string mustExclude = typeof(SampleRule2).FullName!;

        var graph = schema.ConvertToDirectedGraph(new[] { mustInclude });

        var ids = graph.Nodes.Select(node => node.Id).ToArray();
        Assert.Contains(graph.Links, link => ids.Contains(link.Source));
        Assert.Contains(graph.Links, link => ids.Contains(link.Target));
    }

    [Fact]
    public void ConvertToDirectedGraph_WhenNotProvidedFilter_ShouldNotFilterNodes()
    {
        var schema = CreateSut();
        string mustInclude = typeof(SampleRule).FullName!;
        string mustInclude2 = typeof(SampleRule2).FullName!;

        var graph = schema.ConvertToDirectedGraph();

        Assert.Contains(graph.Nodes, node => node.Category == nameof(NodeType.Root));
        Assert.Contains(graph.Nodes, node => node.Category == nameof(NodeType.Dummy));
        Assert.Contains(graph.Nodes.Where(node => node.Properties.ContainsKey("Rule")), node => node.Properties["Rule"]!.ToString()!.Contains(mustInclude));
        Assert.Contains(graph.Nodes.Where(node => node.Properties.ContainsKey("Rule")), node => node.Properties["Rule"]!.ToString()!.Contains(mustInclude2));
    }

    [Fact]
    public void ConvertToDirectedGraph_WhenNotProvidedFilter_ShouldNotFilterLinks()
    {
        var schema = CreateSut();

        var graph = schema.ConvertToDirectedGraph();

        var ids = graph.Nodes.Select(node => node.Id).ToArray();
        Assert.Contains(graph.Links, link => ids.Contains(link.Source));
        Assert.Contains(graph.Links, link => ids.Contains(link.Target));
    }

    [Fact]
    public void ConvertToDirectedGraph_WhenCalled_ShouldFillCategories()
    {
        var schema = CreateSut();

        var graph = schema.ConvertToDirectedGraph();

        var ids = graph.Categories.Select(node => node.Id).ToArray();
        foreach (var type in Enum.GetNames(typeof(NodeType)))
        {
            Assert.Contains(ids, id => id == type);
        }
    }

    [Fact]
    public void ConvertToDirectedGraph_WhenNotProvidedMetricsProvider_ShouldFillDefaultStyles()
    {
        var schema = CreateSut();

        var graph = schema.ConvertToDirectedGraph();

        Assert.All(graph.Styles, style =>
        {
            Assert.Equal(nameof(Node), style.TargetType);

            Assert.NotEmpty(style.Conditions);
            Assert.Equal(1, style.Conditions.Count);
            var condition = style.Conditions.Single();
            Assert.StartsWith("HasCategory('", condition.Expression);
            Assert.EndsWith("')", condition.Expression);

            Assert.NotEmpty(style.Setters);
            Assert.Equal(1, style.Setters.Count);
            var setter = style.Setters.Single();
            Assert.Equal(nameof(Node.Background), setter.Property);
            Assert.NotNull(setter.Value);
            Assert.Null(setter.Expression);
        });

        foreach (var type in Enum.GetNames(typeof(NodeType)))
        {
            Assert.Contains(graph.Styles, style => style.Conditions.Single().Expression.Contains(type));
        }
    }

    [Fact]
    public void ConvertToDirectedGraph_WhenProvidedMetricsProvider_ShouldFillProperties()
    {
        var schema = CreateSut();

        var graph = schema.ConvertToDirectedGraph(metricsProvider: MetricsProvider);

        Assert.All(graph.Properties, property =>
        {
            if (property.Id.EndsWith("Milliseconds"))
            {
                Assert.Equal(typeof(long).FullName, property.DataType);
                return;
            }
            if (property.Id.EndsWith("Count"))
            {
                Assert.Equal(typeof(int).FullName, property.DataType);
                return;
            }
            Assert.Fail($"Unknown property id {property.Id}");
        });
    }

    [Fact]
    public void ConvertToDirectedGraph_WhenProvidedMetricsProvider_ShouldFillNodeProperties()
    {
        var schema = CreateSut();
        var metrics = MetricsProvider.GetMetrics(new RootNode(schema.Nodes[0].Id));
        metrics.ElementCount = 1;
        metrics.InsertInputCount = 2;
        metrics.UpdateInputCount = 3;
        metrics.RetractInputCount = 4;
        metrics.InsertOutputCount = 5;
        metrics.UpdateOutputCount = 6;
        metrics.RetractOutputCount = 7;
        metrics.InsertDurationMilliseconds = 8;
        metrics.UpdateDurationMilliseconds = 9;
        metrics.RetractDurationMilliseconds = 10;

        var graph = schema.ConvertToDirectedGraph(metricsProvider: MetricsProvider);

        var node = graph.Nodes.First(x => x.Id == metrics.NodeId);
        Assert.Equal(metrics.ElementCount, node.Properties["PerfElementCount"]);
        Assert.Equal(metrics.InsertInputCount, node.Properties["PerfInsertInputCount"]);
        Assert.Equal(metrics.InsertOutputCount, node.Properties["PerfInsertOutputCount"]);
        Assert.Equal(metrics.UpdateInputCount, node.Properties["PerfUpdateInputCount"]);
        Assert.Equal(metrics.UpdateOutputCount, node.Properties["PerfUpdateOutputCount"]);
        Assert.Equal(metrics.RetractInputCount, node.Properties["PerfRetractInputCount"]);
        Assert.Equal(metrics.RetractOutputCount, node.Properties["PerfRetractOutputCount"]);
        Assert.Equal(metrics.InsertDurationMilliseconds, node.Properties["PerfInsertDurationMilliseconds"]);
        Assert.Equal(metrics.UpdateDurationMilliseconds, node.Properties["PerfUpdateDurationMilliseconds"]);
        Assert.Equal(metrics.RetractDurationMilliseconds, node.Properties["PerfRetractDurationMilliseconds"]);
        Assert.Equal(metrics.InsertInputCount + metrics.UpdateInputCount + metrics.RetractInputCount, node.Properties["PerfTotalInputCount"]);
        Assert.Equal(metrics.InsertOutputCount + metrics.UpdateOutputCount + metrics.RetractOutputCount, node.Properties["PerfTotalOutputCount"]);
        Assert.Equal(metrics.InsertDurationMilliseconds + metrics.UpdateDurationMilliseconds + metrics.RetractDurationMilliseconds, node.Properties["PerfTotalDurationMilliseconds"]);
    }

    private ReteGraph CreateSut()
    {
        var repository = new RuleRepository { Activator = new InstanceActivator() };
        SetUpRule<SampleRule>(repository);
        SetUpRule<SampleRule2>(repository);
        var factory = repository.Compile();
        var session = factory.CreateSession();
        var schema = session.GetSchema();
        return schema;

        static void SetUpRule<T>(RuleRepository repository)
            where T : Rule
        {
            repository.Load(x => x
                .PrivateTypes()
                .NestedTypes()
                .From(typeof(T)));
        }
    }

    private MetricsAggregator MetricsProvider { get; } = new();

    private class InstanceActivator : IRuleActivator
    {
        private readonly Dictionary<Type, Rule> _rules = new();

        public IEnumerable<Rule> Activate(Type type)
        {
            if (!_rules.TryGetValue(type, out var rule))
            {
                rule = Activator.CreateInstance(type) as Rule;
                _rules[type] = rule ?? throw new ArgumentException($"Cannot create rule of type {type}");
            }
            yield return rule;
        }
    }

    public class SampleFact
    {
    }

    public class SampleFact2
    {
    }

    public class SampleRule : Rule
    {
        public override void Define()
        {
            When()
                .Match<SampleFact>();
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }

    public class SampleRule2 : Rule
    {
        public override void Define()
        {
            When()
                .Match<SampleFact2>(x => false);
            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}

public static class ContextExtensions
{
    public static void NoOp(this IContext context)
    {
    }
}

