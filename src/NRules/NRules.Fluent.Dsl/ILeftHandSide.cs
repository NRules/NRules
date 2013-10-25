using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    public interface ILeftHandSide
    {
        ILeftHandSide If<T>(Expression<Func<T>> alias, params Expression<Func<T, bool>>[] conditions);
        ILeftHandSide Collect<T>(Expression<Func<IEnumerable<T>>> alias, params Expression<Func<T, bool>>[] itemConditions);
        ILeftHandSide Exists<T>(params Expression<Func<T, bool>>[] conditions);
    }
}