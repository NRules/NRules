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
        _root = new RootNode { Id = GetNodeId() };
        _dummyNode = new DummyNode { Id = GetNodeId() };
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
        if (context.AlphaSource is not null)
        {
            BuildJoinNode(context, context.AlphaSource);
        }
        var factMap = IndexMap.CreateMap(ruleDeclarations, context.Declarations);
        var terminalNode = new Terminal(context.BetaSource, factMap);
        return terminalNode;
    }

    protected override void VisitAnd(ReteBuilderContext context, AndElement element)
    {
        foreach (var childElement in element.ChildElements)
        {
            if (context.AlphaSource is not null)
            {
                BuildJoinNode(context, context.AlphaSource);
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
        context.AlphaSource = VisitSource(context, element.Source);
        BuildNotNode(context, context.AlphaSource);
    }

    protected override void VisitExists(ReteBuilderContext context, ExistsElement element)
    {
        context.AlphaSource = VisitSource(context, element.Source);
        BuildExistsNode(context, context.AlphaSource);
    }

    protected override void VisitAggregate(ReteBuilderContext context, AggregateElement element)
    {
        context.AlphaSource = VisitSource(context, element.Source);
        BuildAggregateNode(context, context.AlphaSource, element);
    }

    protected override void VisitPattern(ReteBuilderContext context, PatternElement element)
    {
        var conditions = element.Expressions.Find(PatternElement.ConditionName)
            .Cast<ExpressionElement>().ToList();
        if (element.Source is null)
        {
            context.CurrentAlphaNode = _root;
            context.RegisterDeclaration(element.Declaration);

            context.CurrentAlphaNode = BuildTypeNode(context, element.ValueType, context.CurrentAlphaNode);

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
                context.CurrentAlphaNode = BuildSelectionNode(context, alphaCondition, context.CurrentAlphaNode);
            }
            context.AlphaSource = BuildAlphaMemoryNode(context, context.CurrentAlphaNode);

            if (betaConditions.Count > 0)
            {
                BuildJoinNode(context, context.AlphaSource, betaConditions);
            }
        }
        else
        {
            var isJoined = element.Imports.Any();
            if (isJoined)
            {
                if (conditions.Any())
                {
                    context.AlphaSource = BuildSubnet(context, element.Source);

                    context.RegisterDeclaration(element.Declaration);
                    BuildJoinNode(context, context.AlphaSource, conditions);
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
                joinContext.AlphaSource = BuildSubnet(joinContext, element.Source);

                joinContext.RegisterDeclaration(element.Declaration);
                if (conditions.Any())
                {
                    BuildJoinNode(joinContext, joinContext.AlphaSource, conditions);
                    joinContext.AlphaSource = BuildAdapter(joinContext);
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

    private IAlphaMemoryNode VisitSource(ReteBuilderContext context, RuleElement source)
    {
        var isJoined = source.Imports.Any();
        var subnetContext = isJoined ? new ReteBuilderContext(context) : new ReteBuilderContext(context.Rule, _dummyNode);

        Visit(subnetContext, source);

        if (subnetContext.AlphaSource is null)
        {
            subnetContext.AlphaSource = BuildAdapter(subnetContext);
            context.HasSubnet = isJoined;
        }
        return subnetContext.AlphaSource;
    }

    private IAlphaMemoryNode BuildSubnet(ReteBuilderContext context, RuleElement source)
    {
        var isJoined = source.Imports.Any();
        var subnetContext = isJoined ? new ReteBuilderContext(context) : new ReteBuilderContext(context.Rule, _dummyNode);

        Visit(subnetContext, source);

        if (subnetContext.AlphaSource is null)
        {
            subnetContext.AlphaSource = BuildAdapter(subnetContext);
            context.HasSubnet = isJoined;
        }
        return subnetContext.AlphaSource;
    }

    private ObjectInputAdapter BuildAdapter(ReteBuilderContext context)
    {
        var adapter = context.BetaSource
            .Sinks.OfType<ObjectInputAdapter>()
            .SingleOrDefault()
            ?? new ObjectInputAdapter(context.BetaSource) { Id = GetNodeId() };
        adapter.NodeInfo.Add(context.Rule);
        return adapter;
    }

    private void BuildJoinNode(ReteBuilderContext context, IAlphaMemoryNode alphaSource, List<ExpressionElement>? conditions = null)
    {
        var expressionElements = conditions ?? new List<ExpressionElement>();
        var node = context.BetaSource
            .Sinks.OfType<JoinNode>()
            .FirstOrDefault(x =>
                x.RightSource == alphaSource &&
                x.LeftSource == context.BetaSource &&
                _expressionComparer.AreEqual(
                    x.Declarations, x.ExpressionElements,
                    context.Declarations, expressionElements));
        if (node is null)
        {
            var compiledExpressions = new List<ILhsExpression<bool>>(expressionElements.Count);
            foreach (var expressionElement in expressionElements)
            {
                var compiledExpression = _ruleExpressionCompiler.CompileLhsExpression<bool>(expressionElement, context.Declarations);
                compiledExpressions.Add(compiledExpression);
            }
            node = new JoinNode(context.BetaSource, alphaSource, context.Declarations.ToList(), expressionElements, compiledExpressions, context.HasSubnet)
            {
                Id = GetNodeId()
            };
        }
        node.NodeInfo.Add(context.Rule);
        BuildBetaMemoryNode(context, node);
        context.ResetAlphaSource();
    }

    private void BuildNotNode(ReteBuilderContext context, IAlphaMemoryNode alphaSource)
    {
        var node = alphaSource
            .Sinks.OfType<NotNode>()
            .FirstOrDefault(x =>
                x.RightSource == alphaSource &&
                x.LeftSource == context.BetaSource);
        node ??= new NotNode(context.BetaSource, alphaSource) { Id = GetNodeId() };
        node.NodeInfo.Add(context.Rule);
        BuildBetaMemoryNode(context, node);
        context.ResetAlphaSource();
    }

    private void BuildExistsNode(ReteBuilderContext context, IAlphaMemoryNode alphaSource)
    {
        var node = alphaSource
            .Sinks.OfType<ExistsNode>()
            .FirstOrDefault(x =>
                x.RightSource == alphaSource &&
                x.LeftSource == context.BetaSource);
        node ??= new ExistsNode(context.BetaSource, alphaSource) { Id = GetNodeId() };
        node.NodeInfo.Add(context.Rule);
        BuildBetaMemoryNode(context, node);
        context.ResetAlphaSource();
    }

    private void BuildAggregateNode(ReteBuilderContext context, IAlphaMemoryNode alphaSource, AggregateElement element)
    {
        var node = alphaSource
            .Sinks.OfType<AggregateNode>()
            .FirstOrDefault(x =>
                x.RightSource == alphaSource &&
                x.LeftSource == context.BetaSource &&
                x.Name == element.Name &&
                _expressionComparer.AreEqual(
                    x.Declarations, x.Expressions,
                    context.Declarations, element.Expressions));
        if (node is null)
        {
            var aggregatorFactory = BuildAggregatorFactory(context, element);
            node = new AggregateNode(context.BetaSource, alphaSource, element.Name,
                context.Declarations.ToList(), element.Expressions, aggregatorFactory, context.HasSubnet)
            {
                Id = GetNodeId(),
                NodeInfo = { OutputType = element.ResultType }
            };
        }
        node.NodeInfo.Add(context.Rule);
        BuildBetaMemoryNode(context, node);
        context.ResetAlphaSource();
    }

    private void BuildBindingNode(ReteBuilderContext context, BindingElement element)
    {
        var node = context.BetaSource
            .Sinks.OfType<BindingNode>()
            .FirstOrDefault(x =>
                _expressionComparer.AreEqual(x.ExpressionElement, element));
        if (node is null)
        {
            var compiledExpression = _ruleExpressionCompiler.CompileLhsTupleExpression<object?>(element, context.Declarations);
            node = new BindingNode(element, compiledExpression, element.ResultType, context.BetaSource)
            {
                Id = GetNodeId(),
                NodeInfo = { OutputType = element.ResultType }
            };
        }
        node.NodeInfo.Add(context.Rule);
        BuildBetaMemoryNode(context, node);
        context.ResetAlphaSource();
    }

    private void BuildBetaMemoryNode(ReteBuilderContext context, BetaNode betaNode)
    {
        var memoryNode = betaNode.EnsureMemoryNodeInitialized(GetNodeId);
        memoryNode.NodeInfo.Add(context.Rule);
        context.BetaSource = memoryNode;
    }

    private TypeNode BuildTypeNode(ReteBuilderContext context, Type declarationType, AlphaNode currentAlphaNode)
    {
        var node = currentAlphaNode
            .ChildNodes.OfType<TypeNode>()
            .FirstOrDefault(tn => tn.FilterType == declarationType);

        if (node is null)
        {
            node = new TypeNode(declarationType)
            {
                Id = GetNodeId(),
                NodeInfo = { OutputType = declarationType }
            };
            currentAlphaNode.ChildNodes.Add(node);
        }
        node.NodeInfo.Add(context.Rule);
        return node;
    }

    private SelectionNode BuildSelectionNode(ReteBuilderContext context, ExpressionElement element, AlphaNode currentAlphaNode)
    {
        var node = currentAlphaNode
            .ChildNodes.OfType<SelectionNode>()
            .FirstOrDefault(sn => _expressionComparer.AreEqual(sn.ExpressionElement, element));

        if (node is null)
        {
            var compiledExpression = _ruleExpressionCompiler.CompileLhsFactExpression<bool>(element);
            node = new SelectionNode(element, compiledExpression)
            {
                Id = GetNodeId(),
                NodeInfo = { OutputType = context.Declarations.Last().Type }
            };
            currentAlphaNode.ChildNodes.Add(node);
        }
        node.NodeInfo.Add(context.Rule);
        return node;
    }

    private AlphaMemoryNode BuildAlphaMemoryNode(ReteBuilderContext context, AlphaNode currentAlphaNode)
    {
        var memoryNode = currentAlphaNode.MemoryNode;
        if (memoryNode is null)
        {
            memoryNode = new AlphaMemoryNode
            {
                Id = GetNodeId(),
                NodeInfo = { OutputType = context.Declarations.Last().Type }
            };
            currentAlphaNode.MemoryNode = memoryNode;
        }
        memoryNode.NodeInfo.Add(context.Rule);
        return memoryNode;
    }

    private IAggregatorFactory BuildAggregatorFactory(ReteBuilderContext context, AggregateElement element)
    {
        var factory = element.Name switch
        {
            AggregateElement.CollectName => new CollectionAggregatorFactory(),
            AggregateElement.GroupByName => new GroupByAggregatorFactory(),
            AggregateElement.ProjectName => new ProjectionAggregatorFactory(),
            AggregateElement.FlattenName => new FlatteningAggregatorFactory(),
            _ => GetCustomFactory(element),
        };
        var compiledExpressions = CompileExpressions(context, element);
        factory.Compile(element, compiledExpressions);
        return factory;
    }

    private IAggregatorFactory GetCustomFactory(AggregateElement element)
    {
        var factoryType = element.CustomFactoryType ?? _aggregatorRegistry[element.Name];
        if (factoryType is null)
        {
            throw new ArgumentException($"Custom aggregator does not have a factory registered. Name={element.Name}");
        }

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