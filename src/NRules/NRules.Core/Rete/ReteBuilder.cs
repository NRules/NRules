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
            BuildNode(context, rule.LeftSide);
            BuildRuleNode(context, rule);
        }

        private void BuildNode(ReteBuilderContext context, RuleElement element)
        {
            switch (element.RuleElementType)
            {
                case RuleElementTypes.Match:
                    BuildMatchNode(context, (MatchElement) element);
                    break;
                case RuleElementTypes.Group:
                    BuildGroupNode(context, (GroupElement) element);
                    break;
                case RuleElementTypes.Aggregate:
                    BuildAggregateNode(context, (AggregateElement) element);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("element", 
                        string.Format("Invalid rule element type. ElementType={0}", element.RuleElementType));
            }
        }

        private void BuildGroupNode(ReteBuilderContext context, GroupElement element)
        {
            switch (element.GroupType)
            {
                case GroupType.And:
                    BuildAndGroupNode(context, element);
                    break;
                case GroupType.Or:
                    //fall through - not yet supported
                case GroupType.Not:
                    //fall through - not yet supported
                case GroupType.Exists:
                    BuildExistsGroupNode(context, element);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("element",
                        string.Format("Invalid grouping type. GroupType={0}", element.GroupType));
            }
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

        private void BuildExistsGroupNode(ReteBuilderContext context, GroupElement element)
        {
            BuildNode(context, element.ChildElements.Single());
            var betaNode = new ExistsNode(context.BetaMemoryNode, context.AlphaMemoryNode);
            context.BetaMemoryNode = BuildBetaNodeAssembly(context, betaNode);
            context.AlphaMemoryNode = null;
        }

        private void BuildMatchNode(ReteBuilderContext context, MatchElement element)
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
            var ruleNode = new RuleNode(rule.Handle);
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

                var joinConditionAdapter = new JoinConditionAdaptor(condition, selectionTable.ToArray());
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