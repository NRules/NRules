using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Rule;

namespace NRules.Core.Rete
{
    internal interface IReteBuilder
    {
        ITerminalNode AddRule(ReteBuilderContext context, IRuleDefinition rule);
        INetwork GetNetwork();
    }

    internal class ReteBuilder : IReteBuilder
    {
        private readonly RootNode _root = new RootNode();

        public ITerminalNode AddRule(ReteBuilderContext context, IRuleDefinition rule)
        {
            BuildNode(context, rule.LeftHandSide);
            var terminalNode = new TerminalNode(context.BetaSource);
            return terminalNode;
        }

        private void BuildNode(ReteBuilderContext context, RuleElement element)
        {
            element.Match(
                pattern => BuildPatternNode(context, pattern),
                group => BuildGroupNode(context, group),
                aggregate => BuildAggregateNode(context, aggregate));
        }

        private void BuildGroupNode(ReteBuilderContext context, GroupElement element)
        {
            element.Match(
                and => BuildAndGroupNode(context, element),
                or => BuildOrGroupNode(context, element),
                not => BuildNotGroupNode(context, element),
                exists => BuildExistsGroupNode(context, element));
        }

        private void BuildAndGroupNode(ReteBuilderContext context, GroupElement element)
        {
            context.BetaSource = new DummyNode();
            foreach (var childElement in element.ChildElements)
            {
                BuildNode(context, childElement);
                if (context.AlphaSource != null)
                {
                    var betaNode = BuildJoinNode(context);
                    context.BetaSource = BuildBetaMemoryNode(context, betaNode);
                }
            }
        }

        private void BuildOrGroupNode(ReteBuilderContext context, GroupElement element)
        {
            throw new NotSupportedException("Group Or conditions are not supported");
        }

        private void BuildNotGroupNode(ReteBuilderContext context, GroupElement element)
        {
            throw new NotSupportedException("Group Not conditions are not supported");
        }

        private void BuildExistsGroupNode(ReteBuilderContext context, GroupElement element)
        {
            BuildSubNode(context, element.ChildElements.Single());
            var betaNode = new ExistsNode(context.BetaSource, context.AlphaSource);
            context.BetaSource = BuildBetaMemoryNode(context, betaNode);
            context.AlphaSource = null;
        }

        private void BuildPatternNode(ReteBuilderContext context, PatternElement element)
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
                BuildNode(context, element.Source);
                //TODO: Handle a more generic case, when pattern adds its own conditions
            }
        }

        private void BuildAggregateNode(ReteBuilderContext context, AggregateElement element)
        {
            BuildSubNode(context, element.Source);
            var betaNode = new AggregateNode(context.BetaSource, context.AlphaSource, element.AggregateType);
            context.BetaSource = BuildBetaMemoryNode(context, betaNode);
            //context.RegisterDeclaration(element.Declaration);
            context.AlphaSource = null;
        }

        private void BuildSubNode(ReteBuilderContext context, RuleElement element)
        {
            var subnetContext = new ReteBuilderContext(context);
            BuildNode(subnetContext, element);

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
            betaNode.MemoryNode = memoryNode;
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

        public INetwork GetNetwork()
        {
            INetwork network = new Network(_root);
            return network;
        }
    }
}