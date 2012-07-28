using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class ExistsNode : BetaNode
    {
        private readonly Dictionary<Tuple, int> _hitMap = new Dictionary<Tuple, int>();

        public ExistsNode(ITupleMemory leftSource, IObjectMemory rightSource) : base(leftSource, rightSource)
        {
        }

        protected override void PropagateMatchedAssert(Tuple leftTuple, Fact rightFact)
        {
            int count;
            _hitMap.TryGetValue(leftTuple, out count);
            _hitMap[leftTuple] = count + 1;
            if (count == 0)
            {
                Memory.PropagateAssert(leftTuple);
            }
        }

        protected override void PropagateMatchedUpdate(Tuple leftTuple, Fact rightFact)
        {
            Memory.PropagateUpdate(leftTuple);
        }

        protected override void PropagateMatchedRetract(Tuple leftTuple, Fact rightFact)
        {
            int count;
            _hitMap.TryGetValue(leftTuple, out count);
            _hitMap[leftTuple] = count - 1;
            if (count == 1)
            {
                _hitMap.Remove(leftTuple);
                Memory.PropagateRetract(leftTuple);
            }
        }
    }
}