using System.Linq.Expressions;
using System.Reflection;

namespace NRules.Utilities;

internal class ExpressionComparer
{
    private readonly RuleCompilerOptions _compilerOptions;

    public ExpressionComparer(RuleCompilerOptions compilerOptions)
    {
        _compilerOptions = compilerOptions;
    }

    public bool AreEqual(Expression? x, Expression? y)
    {
        return ExpressionEqual(x, y);
    }

    private bool ExpressionEqual(Expression? x, Expression? y, LambdaExpression? rootX = null, LambdaExpression? rootY = null)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;
        if (x.NodeType != y.NodeType || x.Type != y.Type)
            return false;

        return (x, y) switch
        {
            { x: LambdaExpression lx, y: LambdaExpression ly } => CollectionsEqual(lx.Parameters, ly.Parameters, lx, ly) && ExpressionEqual(lx.Body, ly.Body, lx, ly),
            { x: MemberExpression mex, y: MemberExpression mey } => MemberExpressionsEqual(mex, mey, rootX, rootY),
            { x: BinaryExpression bx, y: BinaryExpression by } => bx.Method == by.Method && ExpressionEqual(bx.Left, by.Left, rootX, rootY) && ExpressionEqual(bx.Right, by.Right, rootX, rootY),
            { x: UnaryExpression ux, y: UnaryExpression uy } => ux.Method == uy.Method && ExpressionEqual(ux.Operand, uy.Operand, rootX, rootY),
            { x: ParameterExpression px, y: ParameterExpression py } _ when rootX is not null && rootY is not null => rootX.Parameters.IndexOf(px) == rootY.Parameters.IndexOf(py),
            { x: MethodCallExpression cx, y: MethodCallExpression cy } => cx.Method == cy.Method && ExpressionEqual(cx.Object, cy.Object, rootX, rootY) && CollectionsEqual(cx.Arguments, cy.Arguments, rootX, rootY),
            { x: InvocationExpression ix, y: InvocationExpression iy } => ExpressionEqual(ix.Expression, iy.Expression, rootX, rootY) && CollectionsEqual(ix.Arguments, iy.Arguments, rootX, rootY),
            { x: NewExpression nx, y: NewExpression ny } => nx.Constructor == ny.Constructor && CollectionsEqual(nx.Arguments, ny.Arguments, rootX, rootY),
            { x: MemberInitExpression mix, y: MemberInitExpression miy } => ExpressionEqual(mix.NewExpression, miy.NewExpression, rootX, rootY) && MemberBindingCollectionsEqual(mix.Bindings, miy.Bindings, rootX, rootY),
            { x: ListInitExpression lix, y: ListInitExpression liy } => ExpressionEqual(lix.NewExpression, liy.NewExpression, rootX, rootY) && ElementInitCollectionsEqual(lix.Initializers, liy.Initializers, rootX, rootY),
            { x: ConstantExpression cx, y: ConstantExpression cy } => Equals(cx.Value, cy.Value),
            { x: TypeBinaryExpression tbx, y: TypeBinaryExpression tby } => tbx.TypeOperand == tby.TypeOperand,
            { x: ConditionalExpression cx, y: ConditionalExpression cy } => ExpressionEqual(cx.Test, cy.Test, rootX, rootY) && ExpressionEqual(cx.IfTrue, cy.IfTrue, rootX, rootY) && ExpressionEqual(cx.IfFalse, cy.IfFalse, rootX, rootY),
            { x: NewArrayExpression ax, y: NewArrayExpression ay } => ax.Type == ay.Type && CollectionsEqual(ax.Expressions, ay.Expressions, rootX, rootY),
            { x: DefaultExpression dx, y: DefaultExpression dy } => dx.Type == dy.Type,
            _ => HandleUnsupportedExpression(x.ToString())
        };
    }

    private bool MemberBindingCollectionsEqual(IReadOnlyCollection<MemberBinding> x, IReadOnlyCollection<MemberBinding> y,
        LambdaExpression? rootX, LambdaExpression? rootY)
    {
        return x.Count == y.Count
               && x.Zip(y, (first, second) => new { X = first, Y = second })
                   .All(o => MemberBindingsEqual(o.X, o.Y, rootX, rootY));
    }

    private bool ElementInitCollectionsEqual(IReadOnlyCollection<ElementInit> x, IReadOnlyCollection<ElementInit> y,
        LambdaExpression? rootX, LambdaExpression? rootY)
    {
        return x.Count == y.Count
               && x.Zip(y, (first, second) => new { X = first, Y = second })
                   .All(o => ElementInitsEqual(o.X, o.Y, rootX, rootY));
    }

    private bool CollectionsEqual(IReadOnlyCollection<Expression> x, IReadOnlyCollection<Expression> y,
        LambdaExpression? rootX, LambdaExpression? rootY)
    {
        return x.Count == y.Count
               && x.Zip(y, (first, second) => new { X = first, Y = second })
                   .All(o => ExpressionEqual(o.X, o.Y, rootX, rootY));
    }

    private bool ElementInitsEqual(ElementInit x, ElementInit y, LambdaExpression? rootX, LambdaExpression? rootY)
    {
        return Equals(x.AddMethod, y.AddMethod)
               && CollectionsEqual(x.Arguments, y.Arguments, rootX, rootY);
    }

    private bool MemberBindingsEqual(MemberBinding x, MemberBinding y, LambdaExpression? rootX, LambdaExpression? rootY)
    {
        if (x.BindingType != y.BindingType)
            return false;

        switch (x.BindingType)
        {
            case MemberBindingType.Assignment:
                var max = (MemberAssignment)x;
                var may = (MemberAssignment)y;
                return ExpressionEqual(max.Expression, may.Expression, rootX, rootY);
            case MemberBindingType.MemberBinding:
                var mmbx = (MemberMemberBinding)x;
                var mmby = (MemberMemberBinding)y;
                return MemberBindingCollectionsEqual(mmbx.Bindings, mmby.Bindings, rootX, rootY);
            case MemberBindingType.ListBinding:
                var mlbx = (MemberListBinding)x;
                var mlby = (MemberListBinding)y;
                return ElementInitCollectionsEqual(mlbx.Initializers, mlby.Initializers, rootX, rootY);
            default:
                return HandleUnsupportedExpression($"Unsupported binding type. BindingType={x.BindingType}");
        }
    }

    private bool MemberExpressionsEqual(MemberExpression x, MemberExpression y, LambdaExpression? rootX, LambdaExpression? rootY)
    {
        if (x.Expression == null || y.Expression == null)
            return Equals(x.Member, y.Member);

        if (x.Expression.NodeType != y.Expression.NodeType)
            return false;

        switch (x.Expression.NodeType)
        {
            case ExpressionType.Constant:
                return Equals(GetValueOfConstantExpression(x), GetValueOfConstantExpression(y));
            case ExpressionType.Parameter:
            case ExpressionType.MemberAccess:
            case ExpressionType.Convert:
            case ExpressionType.Add:
            case ExpressionType.AddChecked:
            case ExpressionType.Subtract:
            case ExpressionType.SubtractChecked:
            case ExpressionType.Multiply:
            case ExpressionType.MultiplyChecked:
            case ExpressionType.Divide:
            case ExpressionType.Modulo:
            case ExpressionType.Power:
            case ExpressionType.Conditional:
                return Equals(x.Member, y.Member) && ExpressionEqual(x.Expression, y.Expression, rootX, rootY);
            case ExpressionType.New:
            case ExpressionType.Call:
                return ExpressionEqual(x.Expression, y.Expression, rootX, rootY);
            default:
                return HandleUnsupportedExpression(x.ToString());
        }
    }

    private static object GetValueOfConstantExpression(MemberExpression mex)
    {
        var o = ((ConstantExpression)mex.Expression).Value;
        return mex.Member switch
        {
            FieldInfo fi => fi.GetValue(o),
            PropertyInfo pi => pi.GetValue(o, null),
            _ => throw new ArgumentException($"Unsupported member type. MemberType={mex.Member.GetType()}")
        };
    }

    private bool HandleUnsupportedExpression(string message)
    {
        return _compilerOptions.UnsupportedExpressionHandling switch
        {
            RuleCompilerUnsupportedExpressionsHandling.FailFast => throw new NotImplementedException(message),
            _ => false
        };
    }
}
