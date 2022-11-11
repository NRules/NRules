using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Rete;

internal interface IReteBuilder
{
    int GetNodeId();
    IEnumerable<ITerminal> AddRule(IRuleDefinition rule);
    INetwork Build();
}

internal class ReteBuilder : RuleElementVisitor<ReteBuilderContext>, IReteBuilder
{
    private readonly RootNode _root;
    private readonly DummyNode _dummyNode;
    private readonly AggregatorRegistry _aggregatorRegistry;
    private readonly ExpressionElementComparer _expressionComparer;
    private readonly IRuleExpressionCompiler _ruleExpressionCompiler;

    private int _nextNodeId = 1;

    public ReteBuilder(RuleCompilerOptions options, AggregatorRegistry aggregatorRegistry, IRuleExpressionCompiler ruleExpressionCompiler)
    {
        _root = new RootNode(GetNodeId());
        _dummyNode = new DummyNode(GetNodeId());
        _aggregatorRegistry = aggregatorRegistry;
        _expressionComparer = new ExpressionElementComparer(options);
        _ruleExpressionCompiler = ruleExpressionCompiler;
    }

    public int GetNodeId()
    {
        return _nextNodeId++;
    }

    public IEnumerable<ITerminal> AddRule(IRuleDefinition rule)
    {
        var ruleDeclarations = rule.LeftHandSide.Exports.ToList();
        var terminals = new List<ITerminal>();
        rule.LeftHandSide.Match(
            and =>
            {
                var context = new ReteBuilderContext(rule, _dummyNode);
                Visit(context, and);
                var terminal = BuildTerminal(context, ruleDeclarations);
                terminals.Add(terminal);
            },
            or =>
            {
                foreach (var childElement in or.ChildElements)
                {
                    var context = new ReteBuilderContext(rule, _dummyNode);
                    Visit(context, childElement);
                    var terminal = BuildTerminal(context, ruleDeclarations);
                    terminals.Add(terminal);
                }
            });
        return terminals;
    }

    private Terminal BuildTerminal(ReteBuilderContext context, IEnumerable<Declaration> ruleDeclarations)
    {
        if (context.AlphaSource != null)
        {
            BuildJoinNode(context);
        }
        var factMap = IndexMap.CreateMap(ruleDeclarations, context.Declarations);
        var terminalNode = new Terminal(context.BetaSource, factMap);
        return terminalNode;
    }

    protected override void VisitAnd(ReteBuilderContext context, AndElement element)
    {
        foreach (var childElement in element.ChildElements)
        {
            if (context.AlphaSource != null)
            {
                BuildJoinNode(context);
            }
            Visit(context, childElement);
        }
    }

    protected override void VisitOr(ReteBuilderContext context, OrElement element)
    {
        throw new InvalidOperationException("Group Or element must be normalized");
    }

    protected override void VisitForAll(ReteBuilderContext context, ForAllElement element)
    {
        throw new InvalidOperationException("ForAll element must be normalized");
    }

    protected override void VisitNot(ReteBuilderContext context, NotElement element)
    {
        VisitSource(context, element.Source);
        BuildNotNode(context);
    }

    protected override void VisitExists(ReteBuilderContext context, ExistsElement element)
    {
        VisitSource(context, element.Source);
        BuildExistsNode(context);
    }

    protected override void VisitAggregate(ReteBuilderContext context, AggregateElement element)
    {
        VisitSource(context, element.Source);
        BuildAggregateNode(context, element);
    }

    protected override void VisitPattern(ReteBuilderContext context, PatternElement element)
    {
        var conditions = element.Expressions.Find(PatternElement.ConditionName)
            .Cast<ExpressionElement>().ToList();
        if (element.Source == null)
        {
            context.CurrentAlphaNode = _root;
            context.RegisterDeclaration(element.Declaration);

            BuildTypeNode(context, element.ValueType);

            var alphaConditions = new List<ExpressionElement>();
            var betaConditions = new List<ExpressionElement>();
            foreach (var condition in conditions)
            {
                if (condition.Imports.Count() == 1 &&
                    Equals(condition.Imports.Single(), element.Declaration))
                    alphaConditions.Add(condition);
                else
                    betaConditions.Add(condition);
            }

            foreach (var alphaCondition in alphaConditions)
            {
                BuildSelectionNode(context, alphaCondition);
            }
            BuildAlphaMemoryNode(context);

            if (betaConditions.Count > 0)
            {
                BuildJoinNode(context, betaConditions);
            }
        }
        else
        {
            var isJoined = element.Imports.Any();
            if (isJoined)
            {
                if (conditions.Any())
                {
                    BuildSubnet(context, element);

                    context.RegisterDeclaration(element.Declaration);
                    BuildJoinNode(context, conditions);
                }
                else
                {
                    Visit(context, element.Source);
                    context.RegisterDeclaration(element.Declaration);
                }
            }
            else
            {
                var joinContext = new ReteBuilderContext(context.Rule, _dummyNode);
                BuildSubnet(joinContext, element);

                joinContext.RegisterDeclaration(element.Declaration);
                if (conditions.Any())
                {
                    BuildJoinNode(joinContext, conditions);
                    BuildAdapter(joinContext);
                }
                context.AlphaSource = joinContext.AlphaSource;

                context.RegisterDeclaration(element.Declaration);
            }
        }
    }

    protected override void VisitBinding(ReteBuilderContext context, BindingElement element)
    {
        BuildBindingNode(context, element);
    }

    private void VisitSource(ReteBuilderContext context, RuleElement source)
    {
        var isJoined = source.Imports.Any();
        var subnetContext = isJoined ? new ReteBuilderContext(context) : new ReteBuilderContext(context.Rule, _dummyNode);

        Visit(subnetContext, source);

        if (subnetContext.AlphaSource == null)
        {
            BuildAdapter(subnetContext);
            context.HasSubnet = isJoined;
        }
        context.AlphaSource = subnetContext.AlphaSource;
    }

    private void BuildSubnet(ReteBuilderContext context, PatternElement element)
    {
        var source = element.Source;
        if (source is null)
        {
            throw new ArgumentException($"Source is not specified for element {element.GetType()}", nameof(element));
        }

        var isJoined = source.Imports.Any();
        var subnetContext = isJoined ? new ReteBuilderContext(context) : new ReteBuilderContext(context.Rule, _dummyNode);

        Visit(subnetContext, source);

        if (subnetContext.AlphaSource == null)
        {
            BuildAdapter(subnetContext);
            context.HasSubnet = isJoined;
        }
        context.AlphaSource = subnetContext.AlphaSource;
    }

    private void BuildAdapter(ReteBuilderContext context)
    {
        var adapter = context.BetaSource
            .Sinks.OfType<ObjectInputAdapter>()
            .SingleOrDefault()
            ?? new ObjectInputAdapter(GetNodeId(), context.BetaSource);
        adapter.NodeInfo.Add(context.Rule);
        context.AlphaSource = adapter;
    }

    private void BuildJoinNode(ReteBuilderContext context, List<ExpressionElement>? conditions = null)
    {
        var alphaSource = context.AlphaSource;
        if (alphaSource is null)
        {
            throw new ArgumentException($"{nameof(context.AlphaSource)} is null", nameof(context));
        }
        var expressionElements = conditions ?? new List<ExpressionElement>();
        var node = context.BetaSource
                       .Sinks.OfType<JoinNode>()
                       .FirstOrDefault(x =>
                           x.RightSource == alphaSource &&
                           x.LeftSource == context.BetaSource &&
                           _expressionComparer.AreEqual(
                               x.Declarations, x.ExpressionElements,
                               context.Declarations, expressionElements))
                   ?? new JoinNode(GetNodeId(), context.BetaSource, alphaSource, context.Declarations.ToList(), expressionElements, expressionElements.Select(expressionElement => _ruleExpressionCompiler.CompileLhsExpression<bool>(expressionElement, context.Declarations))
                       .ToList(), context.HasSubnet);
        node.NodeInfo.Add(context.Rule);
        BuildBetaMemoryNode(context, node);
        context.ResetAlphaSource();
    }

    private void BuildNotNode(ReteBuilderContext context)
    {
        var alphaSource = context.AlphaSource;
        if (alphaSource is null)
        {
            throw new ArgumentException($"{nameof(context.AlphaSource)} is null", nameof(context));
        }

        var node = alphaSource
                       .Sinks.OfType<NotNode>()
                       .FirstOrDefault(x =>
                           x.RightSource == alphaSource &&
                           x.LeftSource == context.BetaSource)
                   ?? new NotNode(GetNodeId(), context.BetaSource, alphaSource);
        node.NodeInfo.Add(context.Rule);
        BuildBetaMemoryNode(context, node);
        context.ResetAlphaSource();
    }

    private void BuildExistsNode(ReteBuilderContext context)
    {
        var alphaSource = context.AlphaSource;
        if (alphaSource is null)
        {
            throw new ArgumentException($"{nameof(context.AlphaSource)} is null", nameof(context));
        }

        var node = alphaSource
            .Sinks.OfType<ExistsNode>()
            .FirstOrDefault(x =>
                x.RightSource == alphaSource &&
                x.LeftSource == context.BetaSource) ?? new ExistsNode(GetNodeId(), context.BetaSource, alphaSource);
        node.NodeInfo.Add(context.Rule);
        BuildBetaMemoryNode(context, node);
        context.ResetAlphaSource();
    }

    private void BuildAggregateNode(ReteBuilderContext context, AggregateElement element)
    {
        var alphaSource = context.AlphaSource;
        if (alphaSource is null)
        {
            throw new ArgumentException($"{nameof(context.AlphaSource)} is null", nameof(context));
        }
        var node = alphaSource
            .Sinks.OfType<AggregateNode>()
            .FirstOrDefault(x =>
                x.RightSource == alphaSource &&
                x.LeftSource == context.BetaSource &&
                x.Name == element.Name &&
                _expressionComparer.AreEqual(
                    x.Declarations, x.Expressions,
                    context.Declarations, element.Expressions))
            ?? new AggregateNode(GetNodeId(), element.ResultType, context.BetaSource, alphaSource, element.Name, context.Declarations.ToList(), element.Expressions, BuildAggregatorFactory(context, element), context.HasSubnet);
        node.NodeInfo.Add(context.Rule);
        BuildBetaMemoryNode(context, node);
        context.ResetAlphaSource();
    }

    private void BuildBindingNode(ReteBuilderContext context, BindingElement element)
    {
        var node = context.BetaSource
            .Sinks.OfType<BindingNode>()
            .FirstOrDefault(x =>
                _expressionComparer.AreEqual(x.ExpressionElement, element))
            ?? new BindingNode(GetNodeId(), element, _ruleExpressionCompiler.CompileLhsTupleExpression<object>(element, context.Declarations), element.ResultType, context.BetaSource);
        node.NodeInfo.Add(context.Rule);
        BuildBetaMemoryNode(context, node);
        context.ResetAlphaSource();
    }

    private void BuildBetaMemoryNode(ReteBuilderContext context, BetaNode betaNode)
    {
        betaNode.MemoryNode ??= new BetaMemoryNode(GetNodeId());
        betaNode.MemoryNode.NodeInfo.Add(context.Rule);
        context.BetaSource = betaNode.MemoryNode;
    }

    private void BuildTypeNode(ReteBuilderContext context, Type declarationType)
    {
        var alphaNode = context.CurrentAlphaNode;
        if (alphaNode is null)
        {
            throw new ArgumentException($"{nameof(context.CurrentAlphaNode)} is null", nameof(context));
        }
        var node = alphaNode
            .ChildNodes.OfType<TypeNode>()
            .FirstOrDefault(tn => tn.FilterType == declarationType);

        if (node == null)
        {
            node = new TypeNode(GetNodeId(), declarationType);
            alphaNode.ChildNodes.Add(node);
        }
        node.NodeInfo.Add(context.Rule);
        context.CurrentAlphaNode = node;
    }

    private void BuildSelectionNode(ReteBuilderContext context, ExpressionElement element)
    {
        var alphaNode = context.CurrentAlphaNode;
        if (alphaNode is null)
        {
            throw new ArgumentException($"{nameof(context.CurrentAlphaNode)} is null", nameof(context));
        }
        var node = alphaNode
            .ChildNodes.OfType<SelectionNode>()
            .FirstOrDefault(sn => _expressionComparer.AreEqual(sn.ExpressionElement, element));

        if (node is null)
        {
            var compiledExpression = _ruleExpressionCompiler.CompileLhsFactExpression<bool>(element);
            node = new SelectionNode(GetNodeId(), context.Declarations.Last().Type, element, compiledExpression);
            alphaNode.ChildNodes.Add(node);
        }
        node.NodeInfo.Add(context.Rule);
        context.CurrentAlphaNode = node;
    }

    private void BuildAlphaMemoryNode(ReteBuilderContext context)
    {
        var alphaNode = context.CurrentAlphaNode;
        if (alphaNode is null)
        {
            throw new ArgumentException($"{nameof(context.CurrentAlphaNode)} is null", nameof(context));
        }

        var memoryNode = alphaNode.MemoryNode;
        if (memoryNode is null)
        {
            memoryNode = new AlphaMemoryNode(GetNodeId(), context.Declarations.Last().Type);
            alphaNode.MemoryNode = memoryNode;
        }
        memoryNode.NodeInfo.Add(context.Rule);
        context.AlphaSource = memoryNode;
    }

    private IAggregatorFactory BuildAggregatorFactory(ReteBuilderContext context, AggregateElement element)
    {
        var factory = element.Name switch
        {
            AggregateElement.CollectName => new CollectionAggregatorFactory(),
            AggregateElement.GroupByName => new GroupByAggregatorFactory(),
            AggregateElement.ProjectName => new ProjectionAggregatorFactory(),
            AggregateElement.FlattenName => new FlatteningAggregatorFactory(),
            _ => GetCustomFactory(element)
        };
        var compiledExpressions = CompileExpressions(context, element);
        factory.Compile(element, compiledExpressions);
        return factory;
    }

    private IAggregatorFactory GetCustomFactory(AggregateElement element)
    {
        var factoryType = element.CustomFactoryType
            ?? _aggregatorRegistry[element.Name]
            ?? throw new ArgumentException($"Custom aggregator does not have a factory registered. Name={element.Name}");

        var factory = (IAggregatorFactory)Activator.CreateInstance(factoryType);
        return factory;
    }

    private IEnumerable<IAggregateExpression> CompileExpressions(ReteBuilderContext context, AggregateElement element)
    {
        var declarations = context.Declarations.Concat(element.Source.Declaration).ToList();
        var result = new List<IAggregateExpression>(element.Expressions.Count);
        foreach (var expression in element.Expressions)
        {
            var aggregateExpression = _ruleExpressionCompiler.CompileAggregateExpression(expression, declarations);
            result.Add(aggregateExpression);
        }
        return result;
    }

    public INetwork Build()
    {
        INetwork network = new Network(_root, _dummyNode);
        return network;
    }
}