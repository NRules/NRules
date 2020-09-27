using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NRules.Utilities
{
    internal static class ExpressionComparer
    {
        public static bool AreEqual(Expression x, Expression y)
        {
            return ExpressionEqual(x, y, null, null);
        }

        private static bool ExpressionEqual(Expression x, Expression y, LambdaExpression rootX, LambdaExpression rootY)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            if (x.NodeType != y.NodeType || x.Type != y.Type) return false;

            if (x is LambdaExpression)
            {
                var lx = (LambdaExpression)x;
                var ly = (LambdaExpression)y;
                var paramsX = lx.Parameters;
                var paramsY = ly.Parameters;
                return CollectionsEqual(paramsX, paramsY, lx, ly) && ExpressionEqual(lx.Body, ly.Body, lx, ly);
            }
            if (x is MemberExpression)
            {
                var mex = (MemberExpression)x;
                var mey = (MemberExpression)y;
                return MemberExpressionsEqual(mex, mey, rootX, rootY);
            }
            if (x is BinaryExpression)
            {
                var bx = (BinaryExpression)x;
                var by = (BinaryExpression)y;
                return bx.Method == by.Method && ExpressionEqual(bx.Left, by.Left, rootX, rootY) &&
                       ExpressionEqual(bx.Right, @by.Right, rootX, rootY);
            }
            if (x is UnaryExpression)
            {
                var ux = (UnaryExpression)x;
                var uy = (UnaryExpression)y;
                return ux.Method == uy.Method && ExpressionEqual(ux.Operand, uy.Operand, rootX, rootY);
            }
            if (x is ParameterExpression)
            {
                var px = (ParameterExpression)x;
                var py = (ParameterExpression)y;
                return rootX.Parameters.IndexOf(px) == rootY.Parameters.IndexOf(py);
            }
            if (x is MethodCallExpression)
            {
                var cx = (MethodCallExpression)x;
                var cy = (MethodCallExpression)y;
                return cx.Method == cy.Method
                       && ExpressionEqual(cx.Object, cy.Object, rootX, rootY)
                       && CollectionsEqual(cx.Arguments, cy.Arguments, rootX, rootY);
            }
            if (x is InvocationExpression)
            {
                var ix = (InvocationExpression)x;
                var iy = (InvocationExpression)y;
                return ExpressionEqual(ix.Expression, iy.Expression, rootX, rootY)
                       && CollectionsEqual(ix.Arguments, iy.Arguments, rootX, rootY);
            }
            if (x is NewExpression)
            {
                var nx = (NewExpression)x;
                var ny = (NewExpression)y;
                return nx.Constructor == ny.Constructor
                       && CollectionsEqual(nx.Arguments, ny.Arguments, rootX, rootY);
            }
            if (x is MemberInitExpression)
            {
                var mix = (MemberInitExpression)x;
                var miy = (MemberInitExpression)y;
                return ExpressionEqual(mix.NewExpression, miy.NewExpression, rootX, rootY)
                       && MemberBindingCollectionsEqual(mix.Bindings, miy.Bindings, rootX, rootY);
            }
            if (x is ListInitExpression)
            {
                var lix = (ListInitExpression)x;
                var liy = (ListInitExpression)y;
                return ExpressionEqual(lix.NewExpression, liy.NewExpression, rootX, rootY)
                       && ElementInitCollectionsEqual(lix.Initializers, liy.Initializers, rootX, rootY);
            }
            if (x is ConstantExpression)
            {
                var cx = (ConstantExpression)x;
                var cy = (ConstantExpression)y;
                return Equals(cx.Value, cy.Value);
            }
            if (x is TypeBinaryExpression)
            {
                var tbx = (TypeBinaryExpression)x;
                var tby = (TypeBinaryExpression)y;
                return Equals(tbx.TypeOperand, tby.TypeOperand);
            }
            if (x is ConditionalExpression)
            {
                var cx = (ConditionalExpression)x;
                var cy = (ConditionalExpression)y;
                return ExpressionEqual(cx.Test, cy.Test, rootX, rootY)
                    && ExpressionEqual(cx.IfTrue, cy.IfTrue, rootX, rootY)
                    && ExpressionEqual(cx.IfFalse, cy.IfFalse, rootX, rootY);
            }
            if (x is NewArrayExpression)
            {
                var ax = (NewArrayExpression) x;
                var ay = (NewArrayExpression) y;
                return Equals(ax.Type, ay.Type) && CollectionsEqual(ax.Expressions, ay.Expressions, rootX, rootY);
            }

            throw new NotImplementedException(x.ToString());
        }

        private static bool MemberBindingCollectionsEqual(IEnumerable<MemberBinding> x, IEnumerable<MemberBinding> y,
            LambdaExpression rootX, LambdaExpression rootY)
        {
            return x.Count() == y.Count()
                   && x.Zip(y, (first, second) => new {X = first, Y = second})
                       .All(o => MemberBindingsEqual(o.X, o.Y, rootX, rootY));
        }

        private static bool ElementInitCollectionsEqual(IEnumerable<ElementInit> x, IEnumerable<ElementInit> y,
            LambdaExpression rootX, LambdaExpression rootY)
        {
            return x.Count() == y.Count()
                   && x.Zip(y, (first, second) => new {X = first, Y = second})
                       .All(o => ElementInitsEqual(o.X, o.Y, rootX, rootY));
        }

        private static bool CollectionsEqual(IEnumerable<Expression> x, IEnumerable<Expression> y,
            LambdaExpression rootX, LambdaExpression rootY)
        {
            return x.Count() == y.Count()
                   && x.Zip(y, (first, second) => new {X = first, Y = second})
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
                    throw new ArgumentOutOfRangeException();
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
                    var constx = GetValueOfConstantExpression(x);
                    var consty = GetValueOfConstantExpression(y);
                    return Equals(constx, consty);
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
            return mex.Member is FieldInfo
                              ? ((FieldInfo)mex.Member).GetValue(o)
                              : ((PropertyInfo)mex.Member).GetValue(o, null);
        }
    }
}
