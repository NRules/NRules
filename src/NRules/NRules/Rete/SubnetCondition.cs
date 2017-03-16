using System;
using System.Collections.Generic;

namespace NRules.Rete
{
    internal class SubnetCondition : IBetaCondition
    {
        public bool IsSatisfiedBy(IExecutionContext context, Tuple leftTuple, Fact rightFact)
        {
            var rightTuple = ((WrapperFact)rightFact).WrappedTuple;
            using (IEnumerator<Fact> leftEnumerator = leftTuple.Facts.GetEnumerator())
            using (IEnumerator<Fact> rightEnumerator = rightTuple.Facts.GetEnumerator())
            {
                AlignEnumerators(leftEnumerator, rightEnumerator, leftTuple.Count, rightTuple.Count);

                while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
                {
                    if (leftEnumerator.Current != rightEnumerator.Current) return false;
                }
            }
            return true;
        }

        private static void AlignEnumerators(IEnumerator<Fact> first, IEnumerator<Fact> second, int firstCount, int secondCount)
        {
            IEnumerator<Fact> biggerEnumerator = firstCount > secondCount ? first : second;
            int diff = Math.Abs(firstCount - secondCount);
            while (diff > 0)
            {
                biggerEnumerator.MoveNext();
                diff--;
            }
        }
    }
}