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
            //var context = new ReteBuilderContext(rule);

            //var alphaConditions = rule.Conditions
            //    .Where(c => c.FactTypes.Count() == 1).ToList();
            //var betaConditions = rule.Conditions
            //    .Where(c => c.FactTypes.Count() > 1).ToList();

            //IBetaMemoryNode left = new DummyNode();
            //foreach (var predicate in rule.Predicates)
            //{
            //    var type = predicate.Declaration.Type;
            //    context.BetaFactTypes.Add(type);

            //    var conditions = alphaConditions.Where(c => c.FactTypes.First() == type);
            //    var alphaMemoryNode = BuildAlphaSubtree(type, conditions);

            //    BetaNode betaNode;
            //    switch (predicate.PredicateType)
            //    {
            //        case PredicateTypes.Selection:
            //            betaNode = new JoinNode(left, alphaMemoryNode);
            //            break;
            //        case PredicateTypes.Aggregate:
            //            betaNode = new AggregateNode(left, alphaMemoryNode, predicate.StrategyType);
            //            break;
            //        case PredicateTypes.Existential:
            //            betaNode = new ExistsNode(left, alphaMemoryNode);
            //            break;
            //        default:
            //            throw new ArgumentException(string.Format("Unsupported predicate type. Type={0}",
            //                                                      predicate.PredicateType));
            //    }

            //    left = BuildBetaNodeAssembly(context, betaNode, left, alphaMemoryNode, betaConditions);
            //}

            //var ruleNode = new RuleNode(rule.Handle);
            //left.Attach(ruleNode);
        }

        //private IBetaMemoryNode BuildBetaNodeAssembly(ReteBuilderContext context, BetaNode betaNode,
        //                                              IBetaMemoryNode left,
        //                                              IAlphaMemoryNode right,
        //                                              IList<ICondition> conditions)
        //{
        //    left.Attach(betaNode);
        //    right.Attach(betaNode);

        //    IEnumerable<ICondition> matchingConditions =
        //        conditions.Where(
        //            jc => jc.FactTypes.All(context.BetaFactTypes.Contains)).ToList();

        //    foreach (var condition in matchingConditions)
        //    {
        //        conditions.Remove(condition);
        //        var selectionTable = new List<int>();
        //        foreach (var factType in condition.FactTypes)
        //        {
        //            int selectionIndex = context.BetaFactTypes.FindIndex(0, t => factType == t);
        //            selectionTable.Add(selectionIndex);
        //        }

        //        var joinConditionAdapter = new JoinConditionAdaptor(condition, selectionTable.ToArray());
        //        betaNode.Conditions.Add(joinConditionAdapter);
        //    }

        //    var memoryNode = new BetaMemoryNode();
        //    betaNode.MemoryNode = memoryNode;
        //    return memoryNode;
        //}

        //private AlphaMemoryNode BuildAlphaSubtree(Type declarationType,
        //                                          IEnumerable<ICondition> conditions)
        //{
        //    TypeNode typeNode = BuildTypeNode(declarationType, _root);
        //    AlphaNode currentNode = typeNode;

        //    foreach (var condition in conditions)
        //    {
        //        SelectionNode selectionNode = BuildSlectionNode(condition, currentNode);
        //        currentNode = selectionNode;
        //    }

        //    var memoryNode = BuildAlphaMemoryNode(declarationType, currentNode);
        //    return memoryNode;
        //}

        //private TypeNode BuildTypeNode(Type declarationType, AlphaNode parent)
        //{
        //    TypeNode typeNode = parent.ChildNodes
        //        .Cast<TypeNode>().FirstOrDefault(tn => tn.FilterType == declarationType);

        //    if (typeNode == null)
        //    {
        //        typeNode = new TypeNode(declarationType);
        //        parent.ChildNodes.Add(typeNode);
        //    }
        //    return typeNode;
        //}

        //private SelectionNode BuildSlectionNode(ICondition condition, AlphaNode parent)
        //{
        //    SelectionNode selectionNode = parent.ChildNodes
        //        .OfType<SelectionNode>().FirstOrDefault(sn => sn.Conditions.First().Equals(condition));

        //    if (selectionNode == null)
        //    {
        //        selectionNode = new SelectionNode(condition);
        //        parent.ChildNodes.Add(selectionNode);
        //    }
        //    return selectionNode;
        //}

        //private AlphaMemoryNode BuildAlphaMemoryNode(Type declarationType, AlphaNode parent)
        //{
        //    AlphaMemoryNode memoryNode = parent.MemoryNode;

        //    if (memoryNode == null)
        //    {
        //        memoryNode = new AlphaMemoryNode(declarationType);
        //        parent.MemoryNode = memoryNode;
        //    }

        //    return memoryNode;
        //}

        public INetwork GetNetwork()
        {
            INetwork network = new Network(_root);
            return network;
        }
    }
}