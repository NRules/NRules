using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rules;

namespace NRules.Core.Rete
{
    internal interface IReteBuilder
    {
        void AddRule(CompiledRule rule);
        INetwork GetNetwork();
    }

    internal class ReteBuilder : IReteBuilder
    {
        private readonly RootNode _root = new RootNode();
        private readonly EventAggregator _aggregator = new EventAggregator();

        public void AddRule(CompiledRule rule)
        {
            var context = new ReteBuilderContext(rule);

            var alphaConditions = rule.Conditions
                .Where(c => c.FactTypes.Count() == 1).ToList();
            var betaConditions = rule.Conditions
                .Where(c => c.FactTypes.Count() > 1).ToList();

            ITupleMemory left = new DummyNode();
            foreach (var predicate in rule.Predicates)
            {
                var type = predicate.Declaration.Type;
                context.BetaFactTypes.Add(type);

                var conditions = alphaConditions.Where(c => c.FactTypes.First() == type);
                var alphaMemory = BuildAlphaSubtree(context, type, conditions);

                BetaNode betaNode;
                switch (predicate.PredicateType)
                {
                    case PredicateTypes.Selection:
                        betaNode = new JoinNode(left, alphaMemory);
                        break;
                    case PredicateTypes.Aggregate:
                        betaNode = new AggregateNode(left, alphaMemory, predicate.StrategyType);
                        break;
                    case PredicateTypes.Existential:
                        betaNode = new ExistsNode(left, alphaMemory);
                        break;
                    default:
                        throw new ArgumentException(string.Format("Unsupported predicate type {0}",
                                                                  predicate.PredicateType));
                }

                left = BuildBetaNodeAssembly(context, betaNode, left, alphaMemory, betaConditions);
            }

            var ruleNode = new RuleNode(rule.Handle, _aggregator);
            left.Attach(ruleNode);
        }

        private ITupleMemory BuildBetaNodeAssembly(ReteBuilderContext context, BetaNode betaNode, ITupleMemory left,
                                                   IObjectMemory right,
                                                   IList<ICondition> conditions)
        {
            left.Attach(betaNode);
            right.Attach(betaNode);

            IEnumerable<ICondition> matchingConditions =
                conditions.Where(
                    jc => jc.FactTypes.All(context.BetaFactTypes.Contains)).ToList();

            foreach (var condition in matchingConditions)
            {
                conditions.Remove(condition);
                var selectionTable = new List<int>();
                foreach (var factType in condition.FactTypes)
                {
                    int selectionIndex = context.BetaFactTypes.FindIndex(0, t => factType == t);
                    selectionTable.Add(selectionIndex);
                }

                var joinConditionAdapter = new JoinConditionAdaptor(condition, selectionTable.ToArray());
                betaNode.Conditions.Add(joinConditionAdapter);
            }

            left = new BetaMemory();
            betaNode.Attach(left);
            return left;
        }

        private AlphaMemory BuildAlphaSubtree(ReteBuilderContext context, Type declarationType,
                                              IEnumerable<ICondition> conditions)
        {
            TypeNode typeNode = BuildTypeNode(declarationType, _root);
            AlphaNode currentNode = typeNode;

            foreach (var condition in conditions)
            {
                SelectionNode selectionNode = BuildSlectionNode(condition, currentNode);
                currentNode = selectionNode;
            }

            var memory = BuildAlphaMemory(declarationType, currentNode);
            return memory;
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
                .OfType<SelectionNode>().FirstOrDefault(sn => sn.Condition.Equals(condition));

            if (selectionNode == null)
            {
                selectionNode = new SelectionNode(condition);
                parent.ChildNodes.Add(selectionNode);
            }
            return selectionNode;
        }

        private AlphaMemory BuildAlphaMemory(Type declarationType, AlphaNode parent)
        {
            AlphaMemoryAdapter adapter = parent.ChildNodes
                .OfType<AlphaMemoryAdapter>().FirstOrDefault();

            if (adapter == null)
            {
                var memory = new AlphaMemory(declarationType);
                adapter = new AlphaMemoryAdapter(memory);
                parent.ChildNodes.Add(adapter);
            }

            return adapter.AlphaMemory;
        }

        public INetwork GetNetwork()
        {
            INetwork network = new Network(_root, _aggregator);
            return network;
        }
    }
}