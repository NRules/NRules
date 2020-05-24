using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Aggregators;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Rete
{
    internal interface IReteBuilder
    {
        IEnumerable<ITerminal> AddRule(IRuleDefinition rule);
        INetwork Build();
    }

    internal class ReteBuilder : RuleElementVisitor<ReteBuilderContext>, IReteBuilder
    {
        private readonly RootNode _root = new RootNode();
        private readonly DummyNode _dummyNode = new DummyNode();
        private readonly AggregatorRegistry _aggregatorRegistry;

        public ReteBuilder(AggregatorRegistry aggregatorRegistry)
        {
            _aggregatorRegistry = aggregatorRegistry;
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
                    var terminal = BuildTerminal(context, and, ruleDeclarations);
                    terminals.Add(terminal);
                },
                or =>
                {
                    foreach (var childElement in or.ChildElements)
                    {
                        var context = new ReteBuilderContext(rule, _dummyNode);
                        Visit(context, childElement);
                        var terminal = BuildTerminal(context, childElement, ruleDeclarations);
                        terminals.Add(terminal);
                    }
                });
            return terminals;
        }

        private Terminal BuildTerminal(ReteBuilderContext context, RuleElement element, IEnumerable<Declaration> ruleDeclarations)
        {
            if (context.AlphaSource != null)
            {
                BuildJoinNode(context, element);
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
                    BuildJoinNode(context, childElement);
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
            BuildSubnet(context, element.Source);
            BuildNotNode(context, element);
        }

        protected override void VisitExists(ReteBuilderContext context, ExistsElement element)
        {
            BuildSubnet(context, element.Source);
            BuildExistsNode(context, element);
        }

        protected override void VisitAggregate(ReteBuilderContext context, AggregateElement element)
        {
            BuildSubnet(context, element.Source);
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

                BuildTypeNode(context, element, element.ValueType);
                var alphaConditions = conditions.Where(x => x.Imports.Count() == 1).ToList();
                foreach (var alphaCondition in alphaConditions)
                {
                    BuildSelectionNode(context, alphaCondition);
                }
                BuildAlphaMemoryNode(context);

                var betaConditions = conditions.Where(x => x.Imports.Count() > 1).ToList();
                if (betaConditions.Count > 0)
                {
                    BuildJoinNode(context, element, betaConditions);
                }
            }
            else
            {
                if (conditions.Any())
                {
                    BuildSubnet(context, element.Source);
                    context.RegisterDeclaration(element.Declaration);

                    BuildJoinNode(context, element, conditions);
                }
                else
                {
                    Visit(context, element.Source);
                    context.RegisterDeclaration(element.Declaration);
                }
            }
        }

        protected override void VisitBinding(ReteBuilderContext context, BindingElement element)
        {
            BuildBindingNode(context, element);
        }

        private void BuildSubnet(ReteBuilderContext context, RuleElement element)
        {
            var subnetContext = new ReteBuilderContext(context);
            Visit(subnetContext, element);

            if (subnetContext.AlphaSource == null)
            {
                var adapter = subnetContext.BetaSource
                    .Sinks.OfType<ObjectInputAdapter>()
                    .SingleOrDefault();
                if (adapter == null)
                {
                    adapter = new ObjectInputAdapter(subnetContext.BetaSource);
                }
                subnetContext.AlphaSource = adapter;
                context.HasSubnet = true;
            }
            context.AlphaSource = subnetContext.AlphaSource;
        }

        private void BuildJoinNode(ReteBuilderContext context, RuleElement element, List<ExpressionElement> conditions = null)
        {
            var expressionElements = conditions ?? new List<ExpressionElement>();
            var node = context.BetaSource
                .Sinks.OfType<JoinNode>()
                .FirstOrDefault(x =>
                    x.RightSource == context.AlphaSource &&
                    x.LeftSource == context.BetaSource &&
                    ExpressionElementComparer.AreEqual(x.ExpressionElements, expressionElements));
            if (node == null)
            {
                var compiledExpressions = new List<ILhsExpression<bool>>(expressionElements.Count);
                foreach (var expressionElement in expressionElements)
                {
                    var compiledExpression = ExpressionCompiler.CompileLhsExpression<bool>(expressionElement, context.Declarations);
                    compiledExpressions.Add(compiledExpression);
                }
                node = new JoinNode(context.BetaSource, context.AlphaSource, expressionElements, compiledExpressions, context.HasSubnet);
            }
            node.NodeInfo.Add(context.Rule, element);
            BuildBetaMemoryNode(context, node);
            context.ResetAlphaSource();
        }

        private void BuildNotNode(ReteBuilderContext context, RuleElement element)
        {
            var node = context.AlphaSource
                .Sinks.OfType<NotNode>()
                .FirstOrDefault(x =>
                    x.RightSource == context.AlphaSource &&
                    x.LeftSource == context.BetaSource);
            if (node == null)
            {
                node = new NotNode(context.BetaSource, context.AlphaSource);
            }
            node.NodeInfo.Add(context.Rule, element);
            BuildBetaMemoryNode(context, node);
            context.ResetAlphaSource();
        }

        private void BuildExistsNode(ReteBuilderContext context, RuleElement element)
        {
            var node = context.AlphaSource
                .Sinks.OfType<ExistsNode>()
                .FirstOrDefault(x =>
                    x.RightSource == context.AlphaSource &&
                    x.LeftSource == context.BetaSource);
            if (node == null)
            {
                node = new ExistsNode(context.BetaSource, context.AlphaSource);
            }
            node.NodeInfo.Add(context.Rule, element);
            BuildBetaMemoryNode(context, node);
            context.ResetAlphaSource();
        }

        private void BuildAggregateNode(ReteBuilderContext context, AggregateElement element)
        {
            var node = context.AlphaSource
                .Sinks.OfType<AggregateNode>()
                .FirstOrDefault(x =>
                    x.RightSource == context.AlphaSource &&
                    x.LeftSource == context.BetaSource &&
                    x.Name == element.Name &&
                    ExpressionElementComparer.AreEqual(x.Expressions, element.Expressions));
            if (node == null)
            {
                var aggregatorFactory = BuildAggregatorFactory(context, element);
                node = new AggregateNode(context.BetaSource, context.AlphaSource, element.Name,
                    element.Expressions, aggregatorFactory, context.HasSubnet);
            }
            node.NodeInfo.Add(context.Rule, element);
            BuildBetaMemoryNode(context, node);
            context.ResetAlphaSource();
        }

        private void BuildBindingNode(ReteBuilderContext context, BindingElement element)
        {
            var node = context.BetaSource
                .Sinks.OfType<BindingNode>()
                .FirstOrDefault(x =>
                    ExpressionElementComparer.AreEqual(x.ExpressionElement, element));
            if (node == null)
            {
                var compiledExpression = ExpressionCompiler.CompileLhsTupleExpression<object>(element, context.Declarations);
                node = new BindingNode(element, compiledExpression, element.ResultType, context.BetaSource);
            }
            node.NodeInfo.Add(context.Rule, element);
            BuildBetaMemoryNode(context, node);
            context.ResetAlphaSource();
        }

        private void BuildBetaMemoryNode(ReteBuilderContext context, BetaNode betaNode)
        {
            if (betaNode.MemoryNode == null)
            {
                betaNode.MemoryNode = new BetaMemoryNode();
            }
            context.BetaSource = betaNode.MemoryNode;
        }

        private void BuildTypeNode(ReteBuilderContext context, RuleElement element, Type declarationType)
        {
            TypeNode node = context.CurrentAlphaNode
                .ChildNodes.OfType<TypeNode>()
                .FirstOrDefault(tn => tn.FilterType.AsType() == declarationType);

            if (node == null)
            {
                node = new TypeNode(declarationType);
                context.CurrentAlphaNode.ChildNodes.Add(node);
            }
            node.NodeInfo.Add(context.Rule, element);
            context.CurrentAlphaNode = node;
        }

        private void BuildSelectionNode(ReteBuilderContext context, ExpressionElement element)
        {
            SelectionNode node = context.CurrentAlphaNode
                .ChildNodes.OfType<SelectionNode>()
                .FirstOrDefault(sn => ExpressionElementComparer.AreEqual(sn.ExpressionElement, element));

            if (node == null)
            {
                var compiledExpression = ExpressionCompiler.CompileLhsFactExpression<bool>(element);
                node = new SelectionNode(element, compiledExpression);
                context.CurrentAlphaNode.ChildNodes.Add(node);
            }
            node.NodeInfo.Add(context.Rule, element);
            context.CurrentAlphaNode = node;
        }

        private void BuildAlphaMemoryNode(ReteBuilderContext context)
        {
            AlphaMemoryNode memoryNode = context.CurrentAlphaNode.MemoryNode;

            if (memoryNode == null)
            {
                memoryNode = new AlphaMemoryNode();
                context.CurrentAlphaNode.MemoryNode = memoryNode;
            }

            context.AlphaSource = memoryNode;
        }

        private IAggregatorFactory BuildAggregatorFactory(ReteBuilderContext context, AggregateElement element)
        {
            IAggregatorFactory factory;
            switch (element.Name)
            {
                case AggregateElement.CollectName:
                    factory = new CollectionAggregatorFactory();
                    break;
                case AggregateElement.GroupByName:
                    factory = new GroupByAggregatorFactory();
                    break;
                case AggregateElement.ProjectName:
                    factory = new ProjectionAggregatorFactory();
                    break;
                case AggregateElement.FlattenName:
                    factory = new FlatteningAggregatorFactory();
                    break;
                default:
                    factory = GetCustomFactory(element);
                    break;
            }
            var compiledExpressions = CompileExpressions(context, element);
            factory.Compile(element, compiledExpressions);
            return factory;
        }

        private IAggregatorFactory GetCustomFactory(AggregateElement element)
        {
            Type factoryType = element.CustomFactoryType;
            if (factoryType == null)
            {
                factoryType = _aggregatorRegistry[element.Name];
            }

            if (factoryType == null)
            {
                throw new ArgumentException($"Custom aggregator does not have a factory registered. Name={element.Name}");
            }

            var factory = (IAggregatorFactory)Activator.CreateInstance(factoryType);
            return factory;
        }

        private static IEnumerable<IAggregateExpression> CompileExpressions(ReteBuilderContext context, AggregateElement element)
        {
            var declarations = context.Declarations.Concat(element.Source.Declaration).ToList();
            var result = new List<IAggregateExpression>(element.Expressions.Count);
            foreach (var expression in element.Expressions)
            {
                var aggregateExpression = ExpressionCompiler.CompileAggregateExpression(expression, declarations);
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
}