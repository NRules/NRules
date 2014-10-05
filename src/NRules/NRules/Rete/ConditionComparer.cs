using System.Collections.Generic;
using System.Linq;

namespace NRules.Rete
{
    internal static class ConditionComparer
    {
        public static bool AreEqual(IEnumerable<IBetaCondition> first, IEnumerable<IBetaCondition> second)
        {
            List<IBetaCondition> collection1 = first.ToList();
            List<IBetaCondition> collection2 = second.ToList();

            if (collection1.Count != collection2.Count) return false;

            for (int i = 0; i < collection1.Count; i++)
            {
                if (!Equals(collection1[i], collection2[i])) return false;
            }

            return true;
        }
    }
}