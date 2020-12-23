using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Utilities
{
    internal static class ExpressionElementComparer
    {
        public static bool AreEqual(NamedExpressionElement first, NamedExpressionElement second)
        {
            if (!Equals(first.Name, second.Name))
                return false;
            if (!ExpressionComparer.AreEqual(first.Expression, second.Expression))
                return false;
            return true;
        }
        
        public static bool AreEqual(ExpressionElement first, ExpressionElement second)
        {
            if (!ExpressionComparer.AreEqual(first.Expression, second.Expression))
                return false;
            return true;
        }

        public static bool AreEqual(IEnumerable<NamedExpressionElement> first, IEnumerable<NamedExpressionElement> second)
        {
            using (var enumerator1 = first.GetEnumerator())
            using (var enumerator2 = second.GetEnumerator())
            {
                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    if (!AreEqual(enumerator1.Current, enumerator2.Current))
                        return false;
                }

                if (enumerator1.MoveNext() || enumerator2.MoveNext())
                    return false;
            }

            return true;
        }

        public static bool AreEqual(IEnumerable<ExpressionElement> first, IEnumerable<ExpressionElement> second)
        {
            using (var enumerator1 = first.GetEnumerator())
            using (var enumerator2 = second.GetEnumerator())
            {
                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    if (!AreEqual(enumerator1.Current, enumerator2.Current))
                        return false;
                }

                if (enumerator1.MoveNext() || enumerator2.MoveNext())
                    return false;
            }

            return true;
        }
    }
}
