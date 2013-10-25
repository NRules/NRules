using System;
using System.Linq.Expressions;

namespace NRules.Fluent.Dsl
{
    public interface IRightHandSide
    {
        IRightHandSide Do(Expression<Action> action);
    }
}