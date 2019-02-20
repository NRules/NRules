using System.Linq;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Rete
{
    internal static class ExpressionCollectionComparer
    {
        public static bool AreEqual(ExpressionCollection first, ExpressionCollection second)
        {
            if (first.Count != second.Count) return false;
            //Values must be in same order
            var pairs = first.Zip(second, System.Tuple.Create);
            bool result = pairs.All(t =>
                t.Item1.Name == t.Item2.Name &&
                ExpressionComparer.AreEqual(t.Item1.Expression, t.Item2.Expression));
            return result;
        }
    }
}