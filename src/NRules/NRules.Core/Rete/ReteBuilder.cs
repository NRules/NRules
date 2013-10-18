using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Rule;

namespace NRules.Core.Rete
{
    internal interface IReteBuilder
    {
        void AddRule(ICompiledRule rule);
        INetwork GetNetwork();
    }

    internal class ReteBuilder : IReteBuilder
    {
        private readonly RootNode _root = new RootNode();

        public void AddRule(ICompiledRule rule)
        {
            var context = new ReteBuilderContext();
            BuildNode(context, rule.Definition.LeftHandSide);
            BuildRuleNode(context, rule);
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
                    var betaNode = new JoinNode(context.BetaSource, context.AlphaSource);
                    context.BetaSource = BuildBetaNodeAssembly(context, betaNode);
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
            BuildNode(context, element.ChildElements.Single());
            var betaNode = new ExistsNode(context.BetaSource, context.AlphaSource);
            context.BetaSource = BuildBetaNodeAssembly(context, betaNode);
            context.AlphaSource = null;
        }

        private void BuildPatternNode(ReteBuilderContext context, PatternElement element)
        {
            AlphaNode currentNode = BuildTypeNode(element.ValueType, _root);

            foreach (var conditionElement in element.Conditions)
            {
                if (conditionElement.Declarations.Count() > 1)
                {
                    var betaCondition = new BetaCondition(conditionElement.Expression);
                    context.BetaConditions.Add(betaCondition);
                    continue;
                }

                var alphaCondition = new AlphaCondition(conditionElement.Expression);
                SelectionNode selectionNode = BuildSlectionNode(alphaCondition, currentNode);
                currentNode = selectionNode;
            }

            context.BetaFactTypes.Add(element.ValueType);
            var memoryNode = BuildAlphaMemoryNode(element.ValueType, currentNode);
            context.AlphaSource = memoryNode;
        }

        private void BuildAggregateNode(ReteBuilderContext context, AggregateElement element)
        {
            BuildNode(context, element.Source);
            var betaNode = new AggregateNode(context.BetaSource, context.AlphaSource, element.AggregateType);
            context.BetaSource = BuildBetaNodeAssembly(context, betaNode);
            context.AlphaSource = null;
        }

        private static void BuildRuleNode(ReteBuilderContext context, ICompiledRule rule)
        {
            var ruleNode = new RuleNode(rule.Handle, rule.Definition.Priority);
            context.BetaSource.Attach(ruleNode);
        }

        private IBetaMemoryNode BuildBetaNodeAssembly(ReteBuilderContext context, BetaNode betaNode)
        {
            IBetaMemoryNode left = context.BetaSource;
            IAlphaMemoryNode right = context.AlphaSource;
        
            left.Attach(betaNode);
            right.Attach(betaNode);

            IEnumerable<BetaCondition> matchingConditions =
                context.BetaConditions.Where(
                    jc => jc.FactTypes.All(context.BetaFactTypes.Contains)).ToList();

            foreach (var condition in matchingConditions)
            {
                context.BetaConditions.Remove(condition);
                var selectionTable = new List<int>();
                foreach (var factType in condition.FactTypes)
                {
                    int selectionIndex = context.BetaFactTypes.FindIndex(0, t => factType == t);
                    selectionTable.Add(selectionIndex);
                }

                condition.FactSelectionTable = selectionTable.ToArray();
                betaNode.Conditions.Add(condition);
            }

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

        private SelectionNode BuildSlectionNode(AlphaCondition condition, AlphaNode parent)
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

        private AlphaMemoryNode BuildAlphaMemoryNode(Type declarationType, AlphaNode parent)
        {
            AlphaMemoryNode memoryNode = parent.MemoryNode;

            if (memoryNode == null)
            {
                memoryNode = new AlphaMemoryNode(declarationType);
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