using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rules;

namespace NRules.Core.Rete
{
    internal interface IReteBuilder
    {
        void AddRule(Rule rule);
        INetwork GetNetwork();
    }

    internal class ReteBuilder : IReteBuilder
    {
        private readonly RootNode _root = new RootNode();
        private readonly EventAggregator _aggregator = new EventAggregator();

        public void AddRule(Rule rule)
        {
            var memories = new Queue<AlphaMemory>();

            foreach (var declaration in rule.Declarations)
            {
                Type declarationType = declaration.Type;
                TypeNode typeNode = _root.ChildNodes
                    .Cast<TypeNode>().FirstOrDefault(tn => tn.FilterType == declarationType);

                if (typeNode == null)
                {
                    typeNode = new TypeNode(declarationType);
                    _root.ChildNodes.Add(typeNode);
                }

                AlphaNode currentNode = typeNode;

                IEnumerable<ICondition> conditions = rule.Conditions.Where(c => c.FactType == declarationType);
                foreach (var condition in conditions)
                {
                    currentNode = new SelectionNode(condition);
                    typeNode.ChildNodes.Add(currentNode);
                }

                var memory = new AlphaMemory(declarationType);
                var adapter = new AlphaMemoryAdapter(memory);
                currentNode.ChildNodes.Add(adapter);
                memories.Enqueue(memory);
            }

            var currentFactTypes = new List<Type>();
            var joinConditions = new List<IJoinCondition>(rule.JoinConditions);

            //First
            var alphaMemory = memories.Dequeue();
            currentFactTypes.Add(alphaMemory.FactType);
            var betaAdapter = new BetaAdapter(alphaMemory);
            ITupleMemory left = new BetaMemory(betaAdapter);

            //All other
            while (memories.Count > 0)
            {
                var right = memories.Dequeue();
                currentFactTypes.Add(right.FactType);

                var join = new JoinNode(left, right);

                IEnumerable<IJoinCondition> matchingConditions =
                    rule.JoinConditions.Where(
                        jc => jc.FactTypes.All(currentFactTypes.Contains));

                foreach (var joinCondition in matchingConditions)
                {
                    joinConditions.Remove(joinCondition);
                    var selectionTable = new List<int>();
                    foreach (var factType in joinCondition.FactTypes)
                    {
                        int selectionIndex = currentFactTypes.FindIndex(0, t => factType == t);
                        selectionTable.Add(selectionIndex);
                    }

                    var joinConditionAdapter = new JoinConditionAdaptor(joinCondition, selectionTable.ToArray());
                    join.Conditions.Add(joinConditionAdapter);
                }

                left = new BetaMemory(join);
            }

            var ruleNode = new RuleNode(rule.Handle, _aggregator);
            left.Attach(ruleNode);
        }

        public INetwork GetNetwork()
        {
            INetwork network = new Network(_root, _aggregator);
            return network;
        }
    }
}