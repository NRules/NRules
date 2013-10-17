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
            context.BetaMemoryNode = new DummyNode();
            foreach (var childElement in element.ChildElements)
            {
                BuildNode(context, childElement);
                if (context.AlphaMemoryNode != null)
                {
                    var betaNode = new JoinNode(context.BetaMemoryNode, context.AlphaMemoryNode);
                    context.BetaMemoryNode = BuildBetaNodeAssembly(context, betaNode);
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
            var betaNode = new ExistsNode(context.BetaMemoryNode, context.AlphaMemoryNode);
            context.BetaMemoryNode = BuildBetaNodeAssembly(context, betaNode);
            context.AlphaMemoryNode = null;
        }

        private void BuildPatternNode(ReteBuilderContext context, PatternElement element)
        {
            AlphaNode currentNode = BuildTypeNode(element.ValueType, _root);

            foreach (var conditionElement in element.Conditions)
            {
                var condition = new Condition(conditionElement.Expression);

                if (condition.FactTypes.Count() > 1)
                {
                    context.BetaConditions.Add(condition);
                    continue;
                }

                SelectionNode selectionNode = BuildSlectionNode(condition, currentNode);
                currentNode = selectionNode;
            }

            context.BetaFactTypes.Add(element.ValueType);
            var memoryNode = BuildAlphaMemoryNode(element.ValueType, currentNode);
            context.AlphaMemoryNode = memoryNode;
        }

        private void BuildAggregateNode(ReteBuilderContext context, AggregateElement element)
        {
            BuildNode(context, element.Source);
            var betaNode = new AggregateNode(context.BetaMemoryNode, context.AlphaMemoryNode, element.AggregateType);
            context.BetaMemoryNode = BuildBetaNodeAssembly(context, betaNode);
            context.AlphaMemoryNode = null;
        }

        private static void BuildRuleNode(ReteBuilderContext context, ICompiledRule rule)
        {
            var ruleNode = new RuleNode(rule.Handle, rule.Definition.Priority);
            context.BetaMemoryNode.Attach(ruleNode);
        }

        private IBetaMemoryNode BuildBetaNodeAssembly(ReteBuilderContext context, BetaNode betaNode)
        {
            IBetaMemoryNode left = context.BetaMemoryNode;
            IAlphaMemoryNode right = context.AlphaMemoryNode;
        
            left.Attach(betaNode);
            right.Attach(betaNode);

            IEnumerable<ICondition> matchingConditions =
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

                var joinConditionAdapter = new JoinConditionAdapter(condition, selectionTable.ToArray());
                betaNode.Conditions.Add(joinConditionAdapter);
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

        private SelectionNode BuildSlectionNode(ICondition condition, AlphaNode parent)
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