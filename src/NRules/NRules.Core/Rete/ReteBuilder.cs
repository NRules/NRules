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
                    context.AlphaSource = null;
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
                    context.BetaConditions.Add(conditionElement);
                    continue;
                }

                var alphaCondition = new AlphaCondition(conditionElement.Expression);
                SelectionNode selectionNode = BuildSelectionNode(alphaCondition, currentNode);
                currentNode = selectionNode;
            }

            context.Declarations.Add(element.Declaration);
            context.AlphaSource = BuildAlphaMemoryNode(element.ValueType, currentNode);

            if (context.BetaConditions.Count > 0)
            {
                var betaNode = new JoinNode(context.BetaSource, context.AlphaSource);
                context.BetaSource = BuildBetaNodeAssembly(context, betaNode);
                context.AlphaSource = null;
            }
        }

        private void BuildAggregateNode(ReteBuilderContext context, AggregateElement element)
        {
            //BuildNode(context, element.Source);
            var subnetContext = new ReteBuilderContext(context);
            BuildNode(subnetContext, element.Source);

            if (subnetContext.AlphaSource == null)
            {
                var adapter = new ObjectInputAdapter(subnetContext.BetaSource);
                subnetContext.AlphaSource = adapter;
            }
            context.AlphaSource = subnetContext.AlphaSource;
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
            ITupleSource left = context.BetaSource;
            IObjectSource right = context.AlphaSource;
        
            left.Attach(betaNode);
            right.Attach(betaNode);

            List<ConditionElement> matchingConditions =
                context.BetaConditions.Where(
                    bc => bc.Declarations.All(context.Declarations.Contains)).ToList();

            foreach (var condition in matchingConditions)
            {
                context.BetaConditions.Remove(condition);
                var selectionTable = new List<int>();
                foreach (var declaration in condition.Declarations)
                {
                    int selectionIndex = context.Declarations.FindIndex(0, d => Equals(declaration, d));
                    selectionTable.Add(selectionIndex);
                }

                var betaCondition = new BetaCondition(condition.Expression, selectionTable.ToArray());
                betaNode.Conditions.Add(betaCondition);
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