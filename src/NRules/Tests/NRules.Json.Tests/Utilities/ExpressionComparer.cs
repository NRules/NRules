using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NRules.Json.Tests.Utilities;

public static class ExpressionComparer
{
    public static bool AreEqual(Expression x, Expression y) => AreEqual(x, y, null, null);

    private static bool AreEqual(Expression x, Expression y, LambdaExpression rootX, LambdaExpression rootY)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;
        if (x.NodeType != y.NodeType || x.Type != y.Type)
            return false;

        switch ((x, y))
        {
            case (LambdaExpression lx, LambdaExpression ly):
                return AreEqual(lx.Parameters, ly.Parameters, lx, ly)
                    && AreEqual(lx.Body, ly.Body, lx, ly);
            case (MemberExpression mex, MemberExpression mey):
                return AreEqual(mex, mey, rootX, rootY);
            case (BinaryExpression bx, BinaryExpression by):
                return bx.Method == @by.Method
                    && AreEqual(bx.Left, @by.Left, rootX, rootY)
                    && AreEqual(bx.Right, @by.Right, rootX, rootY);
            case (UnaryExpression ux, UnaryExpression uy):
                return ux.Method == uy.Method
                    && AreEqual(ux.Operand, uy.Operand, rootX, rootY);
            case (ParameterExpression px, ParameterExpression py):
                return rootX.Parameters.IndexOf(px) == rootY.Parameters.IndexOf(py)
                    && px.Name == py.Name;
            case (MethodCallExpression cx, MethodCallExpression cy):
                return cx.Method == cy.Method
                    && AreEqual(cx.Object, cy.Object, rootX, rootY)
                    && AreEqual(cx.Arguments, cy.Arguments, rootX, rootY);
            case (InvocationExpression ix, InvocationExpression iy):
                return AreEqual(ix.Expression, iy.Expression, rootX, rootY)
                    && AreEqual(ix.Arguments, iy.Arguments, rootX, rootY);
            case (NewExpression nx, NewExpression ny):
                return nx.Constructor == ny.Constructor
                    && AreEqual(nx.Arguments, ny.Arguments, rootX, rootY);
            case (MemberInitExpression mix, MemberInitExpression miy):
                return AreEqual(mix.NewExpression, miy.NewExpression, rootX, rootY)
                    && AreEqual(mix.Bindings, miy.Bindings, rootX, rootY);
            case (ListInitExpression lix, ListInitExpression liy):
                return AreEqual(lix.NewExpression, liy.NewExpression, rootX, rootY)
                    && AreEqual(lix.Initializers, liy.Initializers, rootX, rootY);
            case (ConstantExpression cx, ConstantExpression cy):
                return Equals(cx.Value, cy.Value);
            case (TypeBinaryExpression tbx, TypeBinaryExpression tby):
                return tbx.TypeOperand == tby.TypeOperand;
            case (ConditionalExpression cx, ConditionalExpression cy):
                return AreEqual(cx.Test, cy.Test, rootX, rootY)
                    && AreEqual(cx.IfTrue, cy.IfTrue, rootX, rootY)
                    && AreEqual(cx.IfFalse, cy.IfFalse, rootX, rootY);
            case (NewArrayExpression ax, NewArrayExpression ay):
                return AreEqual(ax.Expressions, ay.Expressions, rootX, rootY);
            case (DefaultExpression, DefaultExpression):
                return true;
            case (BlockExpression bx, BlockExpression by):
                return AreEqual(bx.Result, by.Result, rootX, rootY)
                    && AreEqual(bx.Expressions, by.Expressions, rootX, rootY);
            default:
                throw new NotImplementedException(x.ToString());
        }
    }

    private static bool AreEqual(IReadOnlyCollection<MemberBinding> x, IReadOnlyCollection<MemberBinding> y, LambdaExpression rootX, LambdaExpression rootY)
    {
        return x.Count == y.Count
            && x.Zip(y, (first, second) => (first, second))
                .All(o => AreEqual(o.first, o.second, rootX, rootY));
    }

    private static bool AreEqual(IReadOnlyCollection<ElementInit> x, IReadOnlyCollection<ElementInit> y, LambdaExpression rootX, LambdaExpression rootY)
    {
        return x.Count == y.Count
            && x.Zip(y, (first, second) => (first, second))
                .All(o => AreEqual(o.first, o.second, rootX, rootY));
    }

    private static bool AreEqual(IReadOnlyCollection<Expression> x, IReadOnlyCollection<Expression> y, LambdaExpression rootX, LambdaExpression rootY)
    {
        return x.Count == y.Count
            && x.Zip(y, (first, second) => (first, second))
                .All(o => AreEqual(o.first, o.second, rootX, rootY));
    }

    private static bool AreEqual(ElementInit x, ElementInit y, LambdaExpression rootX, LambdaExpression rootY)
    {
        return Equals(x.AddMethod, y.AddMethod)
            && AreEqual(x.Arguments, y.Arguments, rootX, rootY);
    }

    private static bool AreEqual(MemberBinding x, MemberBinding y, LambdaExpression rootX, LambdaExpression rootY)
    {
        if (x.BindingType != y.BindingType)
            return false;

        switch ((x, y))
        {
            case (MemberAssignment max, MemberAssignment may):
                return AreEqual(max.Expression, may.Expression, rootX, rootY);
            case (MemberMemberBinding mmbx, MemberMemberBinding mmby):
                return AreEqual(mmbx.Bindings, mmby.Bindings, rootX, rootY);
            case (MemberListBinding mlbx, MemberListBinding mlby):
                return AreEqual(mlbx.Initializers, mlby.Initializers, rootX, rootY);
            default:
                throw new ArgumentOutOfRangeException($"Unsupported binding type. BindingType={x.BindingType}");
        }
    }

    private static bool AreEqual(MemberExpression x, MemberExpression y, LambdaExpression rootX, LambdaExpression rootY)
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
                return Equals(x.Member, y.Member) && AreEqual(x.Expression, y.Expression, rootX, rootY);
            case ExpressionType.New:
            case ExpressionType.Call:
                return AreEqual(x.Expression, y.Expression, rootX, rootY);
            default:
                throw new NotImplementedException(x.ToString());
        }
    }

    private static object GetValueOfConstantExpression(MemberExpression mex)
    {
        switch (mex)
        {
            case { Expression: ConstantExpression ce, Member: FieldInfo fi }:
                return fi.GetValue(ce.Value);
            case { Expression: ConstantExpression ce, Member: PropertyInfo pi }:
                return pi.GetValue(ce.Value, null);
            default:
                throw new ArgumentException($"Unsupported member type. MemberType={mex.Member.GetType()}");
        }
    }
}