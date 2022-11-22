using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NRules.Json.Tests.Utilities;

public static class ExpressionComparer
{
    public static bool AreEqual(Expression x, Expression y)
    {
        return ExpressionEqual(x, y, null, null);
    }

    private static bool ExpressionEqual(Expression x, Expression y, LambdaExpression rootX, LambdaExpression rootY)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x == null || y == null)
            return false;
        if (x.NodeType != y.NodeType || x.Type != y.Type)
            return false;

        return (x, y) switch
        {
            (LambdaExpression lx, LambdaExpression ly) => CollectionsEqual(lx.Parameters, ly.Parameters, lx, ly)
                && ExpressionEqual(lx.Body, ly.Body, lx, ly),
            (MemberExpression mex, MemberExpression mey) => MemberExpressionsEqual(mex, mey, rootX, rootY),
            (BinaryExpression bx, BinaryExpression by) => bx.Method == @by.Method
                && ExpressionEqual(bx.Left, @by.Left, rootX, rootY)
                && ExpressionEqual(bx.Right, @by.Right, rootX, rootY),
            (UnaryExpression ux, UnaryExpression uy) => ux.Method == uy.Method
                && ExpressionEqual(ux.Operand, uy.Operand, rootX, rootY),
            (ParameterExpression px, ParameterExpression py) => rootX.Parameters.IndexOf(px) == rootY.Parameters.IndexOf(py)
                && px.Name == py.Name,
            (MethodCallExpression cx, MethodCallExpression cy) => cx.Method == cy.Method
                && ExpressionEqual(cx.Object, cy.Object, rootX, rootY)
                && CollectionsEqual(cx.Arguments, cy.Arguments, rootX, rootY),
            (InvocationExpression ix, InvocationExpression iy) => ExpressionEqual(ix.Expression, iy.Expression, rootX, rootY)
                && CollectionsEqual(ix.Arguments, iy.Arguments, rootX, rootY),
            (NewExpression nx, NewExpression ny) => nx.Constructor == ny.Constructor
                && CollectionsEqual(nx.Arguments, ny.Arguments, rootX, rootY),
            (MemberInitExpression mix, MemberInitExpression miy) => ExpressionEqual(mix.NewExpression, miy.NewExpression, rootX, rootY)
                && MemberBindingCollectionsEqual(mix.Bindings, miy.Bindings, rootX, rootY),
            (ListInitExpression lix, ListInitExpression liy) => ExpressionEqual(lix.NewExpression, liy.NewExpression, rootX, rootY)
                && ElementInitCollectionsEqual(lix.Initializers, liy.Initializers, rootX, rootY),
            (ConstantExpression cx, ConstantExpression cy) => Equals(cx.Value, cy.Value),
            (TypeBinaryExpression tbx, TypeBinaryExpression tby) => tbx.TypeOperand == tby.TypeOperand,
            (ConditionalExpression cx, ConditionalExpression cy) => ExpressionEqual(cx.Test, cy.Test, rootX, rootY)
                && ExpressionEqual(cx.IfTrue, cy.IfTrue, rootX, rootY)
                && ExpressionEqual(cx.IfFalse, cy.IfFalse, rootX, rootY),
            (NewArrayExpression ax, NewArrayExpression ay) => CollectionsEqual(ax.Expressions, ay.Expressions, rootX, rootY),
            (DefaultExpression, DefaultExpression) => true,
            (BlockExpression bx, BlockExpression by) => ExpressionEqual(bx.Result, by.Result, rootX, rootY)
                && CollectionsEqual(bx.Expressions, by.Expressions, rootX, rootY),
            _ => throw new NotImplementedException(x.ToString()),
        };
    }

    private static bool MemberBindingCollectionsEqual(IReadOnlyCollection<MemberBinding> x, IReadOnlyCollection<MemberBinding> y, LambdaExpression rootX, LambdaExpression rootY) =>
        x.Count == y.Count
            && x.Zip(y, (first, second) => new { X = first, Y = second })
                .All(o => MemberBindingsEqual(o.X, o.Y, rootX, rootY));

    private static bool ElementInitCollectionsEqual(IReadOnlyCollection<ElementInit> x, IReadOnlyCollection<ElementInit> y, LambdaExpression rootX, LambdaExpression rootY) =>
        x.Count == y.Count
            && x.Zip(y, (first, second) => new { X = first, Y = second })
                .All(o => ElementInitsEqual(o.X, o.Y, rootX, rootY));

    private static bool CollectionsEqual(IReadOnlyCollection<Expression> x, IReadOnlyCollection<Expression> y, LambdaExpression rootX, LambdaExpression rootY) =>
        x.Count == y.Count
            && x.Zip(y, (first, second) => new { X = first, Y = second })
                .All(o => ExpressionEqual(o.X, o.Y, rootX, rootY));

    private static bool ElementInitsEqual(ElementInit x, ElementInit y, LambdaExpression rootX, LambdaExpression rootY) =>
        Equals(x.AddMethod, y.AddMethod)
            && CollectionsEqual(x.Arguments, y.Arguments, rootX, rootY);

    private static bool MemberBindingsEqual(MemberBinding x, MemberBinding y, LambdaExpression rootX, LambdaExpression rootY)
    {
        if (x.BindingType != y.BindingType)
            return false;

        return (x, y) switch
        {
            (MemberAssignment max, MemberAssignment may) => ExpressionEqual(max.Expression, may.Expression, rootX, rootY),
            (MemberMemberBinding mmbx, MemberMemberBinding mmby) => MemberBindingCollectionsEqual(mmbx.Bindings, mmby.Bindings, rootX, rootY),
            (MemberListBinding mlbx, MemberListBinding mlby) => ElementInitCollectionsEqual(mlbx.Initializers, mlby.Initializers, rootX, rootY),
            _ => throw new ArgumentOutOfRangeException($"Unsupported binding type. BindingType={x.BindingType}"),
        };
    }

    private static bool MemberExpressionsEqual(MemberExpression x, MemberExpression y, LambdaExpression rootX, LambdaExpression rootY)
    {
        if (x.Expression == null || y.Expression == null)
            return Equals(x.Member, y.Member);

        if (x.Expression.NodeType != y.Expression.NodeType)
            return false;

        switch (x.Expression.NodeType)
        {
            case ExpressionType.Constant:
                var cx = GetValueOfConstantExpression(x);
                var cy = GetValueOfConstantExpression(y);
                return Equals(cx, cy);
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
                throw new NotImplementedException(x.ToString());
        }
    }

    private static object GetValueOfConstantExpression(MemberExpression mex) =>
        mex switch
        {
            { Expression: ConstantExpression ce, Member: FieldInfo fi } => fi.GetValue(ce.Value),
            { Expression: ConstantExpression ce, Member: PropertyInfo pi } => pi.GetValue(ce.Value, null),
            _ => throw new ArgumentException($"Unsupported member type. MemberType={mex.Member.GetType()}")
        };
}