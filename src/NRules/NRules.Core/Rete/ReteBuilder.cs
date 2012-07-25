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

            var selectionPredicates = rule.Predicates
                .Where(p => p.PredicateType == PredicateTypes.Selection);
            foreach (var predicate in selectionPredicates)
            {
                Type declarationType = predicate.Declarations.First().Type;
                IEnumerable<ICondition> conditions = predicate.Conditions;

                BuildAlphaSubtree(context, declarationType, conditions);
            }

            ITupleMemory left = new DummyNode();

            var joinConditions = rule.Predicates
                .Where(p => p.PredicateType == PredicateTypes.Join)
                .SelectMany(p => p.Conditions).ToList();
            left = BuildBetaSubtree(context, left, joinConditions);

            var aggregatePredicates = rule.Predicates
                .Where(p => p.PredicateType == PredicateTypes.Aggregate);
            foreach (var predicate in aggregatePredicates)
            {
                context.BetaFactTypes.Clear();
                left = BuildComposite(context, left, predicate);
            }

            var ruleNode = new RuleNode(rule.Handle, _aggregator);
            left.Attach(ruleNode);
        }

        private ITupleMemory BuildComposite(ReteBuilderContext context, ITupleMemory left,
                                            IPredicate aggregatePredicate)
        {
            ITupleMemory aggregateLeft = new DummyNode();
            foreach (var declaration in aggregatePredicate.Declarations)
            {
                BuildAlphaSubtree(context, declaration.Type, new ICondition[] {});
            }
            aggregateLeft = BuildBetaSubtree(context, aggregateLeft, aggregatePredicate.Conditions);

            var aggregateNode = new AggregateNode(aggregatePredicate.AggregationStrategy, aggregateLeft);
            left = BuildJoin(context, left, aggregatePredicate.Conditions, aggregateNode);

            return left;
        }

        private ITupleMemory BuildBetaSubtree(ReteBuilderContext context, ITupleMemory left,
                                              IList<ICondition> conditions)
        {
            while (context.AlphaMemories.Count > 0)
            {
                var right = context.AlphaMemories.Dequeue();
                context.BetaFactTypes.Add(right.FactType);

                left = BuildJoin(context, left, conditions, right);
            }

            return left;
        }

        private ITupleMemory BuildJoin(ReteBuilderContext context, ITupleMemory left, IList<ICondition> conditions,
                                       IObjectMemory right)
        {
            var join = new JoinNode(left, right);
            left.Attach(@join);
            right.Attach(@join);

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
                @join.Conditions.Add(joinConditionAdapter);
            }

            left = new BetaMemory();
            @join.Attach(left);
            return left;
        }

        private void BuildAlphaSubtree(ReteBuilderContext context, Type declarationType,
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
            context.AlphaMemories.Enqueue(memory);
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