using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Dsl
{
    public interface ILeftHandSide
    {
        ILeftHandSide If<T>(Expression<Func<T>> alias, params Expression<Func<T, bool>>[] conditions);
        ILeftHandSide Collect<T>(Expression<Func<IEnumerable<T>>> alias, Expression<Func<T, bool>> itemCondition); 
        ILeftHandSide Exists<T>(Expression<Func<T, bool>> condition);
    }
}