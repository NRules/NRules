using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Utilities;

internal class ExpressionElementComparer
{
    private readonly ExpressionComparer _expressionComparer;

    public ExpressionElementComparer(RuleCompilerOptions compilerOptions)
    {
        _expressionComparer = new ExpressionComparer(compilerOptions);
    }

    public bool AreEqual(NamedExpressionElement first, NamedExpressionElement second)
    {
        return Equals(first.Name, second.Name) && _expressionComparer.AreEqual(first.Expression, second.Expression);
    }

    public bool AreEqual(ExpressionElement first, ExpressionElement second)
    {
        return _expressionComparer.AreEqual(first.Expression, second.Expression);
    }

    public bool AreEqual(
        List<Declaration> firstDeclarations, IEnumerable<NamedExpressionElement> first,
        List<Declaration> secondDeclarations, IEnumerable<NamedExpressionElement> second)
    {
        using var enumerator1 = first.GetEnumerator();
        using var enumerator2 = second.GetEnumerator();

        while (true)
        {
            var hasNext1 = enumerator1.MoveNext();
            var hasNext2 = enumerator2.MoveNext();

            if (hasNext1 && hasNext2)
            {
                if (!AreParameterPositionsEqual(firstDeclarations, enumerator1.Current, secondDeclarations, enumerator2.Current))
                    return false;
                if (!AreEqual(enumerator1.Current, enumerator2.Current))
                    return false;
            }
            else if (hasNext1 || hasNext2)
            {
                return false;
            }
            else
            {
                break;
            }
        }

        return true;
    }

    public bool AreEqual(
        List<Declaration> firstDeclarations, IReadOnlyCollection<ExpressionElement> x,
        List<Declaration> secondDeclarations, IReadOnlyCollection<ExpressionElement> y)
    {
        return x.Count == y.Count
               && x.Zip(y, (first, second) => new { X = first, Y = second })
                   .All(o =>
                       AreParameterPositionsEqual(firstDeclarations, o.X, secondDeclarations, o.Y) &&
                       AreEqual(o.X, o.Y));
    }

    private static bool AreParameterPositionsEqual(
        IEnumerable<Declaration> firstDeclarations, RuleElement firstElement,
        IEnumerable<Declaration> secondDeclarations, RuleElement secondElement)
    {
        var parameterMap1 = IndexMap.CreateMap(firstElement.Imports, firstDeclarations);
        var parameterMap2 = IndexMap.CreateMap(secondElement.Imports, secondDeclarations);

        return Equals(parameterMap1, parameterMap2);
    }
}
