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
        private readonly List<IActivatable> _activatableNodes = new List<IActivatable>(); 

        public ITerminalNode AddRule(ReteBuilderContext context, IRuleDefinition rule)
        {
            Visit(context, rule.LeftHandSide);
            var terminalNode = new TerminalNode(context.BetaSource);
            return terminalNode;
        }

        protected override void VisitAnd(ReteBuilderContext context, AndElement element)
        {
            context.BetaSource = BuildDummyNode();
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
            BuildSubNode(context, element.ChildElements.Single());
            var betaNode = new NotNode(context.BetaSource, context.AlphaSource);
            context.BetaSource = BuildBetaMemoryNode(context, betaNode);
            context.AlphaSource = null;
        }

        protected override void VisitExists(ReteBuilderContext context, ExistsElement element)
        {
            BuildSubNode(context, element.ChildElements.Single());
            var betaNode = new ExistsNode(context.BetaSource, context.AlphaSource);
            context.BetaSource = BuildBetaMemoryNode(context, betaNode);
            context.AlphaSource = null;
        }

        protected override void VisitPattern(ReteBuilderContext context, PatternElement element)
        {
            context.RegisterDeclaration(element.Declaration);
            if (element.Source == null)
            {
                AlphaNode currentNode = BuildTypeNode(element.ValueType, _root);

                var betaConditions = new List<ConditionElement>();
                foreach (var conditionElement in element.Conditions)
                {
                    if (conditionElement.Declarations.Count() > 1)
                    {
                        betaConditions.Add(conditionElement);
                        continue;
                    }

                    var alphaCondition = new AlphaCondition(conditionElement.Expression);
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
            }
        }

        protected override void VisitAggregate(ReteBuilderContext context, AggregateElement element)
        {
            BuildSubNode(context, element.Source);
            var betaNode = new AggregateNode(context.BetaSource, context.AlphaSource, element.AggregateType);
            context.BetaSource = BuildBetaMemoryNode(context, betaNode);
            context.AlphaSource = null;
        }

        private void BuildSubNode(ReteBuilderContext context, RuleElement element)
        {
            var subnetContext = new ReteBuilderContext(context);
            Visit(subnetContext, element);

            if (subnetContext.AlphaSource == null)
            {
                var adapter = new ObjectInputAdapter(subnetContext.BetaSource);
                subnetContext.AlphaSource = adapter;
            }
            context.AlphaSource = subnetContext.AlphaSource;
        }

        private JoinNode BuildJoinNode(ReteBuilderContext context, IEnumerable<ConditionElement> conditions = null)
        {
            var betaNode = new JoinNode(context.BetaSource, context.AlphaSource);
            context.AlphaSource = null;

            if (conditions != null)
            {
                foreach (var condition in conditions)
                {
                    var mask = context.GetTupleMask(condition.Declarations);
                    var betaCondition = new BetaCondition(condition.Expression, mask.ToArray());
                    betaNode.Conditions.Add(betaCondition);
                }
            }

            return betaNode;
        }

        private IBetaMemoryNode BuildBetaMemoryNode(ReteBuilderContext context, BetaNode betaNode)
        {
            var memoryNode = new BetaMemoryNode();
            betaNode.Attach(memoryNode);
            return memoryNode;
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

        private SelectionNode BuildSelectionNode(AlphaCondition condition, AlphaNode parent)
        {
            SelectionNode selectionNode = parent.ChildNodes
                .OfType<SelectionNode>().FirstOrDefault(sn => sn.Conditions.First().Equals(condition));

            if (selectionNode == null)
            {
                selectionNode = new SelectionNode(condition);
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
        
        private DummyNode BuildDummyNode()
        {
            var dummyNode = new DummyNode();
            _activatableNodes.Add(dummyNode);
            return dummyNode;
        }

        public INetwork GetNetwork()
        {
            INetwork network = new Network(_root, _activatableNodes);
            return network;
        }
    }
}