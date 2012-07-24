using System;
using System.Linq.Expressions;

namespace NRules.Dsl
{
    public interface ILeftHandSide
    {
        ILeftHandSide If<T>(Expression<Func<T, bool>> condition);
        ILeftHandSide If<T1, T2>(Expression<Func<T1, T2, bool>> condition);
        ILeftHandSide Collect<T>(Expression<Func<T, bool>> itemCondition);
        ILeftHandSide Collect<T1, T2>(Expression<Func<T1, T2, bool>> itemCondition);
    }
}