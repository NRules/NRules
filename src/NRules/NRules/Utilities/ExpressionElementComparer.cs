using System.Collections.Generic;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Utilities
{
    internal static class ExpressionElementComparer
    {
        public static bool AreEqual(NamedExpressionElement first, NamedExpressionElement second)
        {
            if (!Equals(first.Name, second.Name))
                return false;
            return ExpressionComparer.AreEqual(first.Expression, second.Expression);
        }
        
        public static bool AreEqual(ExpressionElement first, ExpressionElement second)
        {
            return ExpressionComparer.AreEqual(first.Expression, second.Expression);
        }

        public static bool AreEqual(
            List<Declaration> firstDeclarations, IEnumerable<NamedExpressionElement> first, 
            List<Declaration> secondDeclarations, IEnumerable<NamedExpressionElement> second)
        {
            using (var enumerator1 = first.GetEnumerator())
            using (var enumerator2 = second.GetEnumerator())
            {
                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    if (!AreParameterPositionsEqual(firstDeclarations, enumerator1.Current, secondDeclarations, enumerator2.Current)) 
                        return false;
                    if (!AreEqual(enumerator1.Current, enumerator2.Current))
                        return false;
                }

                if (enumerator1.MoveNext() || enumerator2.MoveNext())
                    return false;
            }

            return true;
        }

        public static bool AreEqual(
            List<Declaration> firstDeclarations, IEnumerable<ExpressionElement> first,
            List<Declaration> secondDeclarations, IEnumerable<ExpressionElement> second)
        {
            using (var enumerator1 = first.GetEnumerator())
            using (var enumerator2 = second.GetEnumerator())
            {
                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    if (!AreParameterPositionsEqual(firstDeclarations, enumerator1.Current, secondDeclarations, enumerator2.Current)) 
                        return false;
                    if (!AreEqual(enumerator1.Current, enumerator2.Current))
                        return false;
                }

                if (enumerator1.MoveNext() || enumerator2.MoveNext())
                    return false;
            }

            return true;
        }

        private static bool AreParameterPositionsEqual(
            List<Declaration> firstDeclarations, ExpressionElement firstElement, 
            List<Declaration> secondDeclarations, ExpressionElement secondElement)
        {
            var parameterMap1 = IndexMap.CreateMap(firstElement.Imports, firstDeclarations);
            var parameterMap2 = IndexMap.CreateMap(secondElement.Imports, secondDeclarations);

            return Equals(parameterMap1, parameterMap2);
        }
    }
}
