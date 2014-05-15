using System.Collections.Generic;

namespace NRules.Rete
{
    internal class SubnetCondition : IBetaCondition
    {
        public bool IsSatisfiedBy(Tuple leftTuple, Fact rightFact)
        {
            var rightTuple = ((WrapperFact)rightFact).WrappedTuple;
            using (IEnumerator<Fact> enumerator1 = leftTuple.GetEnumerator())
            using (IEnumerator<Fact> enumerator2 = rightTuple.GetEnumerator())
            {
                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    if (enumerator1.Current != enumerator2.Current) return false;
                }
            }
            return true;
        }
    }
}