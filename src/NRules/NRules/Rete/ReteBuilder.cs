using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Rete
{
    internal interface IReteBuilder
    {
        ITerminalNode AddRule(ReteBuilderContext context, IRuleDefinition rule);
        INetwork Build();
    }

    internal class ReteBuilder : RuleElementVisitor<ReteBuilderContext>, IReteBuilder
    {
        private readonly RootNode _root = new RootNode();
        private readonly DummyNode _dummyNode = new DummyNode();

        public ITerminalNode AddRule(ReteBuilderContext context, IRuleDefinition rule)
        {
            Visit(context, rule.LeftHandSide);
            var terminalNode = new TerminalNode(context.BetaSource);
            return terminalNode;
        }

        protected override void VisitAnd(ReteBuilderContext context, AndElement element)
        {
            if (context.BetaSource == null)
                context.BetaSource = _dummyNode;
            foreach (var childElement in element.ChildElements)
            {
                Visit(context, childElement);
                if (context.AlphaSource != null)
                {
                    BuildJoinNode(context);
                }
            }
        }

        protected override void VisitOr(ReteBuilderContext context, OrElement element)
        {
            throw new NotSupportedException("Group Or conditions are not supported");
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

        protected override void VisitForAll(ReteBuilderContext context, ForAllElement element)
        {
            var normalizedElement = ElementTransform.Normalize(element);
            Visit(context, normalizedElement);
        }

        protected override void VisitAggregate(ReteBuilderContext context, AggregateElement element)
        {
            BuildSubnet(context, element.Source);
            BuildAggregateNode(context, element.AggregateType);
        }

        protected override void VisitPattern(ReteBuilderContext context, PatternElement element)
        {
            if (element.Source == null)
            {
                context.CurrentAlphaNode = _root;
                context.RegisterDeclaration(element.Declaration);

                BuildTypeNode(context, element.ValueType);
                var alphaConditions = element.Conditions.Where(x => x.Declarations.Count() == 1).ToList();
                foreach (var alphaCondition in alphaConditions)
                {
                    BuildSelectionNode(context, alphaCondition);
                }
                BuildAlphaMemoryNode(context);

                var betaConditions = element.Conditions.Where(x => x.Declarations.Count() > 1).ToList();
                if (betaConditions.Count > 0)
                {
                    BuildJoinNode(context, betaConditions);
                }
            }
            else
            {
                Visit(context, element.Source);

                context.RegisterDeclaration(element.Declaration);
                //TODO: Handle a more generic case, when pattern adds its own conditions
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
                    var mask = context.GetTupleMask(condition.Declarations);
                    var betaCondition = new BetaCondition(condition.Expression, mask);
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
                node = new JoinNode(context.BetaSource, context.AlphaSource);
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

        private void BuildAggregateNode(ReteBuilderContext context, Type aggregateType)
        {
            var node = context.AlphaSource
                .Sinks.OfType<AggregateNode>()
                .FirstOrDefault(x =>
                    x.RightSource == context.AlphaSource &&
                    x.LeftSource == context.BetaSource &&
                    x.AggregateType == aggregateType);
            if (node == null)
            {
                node = new AggregateNode(context.BetaSource, context.AlphaSource, aggregateType);
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
                .FirstOrDefault(tn => tn.FilterType == declarationType);

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
                .FirstOrDefault(sn => sn.Conditions.Single().Equals(alphaCondition));

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