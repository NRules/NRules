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

            foreach (var declaration in rule.Declarations)
            {
                Type declarationType = declaration.Type;
                IEnumerable<ICondition> conditions = declaration.Conditions;

                BuildAlphaSubtree(context, declarationType, conditions);
            }

            ITupleMemory left = new DummyNode();

            var joinConditions = new List<ICondition>(rule.Conditions);
            left = BuildBetaSubtree(context, left, joinConditions);

            foreach (var composite in rule.Composites)
            {
                left = BuildComposite(context, left, composite);
            }

            var ruleNode = new RuleNode(rule.Handle, _aggregator);
            left.Attach(ruleNode);
        }

        private ITupleMemory BuildComposite(ReteBuilderContext context, ITupleMemory left,
                                            ICompositeDeclaration composite)
        {
            ITupleMemory aggregateLeft = new DummyNode();
            foreach (var type in composite.FactTypes)
            {
                BuildAlphaSubtree(context, type, new ICondition[] {});
                aggregateLeft = BuildBetaSubtree(context, aggregateLeft, composite.Conditions);
            }

            var aggregateNode = new AggregateNode(composite.AggregationStrategy, aggregateLeft);
            left = BuildJoin(left, composite.Conditions, composite.FactTypes.ToList(), aggregateNode);

            return left;
        }

        private ITupleMemory BuildBetaSubtree(ReteBuilderContext context, ITupleMemory left,
                                              IList<ICondition> conditions)
        {
            var currentFactTypes = new List<Type>();

            while (context.AlphaMemories.Count > 0)
            {
                var right = context.AlphaMemories.Dequeue();
                currentFactTypes.Add(right.FactType);

                left = BuildJoin(left, conditions, currentFactTypes, right);
            }

            return left;
        }

        private ITupleMemory BuildJoin(ITupleMemory left, IList<ICondition> conditions, List<Type> currentFactTypes,
                                       IObjectMemory right)
        {
            var join = new JoinNode(left, right);
            left.Attach(@join);
            right.Attach(@join);

            IEnumerable<ICondition> matchingConditions =
                conditions.Where(
                    jc => jc.FactTypes.All(currentFactTypes.Contains)).ToList();

            foreach (var condition in matchingConditions)
            {
                conditions.Remove(condition);
                var selectionTable = new List<int>();
                foreach (var factType in condition.FactTypes)
                {
                    int selectionIndex = currentFactTypes.FindIndex(0, t => factType == t);
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
                .OfType<SelectionNode>().FirstOrDefault(sn => sn.Condition.Key == condition.Key);

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