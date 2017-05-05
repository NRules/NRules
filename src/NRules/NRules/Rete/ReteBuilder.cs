using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal interface IReteBuilder
    {
        IEnumerable<ITerminalNode> AddRule(IRuleDefinition rule);
        INetwork Build();
    }

    internal class ReteBuilder : RuleElementVisitor<ReteBuilderContext>, IReteBuilder
    {
        private readonly RootNode _root = new RootNode();
        private readonly DummyNode _dummyNode = new DummyNode();

        public IEnumerable<ITerminalNode> AddRule(IRuleDefinition rule)
        {
            var ruleDeclarations = rule.LeftHandSide.Declarations.ToList();
            var terminals = new List<ITerminalNode>();
            rule.LeftHandSide.Match(
                and =>
                {
                    var context = new ReteBuilderContext(_dummyNode);
                    Visit(context, and);
                    var terminalNode = BuildTerminalNode(context, ruleDeclarations);
                    terminals.Add(terminalNode);
                },
                or =>
                {
                    foreach (var childElement in or.ChildElements)
                    {
                        var context = new ReteBuilderContext(_dummyNode);
                        Visit(context, childElement);
                        var terminalNode = BuildTerminalNode(context, ruleDeclarations);
                        terminals.Add(terminalNode);
                    }
                });
            return terminals;
        }

        private TerminalNode BuildTerminalNode(ReteBuilderContext context, IEnumerable<Declaration> ruleDeclarations)
        {
            if (context.AlphaSource != null)
            {
                BuildJoinNode(context);
            }
            var factIndexMap = IndexMap.CreateMap(ruleDeclarations, context.Declarations);
            var terminalNode = new TerminalNode(context.BetaSource, factIndexMap);
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
            BuildSubnet(context, element.Source);
            BuildNotNode(context);
        }

        protected override void VisitExists(ReteBuilderContext context, ExistsElement element)
        {
            BuildSubnet(context, element.Source);
            BuildExistsNode(context);
        }

        protected override void VisitAggregate(ReteBuilderContext context, AggregateElement element)
        {
            BuildSubnet(context, element.Source);
            BuildAggregateNode(context, element);
        }

        protected override void VisitPattern(ReteBuilderContext context, PatternElement element)
        {
            if (element.Source == null)
            {
                context.CurrentAlphaNode = _root;
                context.RegisterDeclaration(element.Declaration);

                BuildTypeNode(context, element.ValueType);
                var alphaConditions = element.Conditions.Where(x => x.References.Count() == 1).ToList();
                foreach (var alphaCondition in alphaConditions)
                {
                    BuildSelectionNode(context, alphaCondition);
                }
                BuildAlphaMemoryNode(context);

                var betaConditions = element.Conditions.Where(x => x.References.Count() > 1).ToList();
                if (betaConditions.Count > 0)
                {
                    BuildJoinNode(context, betaConditions);
                }
            }
            else
            {
                if (element.Conditions.Any())
                {
                    BuildSubnet(context, element.Source);
                    context.RegisterDeclaration(element.Declaration);

                    BuildJoinNode(context, element.Conditions);
                }
                else
                {
                    Visit(context, element.Source);
                    context.RegisterDeclaration(element.Declaration);
                }
            }
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

        private void BuildJoinNode(ReteBuilderContext context, IEnumerable<ConditionElement> conditions = null)
        {
            var betaConditions = new List<BetaCondition>();
            if (conditions != null)
            {
                foreach (var condition in conditions)
                {
                    var factIndexMap = IndexMap.CreateMap(condition.References, context.Declarations);
                    var betaCondition = new BetaCondition(condition.Expression, factIndexMap);
                    betaConditions.Add(betaCondition);
                }
            }

            var node = context.BetaSource
                .Sinks.OfType<JoinNode>()
                .FirstOrDefault(x => 
                    x.RightSource == context.AlphaSource &&
                    x.LeftSource == context.BetaSource &&
                    ConditionComparer.AreEqual(x.Conditions, betaConditions));
            if (node == null)
            {
                node = new JoinNode(context.BetaSource, context.AlphaSource, context.HasSubnet);
                if (context.HasSubnet) node.Conditions.Insert(0, new SubnetCondition());
                foreach (var betaCondition in betaConditions)
                {
                    node.Conditions.Add(betaCondition);
                }
            }
            BuildBetaMemoryNode(context, node);
            context.ResetAlphaSource();
        }

        private void BuildNotNode(ReteBuilderContext context)
        {
            var node = context.AlphaSource
                .Sinks.OfType<NotNode>()
                .FirstOrDefault(x =>
                    x.RightSource == context.AlphaSource &&
                    x.LeftSource == context.BetaSource);
            if (node == null)
            {
                node = new NotNode(context.BetaSource, context.AlphaSource);
                if (context.HasSubnet) node.Conditions.Insert(0, new SubnetCondition());
            }
            BuildBetaMemoryNode(context, node);
            context.ResetAlphaSource();
        }

        private void BuildExistsNode(ReteBuilderContext context)
        {
            var node = context.AlphaSource
                .Sinks.OfType<ExistsNode>()
                .FirstOrDefault(x =>
                    x.RightSource == context.AlphaSource &&
                    x.LeftSource == context.BetaSource);
            if (node == null)
            {
                node = new ExistsNode(context.BetaSource, context.AlphaSource);
                if (context.HasSubnet) node.Conditions.Insert(0, new SubnetCondition());
            }
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
                    ExpressionMapComparer.AreEqual(x.ExpressionMap, element.ExpressionMap));
            if (node == null)
            {
                node = new AggregateNode(context.BetaSource, context.AlphaSource, element.Name, 
                    element.ExpressionMap, element.AggregatorFactory, context.HasSubnet);
                if (context.HasSubnet) node.Conditions.Insert(0, new SubnetCondition());
            }
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

        private void BuildTypeNode(ReteBuilderContext context, Type declarationType)
        {
            TypeNode typeNode = context.CurrentAlphaNode
                .ChildNodes.OfType<TypeNode>()
                .FirstOrDefault(tn => tn.FilterType.AsType() == declarationType);

            if (typeNode == null)
            {
                typeNode = new TypeNode(declarationType);
                context.CurrentAlphaNode.ChildNodes.Add(typeNode);
            }
            context.CurrentAlphaNode = typeNode;
        }

        private void BuildSelectionNode(ReteBuilderContext context, ConditionElement condition)
        {
            var alphaCondition = new AlphaCondition(condition.Expression);
            SelectionNode selectionNode = context.CurrentAlphaNode
                .ChildNodes.OfType<SelectionNode>()
                .FirstOrDefault(sn => sn.Condition.Equals(alphaCondition));

            if (selectionNode == null)
            {
                selectionNode = new SelectionNode(alphaCondition);
                context.CurrentAlphaNode.ChildNodes.Add(selectionNode);
            }
            context.CurrentAlphaNode = selectionNode;
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
        
        public INetwork Build()
        {
            INetwork network = new Network(_root, _dummyNode);
            return network;
        }
    }
}