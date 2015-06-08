using System.Linq;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Rete
{
    internal static class ExpressionMapComparer
    {
        public static bool AreEqual(ExpressionMap first, ExpressionMap second)
        {
            if (first.Count != second.Count) return false;
            //Assume values are sorted
            var pairs = first.Zip(second, System.Tuple.Create);
            bool result = pairs.All(t =>
                t.Item1.Name == t.Item2.Name &&
                ExpressionComparer.AreEqual(t.Item1.Expression, t.Item2.Expression));
            return result;
        }
    }
}