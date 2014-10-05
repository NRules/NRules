using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal interface IReteBuilder
    {
        ITerminalNode AddRule(ReteBuilderContext context, IRuleDefinition rule);
        INetwork GetNetwork();
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
            context.BetaSource = _dummyNode;
            foreach (var childElement in element.ChildElements)
            {
                Visit(context, childElement);
                if (context.AlphaSource != null)
                {
                    var betaNode = BuildJoinNode(context);
                    context.BetaSource = BuildBetaMemoryNode(context, betaNode);
                }
            }
        }

        protected override void VisitOr(ReteBuilderContext context, OrElement element)
        {
            throw new NotSupportedException("Group Or conditions are not supported");
        }

        protected override void VisitNot(ReteBuilderContext context, NotElement element)
        {
            BuildSubnet(context, element.ChildElements.Single());
            var betaNode = new NotNode(context.BetaSource, context.AlphaSource);
            if (context.HasSubnet) betaNode.Conditions.Insert(0, new SubnetCondition());
            context.BetaSource = BuildBetaMemoryNode(context, betaNode);
            context.ResetAlphaSource();
        }

        protected override void VisitExists(ReteBuilderContext context, ExistsElement element)
        {
            BuildSubnet(context, element.ChildElements.Single());
            var betaNode = new ExistsNode(context.BetaSource, context.AlphaSource);
            if (context.HasSubnet) betaNode.Conditions.Insert(0, new SubnetCondition());
            context.BetaSource = BuildBetaMemoryNode(context, betaNode);
            context.ResetAlphaSource();
        }

        protected override void VisitAggregate(ReteBuilderContext context, AggregateElement element)
        {
            BuildSubnet(context, element.Source);
            var betaNode = new AggregateNode(context.BetaSource, context.AlphaSource, element.AggregateType);
            if (context.HasSubnet) betaNode.Conditions.Insert(0, new SubnetCondition());
            context.BetaSource = BuildBetaMemoryNode(context, betaNode);
            context.ResetAlphaSource();
        }

        protected override void VisitPattern(ReteBuilderContext context, PatternElement element)
        {
            if (element.Source == null)
            {
                context.RegisterDeclaration(element.Declaration);

                AlphaNode currentNode = BuildTypeNode(element.ValueType, _root);

                var alphaConditions = element.Conditions.Where(x => x.Declarations.Count() == 1).ToList();
                var betaConditions = element.Conditions.Where(x => x.Declarations.Count() > 1).ToList();

                foreach (var alphaCondition in alphaConditions)
                {
                    SelectionNode selectionNode = BuildSelectionNode(alphaCondition, currentNode);
                    currentNode = selectionNode;
                }

                context.AlphaSource = BuildAlphaMemoryNode(currentNode);

                if (betaConditions.Count > 0)
                {
                    var joinNode = BuildJoinNode(context, betaConditions);
                    context.BetaSource = BuildBetaMemoryNode(context, joinNode);
                }
            }
            else
            {
                Visit(context, element.Source);
                //TODO: Handle a more generic case, when pattern adds its own conditions

                context.RegisterDeclaration(element.Declaration);
            }
        }

        private void BuildSubnet(ReteBuilderContext context, RuleElement element)
        {
            var subnetContext = new ReteBuilderContext(context);
            Visit(subnetContext, element);

            if (subnetContext.AlphaSource == null)
            {
                var adapter = new ObjectInputAdapter(subnetContext.BetaSource);
                subnetContext.AlphaSource = adapter;
                context.HasSubnet = true;
            }
            context.AlphaSource = subnetContext.AlphaSource;
        }

        private JoinNode BuildJoinNode(ReteBuilderContext context, IEnumerable<ConditionElement> conditions = null)
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

            var betaNode = context.BetaSource
                .Sinks.OfType<JoinNode>()
                .FirstOrDefault(jn => 
                    jn.RightSource == context.AlphaSource &&
                    jn.LeftSource == context.BetaSource &&
                    ConditionComparer.AreEqual(jn.Conditions, betaConditions));

            if (betaNode == null)
            {
                betaNode = new JoinNode(context.BetaSource, context.AlphaSource);
                foreach (var betaCondition in betaConditions)
                {
                    betaNode.Conditions.Add(betaCondition);
                }
            }
            context.AlphaSource = null;

            return betaNode;
        }

        private IBetaMemoryNode BuildBetaMemoryNode(ReteBuilderContext context, BetaNode betaNode)
        {
            if (betaNode.MemoryNode == null)
            {
                betaNode.MemoryNode = new BetaMemoryNode();
            }
            return betaNode.MemoryNode;
        }

        private TypeNode BuildTypeNode(Type declarationType, AlphaNode parent)
        {
            TypeNode typeNode = parent.ChildNodes
                .Cast<TypeNode>().FirstOrDefault(tn => tn.FilterType == declarationType);

            if (typeNode == null)
            {
                typeNode = new TypeNode(declarationType);
                parent.ChildNodes.Add(typeNode);
            }
            return typeNode;
        }

        private SelectionNode BuildSelectionNode(ConditionElement condition, AlphaNode parent)
        {
            var alphaCondition = new AlphaCondition(condition.Expression);
            SelectionNode selectionNode = parent
                .ChildNodes.OfType<SelectionNode>()
                .FirstOrDefault(sn => sn.Conditions.First().Equals(alphaCondition));

            if (selectionNode == null)
            {
                selectionNode = new SelectionNode(alphaCondition);
                parent.ChildNodes.Add(selectionNode);
            }
            return selectionNode;
        }

        private IAlphaMemoryNode BuildAlphaMemoryNode(AlphaNode parent)
        {
            AlphaMemoryNode memoryNode = parent.MemoryNode;

            if (memoryNode == null)
            {
                memoryNode = new AlphaMemoryNode();
                parent.MemoryNode = memoryNode;
            }

            return memoryNode;
        }
        
        public INetwork GetNetwork()
        {
            INetwork network = new Network(_root, _dummyNode);
            return network;
        }
    }
}