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

        switch (x)
        {
            case LambdaExpression lx:
                {
                    var ly = (LambdaExpression)y;
                    var paramsX = lx.Parameters;
                    var paramsY = ly.Parameters;
                    return CollectionsEqual(paramsX, paramsY, lx, ly) && ExpressionEqual(lx.Body, ly.Body, lx, ly);
                }
            case MemberExpression mex:
                {
                    var mey = (MemberExpression)y;
                    return MemberExpressionsEqual(mex, mey, rootX, rootY);
                }
            case BinaryExpression bx:
                {
                    var by = (BinaryExpression)y;
                    return bx.Method == @by.Method && ExpressionEqual(bx.Left, @by.Left, rootX, rootY) &&
                           ExpressionEqual(bx.Right, @by.Right, rootX, rootY);
                }
            case UnaryExpression ux:
                {
                    var uy = (UnaryExpression)y;
                    return ux.Method == uy.Method && ExpressionEqual(ux.Operand, uy.Operand, rootX, rootY);
                }
            case ParameterExpression px:
                {
                    var py = (ParameterExpression)y;
                    return rootX.Parameters.IndexOf(px) == rootY.Parameters.IndexOf(py) &&
                           px.Name == py.Name;
                }
            case MethodCallExpression cx:
                {
                    var cy = (MethodCallExpression)y;
                    return cx.Method == cy.Method
                           && ExpressionEqual(cx.Object, cy.Object, rootX, rootY)
                           && CollectionsEqual(cx.Arguments, cy.Arguments, rootX, rootY);
                }
            case InvocationExpression ix:
                {
                    var iy = (InvocationExpression)y;
                    return ExpressionEqual(ix.Expression, iy.Expression, rootX, rootY)
                           && CollectionsEqual(ix.Arguments, iy.Arguments, rootX, rootY);
                }
            case NewExpression nx:
                {
                    var ny = (NewExpression)y;
                    return nx.Constructor == ny.Constructor
                           && CollectionsEqual(nx.Arguments, ny.Arguments, rootX, rootY);
                }
            case MemberInitExpression mix:
                {
                    var miy = (MemberInitExpression)y;
                    return ExpressionEqual(mix.NewExpression, miy.NewExpression, rootX, rootY)
                           && MemberBindingCollectionsEqual(mix.Bindings, miy.Bindings, rootX, rootY);
                }
            case ListInitExpression lix:
                {
                    var liy = (ListInitExpression)y;
                    return ExpressionEqual(lix.NewExpression, liy.NewExpression, rootX, rootY)
                           && ElementInitCollectionsEqual(lix.Initializers, liy.Initializers, rootX, rootY);
                }
            case ConstantExpression cx:
                {
                    var cy = (ConstantExpression)y;
                    return Equals(cx.Value, cy.Value);
                }
            case TypeBinaryExpression tbx:
                {
                    var tby = (TypeBinaryExpression)y;
                    return tbx.TypeOperand == tby.TypeOperand;
                }
            case ConditionalExpression cx:
                {
                    var cy = (ConditionalExpression)y;
                    return ExpressionEqual(cx.Test, cy.Test, rootX, rootY)
                           && ExpressionEqual(cx.IfTrue, cy.IfTrue, rootX, rootY)
                           && ExpressionEqual(cx.IfFalse, cy.IfFalse, rootX, rootY);
                }
            case NewArrayExpression ax:
                {
                    var ay = (NewArrayExpression)y;
                    return ax.Type == ay.Type && CollectionsEqual(ax.Expressions, ay.Expressions, rootX, rootY);
                }
            case DefaultExpression dx:
                {
                    var dy = (DefaultExpression)y;
                    return dx.Type == dy.Type;
                }
            case BlockExpression bx:
                {
                    var by = (BlockExpression)y;
                    return ExpressionEqual(bx.Result, by.Result, rootX, rootY)
                        && CollectionsEqual(bx.Expressions, by.Expressions, rootX, rootY);
                }
            default:
                throw new NotImplementedException(x.ToString());
        }
    }

    private static bool MemberBindingCollectionsEqual(IReadOnlyCollection<MemberBinding> x, IReadOnlyCollection<MemberBinding> y,
        LambdaExpression rootX, LambdaExpression rootY)
    {
        return x.Count == y.Count
               && x.Zip(y, (first, second) => new { X = first, Y = second })
                   .All(o => MemberBindingsEqual(o.X, o.Y, rootX, rootY));
    }

    private static bool ElementInitCollectionsEqual(IReadOnlyCollection<ElementInit> x, IReadOnlyCollection<ElementInit> y,
        LambdaExpression rootX, LambdaExpression rootY)
    {
        return x.Count == y.Count
               && x.Zip(y, (first, second) => new { X = first, Y = second })
                   .All(o => ElementInitsEqual(o.X, o.Y, rootX, rootY));
    }

    private static bool CollectionsEqual(IReadOnlyCollection<Expression> x, IReadOnlyCollection<Expression> y,
        LambdaExpression rootX, LambdaExpression rootY)
    {
        return x.Count == y.Count
               && x.Zip(y, (first, second) => new { X = first, Y = second })
                   .All(o => ExpressionEqual(o.X, o.Y, rootX, rootY));
    }

    private static bool ElementInitsEqual(ElementInit x, ElementInit y, LambdaExpression rootX,
        LambdaExpression rootY)
    {
        return Equals(x.AddMethod, y.AddMethod)
               && CollectionsEqual(x.Arguments, y.Arguments, rootX, rootY);
    }

    private static bool MemberBindingsEqual(MemberBinding x, MemberBinding y, LambdaExpression rootX,
        LambdaExpression rootY)
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
                throw new ArgumentOutOfRangeException($"Unsupported binding type. BindingType={x.BindingType}");
        }
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
}